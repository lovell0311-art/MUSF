using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.LoginCenter)]
    public class Gate2LoginCenter_SetLoginRecordHandler : AMRpcHandler<Gate2LoginCenter_SetLoginRecord, LoginCenter2Gate_SetLoginRecord>
    {
        protected override async Task<bool> CodeAsync(Session session, Gate2LoginCenter_SetLoginRecord b_Request, LoginCenter2Gate_SetLoginRecord b_Response, Action<IMessage> b_Reply)
        {
            LoginInfoRecordComponent loginInfoRecord = Root.MainFactory.GetCustomComponent<LoginInfoRecordComponent>();
            
            //using(await CoroutineLockComponent.Instance.Wait(CoroutineLockType.LoginCenterLock,b_Request.UserId))
            {

                // TODO 设置记录
                LoginInfo info = loginInfoRecord.Get(b_Request.UserId);
                if(info == null)
                {
                    b_Response.Error = ErrorCode.ERR_LoginRecordError;
                    b_Reply(b_Response);
                    return true;
                }
                if(info.GateServerId != b_Request.GateServerId)
                {
                    b_Response.Error = ErrorCode.ERR_LoginRecordError2;
                    b_Reply(b_Response);
                    return true;
                }
                info.GameUserId = b_Request.GameUserId;
                info.GameServerId = b_Request.GameServerId;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}