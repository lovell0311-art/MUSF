using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_LoginGateHandler : AMRpcHandler<C2G_LoginGateRequest, G2C_LoginGateResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2G_LoginGateRequest b_Request, G2C_LoginGateResponse b_Response, Action<IMessage> b_Reply)
        {
            string account = Root.MainFactory.GetCustomComponent<GateSessionKeyComponent>().Get(b_Request.Key);
            if (account == null || account == "")
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(110);
                //response.Message = language.GetMessage("Gate key验证失败!");
                b_Reply(b_Response);
                return false;
            }

            // 移除key
            Root.MainFactory.GetCustomComponent<GateSessionKeyComponent>().Remove(b_Request.Key);
            long mAccount = long.Parse(account);

            long instanceId = session.InstanceId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginGate, mAccount))
            {
                if (instanceId != session.InstanceId)
                {
                    // TODO 防止串号
                    b_Response.Error = ErrorCode.ERR_SessionChanged;
                    b_Reply(b_Response);
                    return false;
                }
                string address = session.RemoteAddress.Address.ToString();

                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                LoginCenter2Gate_AddRecordAndLogin l2GAddRecordAndLogin = (LoginCenter2Gate_AddRecordAndLogin)await loginCenterSession.Call(new Gate2LoginCenter_AddRecordAndLogin()
                {
                    UserId = mAccount,
                    GateServerId = OptionComponent.Options.AppId,
                    ConnectIp = address
                });
                if (l2GAddRecordAndLogin.Error == ErrorCode.ERR_AccountNotOffline)
                {
                    // 下线账号
                    int gateServerId = l2GAddRecordAndLogin.GateServerId;
                    async Task DisconnectGateUser()
                    {
                        var gateConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Gate, gateServerId);
                        Session gateSession2 = Game.Scene.GetComponent<NetInnerComponent>().Get(gateConfig.ServerInnerIP);
                        Gate2S_DisconnectGateUser g2SDisconneectGateUser = (Gate2S_DisconnectGateUser)await gateSession2.Call(new S2Gate_DisconnectGateUser() { UserId = mAccount });
                        if (g2SDisconneectGateUser.Error != ErrorCode.ERR_Success)
                        {
                            Log.Warning($"顶号失败! g2SDisconneectGateUser.Error={g2SDisconneectGateUser.Error}");
                        }
                    }
                    DisconnectGateUser().Coroutine();

                    // 重新生成一个key,用于下次连接
                    b_Response.NewKey = Help_UniqueValueHelper.GetServerUniqueValue();
                    Root.MainFactory.GetCustomComponent<GateSessionKeyComponent>().Add(account, b_Response.NewKey);

                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(111);
                    b_Reply(b_Response);
                    return true;
                }
                else if (l2GAddRecordAndLogin.Error != ErrorCode.ERR_Success)
                {
                    b_Response.Error = l2GAddRecordAndLogin.Error;
                    b_Reply(b_Response);
                    return true;
                }

                SessionGateUserComponent mSessionUserComponent = session.AddComponent<SessionGateUserComponent>();
                mSessionUserComponent.UserId = mAccount;
                mSessionUserComponent.GateServerId = OptionComponent.Options.AppId;

                GateUser user = Root.MainFactory.GetCustomComponent<GateUserComponent>().AddUserByUserId(mAccount);
                user.ChannelId = b_Request.ChannelId;
                user.ClientSession = session;
                user.CSessionInstanceId = session.InstanceId;
                user.GateUserState = GateUserState.Gate;

                // 定时给客户端发送新Key
                session.AddComponent<GateSessionKeyNoticeComponent>();
                session.AddComponent<NetworkSendLogComponent>();
                Log.Info($"#Session# a:{mAccount} session.Id:{session.Id}");

                // 重新生成一个key,用于断线重连
                b_Response.NewKey = Help_UniqueValueHelper.GetServerUniqueValue();
                Root.MainFactory.GetCustomComponent<GateSessionKeyComponent>().Add(account, b_Response.NewKey, 1000 * 60 * 8);
            }


            b_Reply(b_Response);
            return true;
        }
    }
}