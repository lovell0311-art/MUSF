using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2Gate_KickRoleHandler : AMRpcHandler<C2Gate_KickRole, Gate2C_KickRole>
    {
        protected override async Task<bool> CodeAsync(Session session, C2Gate_KickRole b_Request, Gate2C_KickRole b_Response, Action<IMessage> b_Reply)
        {
            SessionGateUserComponent mConnectGate = session.GetComponent<SessionGateUserComponent>();
            if(mConnectGate == null)
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
            await mGateUser.KickRole();
            b_Reply(b_Response);
            return true;
        }
    }
}