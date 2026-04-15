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
    public class G2M_CheckExtensionCodeHandler : AMActorRpcHandler<G2M_CheckExtensionCode, M2G_CheckExtensionCode>
    {
        protected override async Task<bool> Run(G2M_CheckExtensionCode b_Request, M2G_CheckExtensionCode b_Response, Action<IMessage> b_Reply)
        {
            var component = Root.MainFactory.GetCustomComponent<PromotionComponent>();
            if (component == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                b_Reply(b_Response);
                return false;
            }
            if (component.PromotionUserCode.TryGetValue(b_Request.Code, out long Value))
            {
                if (await component.UPNumberOfGuests(0, b_Request.Code, b_Request.UserID))
                {
                    b_Reply(b_Response);
                    return true;
                }
            }
            b_Response.Error = 135;
            b_Reply(b_Response);
            return true;

        }
    }
}