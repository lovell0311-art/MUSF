using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.LoginCenter)]
    public class S2LoginCenter_GetLoginRecordHandler : AMRpcHandler<S2LoginCenter_GetLoginRecord, LoginCenter2S_GetLoginRecord>
    {
        protected override async Task<bool> CodeAsync(Session session, S2LoginCenter_GetLoginRecord b_Request, LoginCenter2S_GetLoginRecord b_Response, Action<IMessage> b_Reply)
        {
            LoginInfoRecordComponent loginInfoRecord = Root.MainFactory.GetCustomComponent<LoginInfoRecordComponent>();
            
            //using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock,b_Request.UserId))
            {
                LoginInfo info = loginInfoRecord.Get(b_Request.UserId);
                if(info != null)
                {
                    b_Response.UserId = info.UserId;
                    b_Response.GateServerId = info.GateServerId;
                    b_Response.GameUserId = info.GameUserId;
                    b_Response.GameServerId = info.GameServerId;
                }
                else
                {
                    b_Response.UserId = 0;
                    b_Response.GateServerId = 0;
                    b_Response.GameUserId = 0;
                    b_Response.GameServerId = 0;
                }
               

                b_Response.Error = ErrorCode.ERR_Success;
                b_Reply(b_Response);
                return true;
            }

            
        }
    }
}