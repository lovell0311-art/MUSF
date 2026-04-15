using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.LoginCenter)]
    public class Realm2LoginCenter_LoginAccountHandler : AMRpcHandler<Realm2LoginCenter_LoginAccount, LoginCenter2Realm_LoginAccount>
    {
        protected override async Task<bool> CodeAsync(Session session, Realm2LoginCenter_LoginAccount b_Request, LoginCenter2Realm_LoginAccount b_Response, Action<IMessage> b_Reply)
        {
            LoginInfoRecordComponent loginInfoRecord = Root.MainFactory.GetCustomComponent<LoginInfoRecordComponent>();
            
            //using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock,b_Request.UserId))
            {
                if(!loginInfoRecord.IsExist(b_Request.UserId))
                {
                    b_Response.Error = ErrorCode.ERR_Success;
                    b_Reply(b_Response);
                    return true;
                }

                // 玩家在线
                // TODO 返回GateServerId，让登录服踢
                LoginInfo info = loginInfoRecord.Get(b_Request.UserId);

                b_Response.GateServerId = info.GateServerId;
                // 账号未下线
                b_Response.Error = ErrorCode.ERR_AccountNotOffline;
                b_Reply(b_Response);
                return true;
            }

            
        }
    }
}