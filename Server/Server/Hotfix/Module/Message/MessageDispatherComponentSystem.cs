using System;
using System.Collections.Generic;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using CustomFrameWork;

using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class MessageDispatcherComponentAwakeSystem : AwakeSystem<MessageDispatcherComponent>
    {
        public override void Awake(MessageDispatcherComponent self)
        {
            self.Load();
        }
    }

    [ObjectSystem]
    public class MessageDispatcherComponentLoadSystem : LoadSystem<MessageDispatcherComponent>
    {
        public override void Load(MessageDispatcherComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 消息分发组件
    /// </summary>
    [UnsafeAsync]
    public static class MessageDispatcherComponentHelper
    {
        private static bool ShouldTraceRoleFlowMessage(IActorMessage actorMessage)
        {
            return actorMessage is G2C_MovePos_notice || actorMessage is G2C_LoadingComplete;
        }

        private static string DescribeRoleFlowMessage(IActorMessage actorMessage)
        {
            if (actorMessage is G2C_MovePos_notice movePos)
            {
                return $"{actorMessage.GetType().Name} actor={actorMessage.ActorId} gameUserId={movePos.GameUserId} map={movePos.MapId} pos={movePos.X},{movePos.Y} view={movePos.ViewId} needMove={movePos.IsNeedMove}";
            }

            return $"{actorMessage.GetType().Name} actor={actorMessage.ActorId}";
        }

        public static void Load(this MessageDispatcherComponent self)
        {
            self.Handlers.Clear();

            AppType appType = OptionComponent.Options.AppType;

            List<Type> types = Game.EventSystem.GetTypes(typeof(MessageHandlerAttribute));

            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageHandlerAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                MessageHandlerAttribute messageHandlerAttribute = attrs[0] as MessageHandlerAttribute;
                if (!messageHandlerAttribute.Type.Is(appType))
                {
                    continue;
                }

                IMHandler iMHandler = Activator.CreateInstance(type) as IMHandler;
                if (iMHandler == null)
                {
                    Log.Error($"message handle {type.Name} 需要继承 IMHandler");
                    continue;
                }

                Type messageType = iMHandler.GetMessageType();
                ushort opcode = OpcodeTypeComponent.Instance.GetOpcode(messageType);
                if (opcode == 0)
                {
                    Log.Error($"消息opcode为0: {messageType.Name}");
                    continue;
                }
                self.RegisterHandler(opcode, iMHandler);
            }
        }

        public static void RegisterHandler(this MessageDispatcherComponent self, ushort opcode, IMHandler handler)
        {
            if (!self.Handlers.ContainsKey(opcode))
            {
                self.Handlers.Add(opcode, new List<IMHandler>());
            }
            self.Handlers[opcode].Add(handler);
        }

        public static void Handle(this MessageDispatcherComponent self, Session session, MessageInfo messageInfo)
        {
            List<IMHandler> actions;
            if (!self.Handlers.TryGetValue(messageInfo.Opcode, out actions))
            {
                Log.Error($"消息没有处理: {messageInfo.Opcode} {JsonHelper.ToJson(messageInfo.Message)}");
                return;
            }

            foreach (IMHandler ev in actions)
            {
                try
                {
                    ev.Handle(session, messageInfo.Message).Coroutine();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public static async void RunMessageAsync(this MessageDispatcherComponent self, Session session, MessageInfo messageInfo)
        {
            if (self.Handlers.TryGetValue(messageInfo.Opcode, out var actions))
            {// 已经实现包处理 视为目的地
                foreach (IMHandler ev in actions)
                {
                    try
                    {
                        await ev.Handle(session, messageInfo.Message);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
            else if(OptionComponent.Options.AppType != AppType.Robot)
            {
                try
                {
                    bool SendActorMessageTo(long actorId, IActorMessage message)
                    {
                        bool traceRoleFlow = ShouldTraceRoleFlowMessage(message);
                        GatePlayer mGatePlayer = Root.MainFactory.GetCustomComponent<GatePlayerComponent>().Get(actorId);
                        if (mGatePlayer == null)
                        {
                            if (traceRoleFlow)
                            {
                                Log.Warning($"#RoleFlow# GateRoute miss-gate-player actor={actorId} msg={DescribeRoleFlowMessage(message)}");
                            }
                            return false;
                        }
                        if (mGatePlayer.Session != null &&
                            mGatePlayer.Session.IsDisposed == false &&
                            mGatePlayer.Session.InstanceId == mGatePlayer.SessionInstanceId)
                        {
                            if (traceRoleFlow)
                            {
                                Log.Info($"#RoleFlow# GateRoute forward actor={actorId} session={mGatePlayer.Session.Id} sessionInstance={mGatePlayer.Session.InstanceId} gatePlayerSessionInstance={mGatePlayer.SessionInstanceId} msg={DescribeRoleFlowMessage(message)}");
                            }
                            message.ActorId = 0;

                            mGatePlayer.Session.Send(message);
                            return true;
                        }
                        if (traceRoleFlow)
                        {
                            Log.Warning($"#RoleFlow# GateRoute stale-session actor={actorId} session={(mGatePlayer.Session?.Id ?? 0)} sessionDisposed={(mGatePlayer.Session?.IsDisposed ?? true)} sessionInstance={(mGatePlayer.Session?.InstanceId ?? 0)} gatePlayerSessionInstance={mGatePlayer.SessionInstanceId} msg={DescribeRoleFlowMessage(message)}");
                        }
                        return false;
                    }

                    switch (messageInfo.Message)
                    {
                        case IActorMessage iactorMessage:
                            {
                                var netConnectGate = session.GetComponent<SessionGateUserComponent>();
                                if (netConnectGate == null)
                                {
                                    if(iactorMessage.ActorId != 0)
                                    {
                                        SendActorMessageTo(iactorMessage.ActorId, iactorMessage);
                                    }
                                    else if(iactorMessage is IActorListMessage iactorListMessage)
                                    {
                                        using ListComponent<long> actorIdList = ListComponent<long>.Create();
                                        actorIdList.AddRange(iactorListMessage.ActorIdList);
                                        iactorListMessage.ActorIdList.Clear();
                                        foreach (long actorId in actorIdList)
                                        {
                                            SendActorMessageTo(actorId, iactorListMessage);
                                        }
                                    }
                                }
                            }
                            break;
                        case IActorRequest mRequest:
                            {
                                var netConnectGate = session.GetComponent<SessionGateUserComponent>();
                                if (netConnectGate != null)
                                {
                                    if (netConnectGate.TransferServerId == 0)
                                    {
                                        Log.Error($"玩家找不到或者ID为0 {netConnectGate.TransferServerId == 0}\n GameRouteMessage {mRequest.GetType().Name} : {Newtonsoft.Json.JsonConvert.SerializeObject(messageInfo.Message)}");
                                        return;
                                    }
    #if NOLOCATION
                                    int rpcId = mRequest.RpcId; // 这里要保存客户端的rpcId
                                    long instanceId = session.InstanceId;

                                    //ActorMessageSender actorMessageSender = Game.Scene.GetComponent<ActorMessageSenderComponent>().Get(unitId);
                                    mRequest.ActorId = netConnectGate.GameUserId;
                                    mRequest.AppendData = (long)((uint)netConnectGate.GameAreaId << 16 | (uint)netConnectGate.GameAreaLineId);

                                    var ipEndPoint = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(netConnectGate.TransferServerId).ServerInnerIP;
                                    //var ipEndPoint = StartConfigComponent.Instance.GetInnerAddress(netConnectGate.TransferServerId);
                                    Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint);
                                    IResponse response = await targetSession.CallWithoutException(mRequest);
    #else
							        ActorLocationSender actorLocationSender = Game.Scene.GetComponent<ActorLocationSenderComponent>().Get(unitId);
							        int rpcId = actorLocationRequest.RpcId; // 这里要保存客户端的rpcId
							        long instanceId = session.InstanceId;
							        IResponse response = await actorLocationSender.Call(actorLocationRequest);
    #endif
                                    response.RpcId = rpcId;
                                    // session可能已经断开了，所以这里需要判断
                                    if (session.InstanceId == instanceId)
                                    {
                                        session.Reply(response);
                                    }
                                }
                            }
                            break;
                        default:
                            Log.Warning($"Receive 发送数据问题:无处理   {messageInfo.Message.GetType().Name} : {Newtonsoft.Json.JsonConvert.SerializeObject(messageInfo.Message)}");

                            break;
                    }
                }
                catch(Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}
