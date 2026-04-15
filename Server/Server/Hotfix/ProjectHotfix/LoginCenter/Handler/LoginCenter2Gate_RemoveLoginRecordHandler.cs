using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.LoginCenter)]
    public class LoginCenter2Gate_RemoveLoginRecordHandler : AMRpcHandler<LoginCenter2Gate_RemoveLoginRecord, Gate2LoginCenter_RemoveLoginRecord>
    {
        protected override async Task<bool> CodeAsync(Session session, LoginCenter2Gate_RemoveLoginRecord b_Request, Gate2LoginCenter_RemoveLoginRecord b_Response, Action<IMessage> b_Reply)
        {
            LoginInfoRecordComponent loginInfoRecord = Root.MainFactory.GetCustomComponent<LoginInfoRecordComponent>();

            //using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock,b_Request.UserId))
            {
                LoginInfo info = loginInfoRecord.Get(b_Request.UserId);
                if (info == null)
                {
                    b_Response.Error = ErrorCode.ERR_LoginRecordError;
                    b_Reply(b_Response);
                    return true;
                }
                if (info.GateServerId != b_Request.GateServerId)
                {
                    b_Response.Error = ErrorCode.ERR_LoginRecordError2;
                    b_Reply(b_Response);
                    return true;
                }
                loginInfoRecord.Remove(b_Request.UserId);

                var mIpConnectDic = Root.MainFactory.GetCustomComponent<IpCacheComponent>().IpConnectDic;
                if (mIpConnectDic.TryGetValue(info.ConnectIp, out var connectCount))
                {
                    connectCount--;
                    if (connectCount < 0) connectCount = 0;
                    mIpConnectDic[info.ConnectIp] = connectCount;
                }
                //Console.WriteLine($"在线人数 : {connectCount}");

                Log.PLog($"a:{b_Request.UserId} 移除玩家在线记录");
            }

            b_Reply(b_Response);
            return true;
        }
    }
}