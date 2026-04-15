using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.LoginCenter)]
    public class Gate2LoginCenter_AddRecordAndLoginHandler : AMRpcHandler<Gate2LoginCenter_AddRecordAndLogin, LoginCenter2Gate_AddRecordAndLogin>
    {
        protected override async Task<bool> CodeAsync(Session session, Gate2LoginCenter_AddRecordAndLogin b_Request, LoginCenter2Gate_AddRecordAndLogin b_Response, Action<IMessage> b_Reply)
        {
            LoginInfoRecordComponent loginInfoRecord = Root.MainFactory.GetCustomComponent<LoginInfoRecordComponent>();

            //using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock,b_Request.UserId))
            {
                if(loginInfoRecord.IsExist(b_Request.UserId))
                {
                    // 账号未下线
                    b_Response.Error = ErrorCode.ERR_AccountNotOffline;
                    b_Response.GateServerId = loginInfoRecord.Get(b_Request.UserId).GateServerId;
                    b_Reply(b_Response);
                    return true;
                }

                switch (b_Request.ConnectIp)
                {
                    case "192.165.5.2":
                        // 白名单
                        break;
                    default:
                        {
                            var mIpConnectDic = Root.MainFactory.GetCustomComponent<IpCacheComponent>().IpConnectDic;
                            if (mIpConnectDic.TryGetValue(b_Request.ConnectIp, out var connectCount) && connectCount > ConstData.LoginLimit)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(118);
                                b_Reply(b_Response);
                                return false;
                            }
                            connectCount++;
                            mIpConnectDic[b_Request.ConnectIp] = connectCount;
                        }
                        break;
                }

                //Console.WriteLine($"在线人数2 : {connectCount}");

                // TODO 添加记录
                loginInfoRecord.Add(new LoginInfo()
                {
                    UserId = b_Request.UserId,
                    GateServerId = b_Request.GateServerId,
                    ConnectIp = b_Request.ConnectIp
                });
                Log.PLog($"a:{b_Request.UserId} 添加玩家在线记录");
            }

            b_Reply(b_Response);
            return true;
        }
    }
}