using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_LoginSystemEnterGameAreaMessageHandler : AMRpcHandler<C2G_LoginSystemEnterGameAreaMessage, G2C_LoginSystemEnterGameAreaMessage>
    {
        protected override async Task<bool> CodeAsync(Session session, C2G_LoginSystemEnterGameAreaMessage b_Request, G2C_LoginSystemEnterGameAreaMessage b_Response, Action<IMessage> b_Reply)
        {
            SessionGateUserComponent mConnectGate = session.GetComponent<SessionGateUserComponent>();
            if (mConnectGate == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(112);
                b_Reply(b_Response);
                return false;
            }
            GateUser mGateUser = Root.MainFactory.GetCustomComponent<GateUserComponent>().GetUserByUserId(mConnectGate.UserId);
            if (mGateUser == null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_AccountNotExists;
                //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
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
                if (mGateUser.GateUserState != GateUserState.Gate)
                {
                    // 某些异常断线/返回登录场景后，GateUser 状态可能残留在 Game。
                    // 这里先尝试回滚到 Gate 状态，再继续本次进区流程。
                    await mGateUser.KickRole();
                    if (mGateUser.GateUserState != GateUserState.Gate)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21007);
                        b_Reply(b_Response);
                        return false;
                    }
                }
                int gameServerId = 0;
                if (b_Request.GameAreaId == 1)
                {
                    gameServerId = b_Request.LineId switch
                    {
                        1 => 257,
                        3 => 259,
                        6 => 262,
                        _ => 0
                    };
                }

                if (gameServerId == 0)
                {
                    G2M_EnterGameGetAreaLineInfoMessage mPackage = new G2M_EnterGameGetAreaLineInfoMessage();
                    mPackage.UserId = mGateUser.UserID;
                    mPackage.AreaId = b_Request.GameAreaId;
                    mPackage.AreaLineId = b_Request.LineId;

                    var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Match);
                    int mServerIndex = Help_RandomHelper.Range(0, mMatchConfigs.Length);
                    var mMatchServerSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mMatchConfigs[mServerIndex].ServerInnerIP);
                    M2G_EnterGameGetAreaLineInfoMessage mResult = await mMatchServerSession.Call(mPackage) as M2G_EnterGameGetAreaLineInfoMessage;
                    if (mResult == null)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21003);
                        //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有匹配到有效的MATCH服务器!");
                        b_Reply(b_Response);
                        return false;
                    }
                    if (mResult.Error != 0)
                    {
                        b_Response.Error = mResult.Error;
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage(mResult.Message);
                        b_Reply(b_Response);
                        return true;
                    }

                    gameServerId = mResult.GameServerId;
                }

                string address = session.RemoteAddress.Address.ToString();
                G2Game_EnterGameAreaMessage mSendPackage = new G2Game_EnterGameAreaMessage()
                {
                    UserId = mGateUser.UserID,
                    AreaId = b_Request.GameAreaId,
                    AreaLineId = b_Request.LineId,
                    GateServerID = mConnectGate.GateServerId,
                    ChannelId = mGateUser.ChannelId,
                    ConnectIp = address
                };
                var mGameConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(gameServerId);
                var mServerSession = Game.Scene.GetComponent<NetInnerComponent>().Get(mGameConfig.ServerInnerIP);
                Game2G_EnterGameAreaMessage mResult2 = await mServerSession.Call(mSendPackage) as Game2G_EnterGameAreaMessage;
                if (mResult2 == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21004);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有匹配到有效的Game服务器!");
                    b_Reply(b_Response);
                    return false;
                }
                if (mResult2.Error != 0)
                {
                    b_Response.Error = mResult2.Error;
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage(mResult2.Message);
                    b_Reply(b_Response);
                    return false;
                }
                {//记录上次登录大区
                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
                    DBAccountInfo dbLoginInfo = null;
                    if (mDBProxy != null)
                    {
                        {
                            var list = await mDBProxy.Query<DBAccountInfo>(p => p.Id == mGateUser.UserID);
                            if (list.Count > 0)
                            {
                                dbLoginInfo = list[0] as DBAccountInfo;
                            }
                        }
                    }
                    if (dbLoginInfo != null)
                    {
                        dbLoginInfo.LastLoginToTheRegion = b_Request.GameAreaId;
                        dbLoginInfo.LastLoginToLine = b_Request.LineId;
                        await mDBProxy.Save(dbLoginInfo);
                    }
                }
                mConnectGate.TransferServerId = gameServerId;
                mConnectGate.GameServerId = gameServerId;
                mConnectGate.GameAreaId = b_Request.GameAreaId;
                mConnectGate.GameAreaLineId = b_Request.LineId;
                mGateUser.GameAreaId = b_Request.GameAreaId;
                mGateUser.GameAreaLineId = b_Request.LineId;
                mGateUser.GameServerId = gameServerId;
                mGateUser.GateUserState = GateUserState.Game;

                b_Response.GameIds.AddRange(mResult2.GameUserIds);
                b_Response.GameOccupation.AddRange(mResult2.GameOccupation);

                b_Reply(b_Response);
                return true;
            }
        }
    }
}
