using Aop.Api.Domain;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Ecm.V20190719.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.LoginCenter)]
    public class G2M_ClaimVerificationHandler : AMActorRpcHandler<G2M_ClaimVerification, M2G_ClaimVerification>
    {
        protected override async Task<bool> Run(G2M_ClaimVerification b_Request, M2G_ClaimVerification b_Response, Action<IMessage> b_Reply)
        {
            var component = Root.MainFactory.GetCustomComponent<PromotionComponent>();
            if (component == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                b_Reply(b_Response);
                return false;
            }

            if (!await component.UPAwardStatus(b_Request.Type, b_Request.UserID))
            {
                b_Response.Error = 2402;
                b_Reply(b_Response);
                return false;
            }
            b_Reply(b_Response);
            return true;

        }
    }
}