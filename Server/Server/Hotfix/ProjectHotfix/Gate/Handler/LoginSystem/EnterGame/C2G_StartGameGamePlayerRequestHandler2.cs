
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_StartGameGamePlayerRequestHandler2 : AMActorRpcHandler<C2G_StartGameGamePlayerRequest, G2C_StartGameGamePlayerResponse>
    {
        protected override Task<bool> BeforeCodeAsync(Session b_Connect, C2G_StartGameGamePlayerRequest b_Request, G2C_StartGameGamePlayerResponse b_Response, Action<IMessage> b_Reply)
        {
            return base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
        }

        protected override async Task<bool> CodeAsync(Session session, C2G_StartGameGamePlayerRequest b_Request, G2C_StartGameGamePlayerResponse b_Response, Action<IMessage> b_Reply)
        {
            Log.Info($"#RoleFlow# GateStartGame start session={session?.Id ?? 0} rpc={b_Request.RpcId} gameUserId={b_Request.GameUserId}");
            SessionGateUserComponent mConnectGate = session.GetComponent<SessionGateUserComponent>();
            if (mConnectGate == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(112);
                b_Reply(b_Response);
                return false;
            }
            GateUser mGatePlayer = Root.MainFactory.GetCustomComponent<GateUserComponent>().GetUserByUserId(mConnectGate.UserId);
            if (mGatePlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(120);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家不存在!");
                b_Reply(b_Response);
                return false;
            }
            long instanceId = session.InstanceId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, mConnectGate.UserId))
            {
                if (instanceId != session.InstanceId)
                {
                    // TODO 防止串号
                    b_Response.Error = ErrorCode.ERR_SessionChanged;
                    b_Reply(b_Response);
                    return false;
                }
                if(mGatePlayer.GateUserState != GateUserState.Game)
                {
                    if (!TryRecoverGateAreaState(session, mConnectGate, mGatePlayer))
                    {
                        // 没有进入区服
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21009);
                        b_Reply(b_Response);
                        return false;
                    }
                }

                if (mConnectGate.GameUserId != 0 && mConnectGate.GameUserId != b_Request.GameUserId)
                {
                    Log.Warning($"#RoleFlow# GateStartGame clear stale session-gameUser session={session?.Id ?? 0} requestGameUserId={b_Request.GameUserId} sessionGameUserId={mConnectGate.GameUserId}");
                    mConnectGate.GameUserId = 0;
                }

                if (mGatePlayer.GatePlayer != null)
                {
                    await ClearStaleGatePlayerAsync(session, mConnectGate, mGatePlayer, b_Request.GameUserId);
                }

                b_Request.ActorId = mConnectGate.UserId;
                b_Request.AppendData = (long)((uint)mConnectGate.GameAreaId << 16 | (uint)mConnectGate.GameAreaLineId);
                int mCallTagId = b_Request.RpcId;

                //var ipEndPoint = StartConfigComponent.Instance.GetInnerAddress(mConnectGate.GameServerId);
                var ipEndPoint = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(mConnectGate.GameServerId);
                Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(ipEndPoint.ServerInnerIP);
                G2C_StartGameGamePlayerResponse mResult2 = await targetSession.Call(b_Request) as G2C_StartGameGamePlayerResponse;
                if (mResult2 == null)
                {
                    Log.Warning($"#RoleFlow# GateStartGame target null session={session?.Id ?? 0} gameUserId={b_Request.GameUserId}");
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21004);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有匹配到有效的Game服务器!");
                    b_Reply(b_Response);
                    return true;
                }
                if (mResult2.Error != 0)
                {
                    Log.Warning($"#RoleFlow# GateStartGame target error session={session?.Id ?? 0} gameUserId={b_Request.GameUserId} error={mResult2.Error}");
                    b_Response.Error = mResult2.Error;
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage(mResult2.Message);
                    b_Reply(b_Response);
                    return true;
                }

                mConnectGate.GameUserId = mResult2.GameUserId;

                GatePlayer mPlayer = Root.MainFactory.GetCustomComponent<GatePlayerComponent>().AddPlayerByUserID(mConnectGate.GameUserId);
                mPlayer.Session = session;
                mPlayer.SessionInstanceId = session.InstanceId;
                mPlayer.UserId = mGatePlayer.UserID;
                mPlayer.GameServerId = mConnectGate.GameServerId;
                mGatePlayer.GatePlayer = mPlayer;

                try
                {
                    var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                    Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                    LoginCenter2Gate_SetLoginRecord l2GSetLoginRecord = (LoginCenter2Gate_SetLoginRecord)await loginCenterSession.Call(new Gate2LoginCenter_SetLoginRecord()
                    {
                        UserId = mGatePlayer.UserID,
                        GateServerId = OptionComponent.Options.AppId,
                        GameUserId = mConnectGate.GameUserId,
                        GameServerId = mConnectGate.GameServerId
                    });
                    if(l2GSetLoginRecord.Error != ErrorCode.ERR_Success)
                    {
                        Log.Error($"{l2GSetLoginRecord.Error}:{l2GSetLoginRecord.Message}");
                    }
                }catch(Exception e)
                {
                    Log.Error(e);
                }

                mResult2.GameUserId = 0;
                mResult2.RpcId = mCallTagId;
                Log.Info($"#RoleFlow# GateStartGame finish session={session?.Id ?? 0} gameUserId={mConnectGate.GameUserId} rpc={mCallTagId}");
                b_Reply(mResult2);
                return true;
            }
        }

        private static async Task ClearStaleGatePlayerAsync(Session session, SessionGateUserComponent connectGate, GateUser gateUser, long requestGameUserId)
        {
            GatePlayer staleGatePlayer = gateUser?.GatePlayer;
            if (staleGatePlayer == null)
            {
                return;
            }

            Log.Warning($"#RoleFlow# GateStartGame stale gate-player detected session={session?.Id ?? 0} requestGameUserId={requestGameUserId} staleGameUserId={staleGatePlayer.GameUserId} staleSessionId={staleGatePlayer.Session?.Id ?? 0} staleSessionInstanceId={staleGatePlayer.SessionInstanceId} gameServerId={staleGatePlayer.GameServerId}");

            try
            {
                int targetGameServerId = staleGatePlayer.GameServerId != 0 ? staleGatePlayer.GameServerId : connectGate.GameServerId;
                if (targetGameServerId != 0)
                {
                    var startUpInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(targetGameServerId);
                    Session targetSession = Game.Scene.GetComponent<NetInnerComponent>().Get(startUpInfo.ServerInnerIP);
                    if (targetSession != null)
                    {
                        await targetSession.Call(new S2Game_RequestExitGame() { UserId = gateUser.UserID });
                    }
                    else
                    {
                        Log.Warning($"#RoleFlow# GateStartGame stale cleanup target-session-missing session={session?.Id ?? 0} requestGameUserId={requestGameUserId} gameServerId={targetGameServerId}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"#RoleFlow# GateStartGame stale cleanup failed session={session?.Id ?? 0} requestGameUserId={requestGameUserId} staleGameUserId={staleGatePlayer.GameUserId}\n{e}");
            }
            finally
            {
                GatePlayer removedGatePlayer = Root.MainFactory.GetCustomComponent<GatePlayerComponent>().Remove(staleGatePlayer.GameUserId);
                if (removedGatePlayer != null && removedGatePlayer.IsDisposeable == false)
                {
                    removedGatePlayer.Dispose();
                }

                if (ReferenceEquals(gateUser.GatePlayer, staleGatePlayer))
                {
                    gateUser.GatePlayer = null;
                }

                if (connectGate.GameUserId == staleGatePlayer.GameUserId)
                {
                    connectGate.GameUserId = 0;
                }

                Log.Warning($"#RoleFlow# GateStartGame stale gate-player cleared session={session?.Id ?? 0} requestGameUserId={requestGameUserId} staleGameUserId={staleGatePlayer.GameUserId}");
            }
        }

        private static bool TryRecoverGateAreaState(Session session, SessionGateUserComponent connectGate, GateUser gateUser)
        {
            if (connectGate == null || gateUser == null)
            {
                return false;
            }

            if (connectGate.GameServerId == 0 || connectGate.GameAreaId == 0 || connectGate.GameAreaLineId == 0)
            {
                return false;
            }

            gateUser.GameServerId = connectGate.GameServerId;
            gateUser.GameAreaId = connectGate.GameAreaId;
            gateUser.GameAreaLineId = connectGate.GameAreaLineId;
            gateUser.GateUserState = GateUserState.Game;
            Log.Warning($"#RoleFlow# GateStartGame recover gate-state session={session?.Id ?? 0} area={gateUser.GameAreaId} line={gateUser.GameAreaLineId} gameServerId={gateUser.GameServerId}");
            return true;
        }
    }
}
