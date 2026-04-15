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
    public class G2M_GetPromotionCodeHandler : AMActorRpcHandler<G2M_GetPromotionCode, M2G_GetPromotionCode>
    {
        protected override async Task<bool> Run(G2M_GetPromotionCode b_Request, M2G_GetPromotionCode b_Response, Action<IMessage> b_Reply)
        {
            var component = Root.MainFactory.GetCustomComponent<PromotionComponent>();
            if (component == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                b_Reply(b_Response);
                return false;
            }

            if (component.GetUserInfo(b_Request.UserID) == null)
            {
                component.CreateCode(b_Request.UserID);
            }

            var Indo = component.GetUserInfo(b_Request.UserID);
            b_Response.Codeinfo = new GMInvitationInfo();
            b_Response.Codeinfo.InvitationCode = Indo.Code;
            b_Response.Codeinfo.NumberCntI = Indo.MemberI > 10 ? 10 : Indo.MemberI;
            b_Response.Codeinfo.AwardStatusI = Indo.MemberStatusI;
            b_Response.Codeinfo.NumberCntII = Indo.MemberII > 10 ? 10 : Indo.MemberII;
            b_Response.Codeinfo.AwardStatusII = Indo.MemberStatusII;
            b_Response.Codeinfo.NumberCntIII = Indo.MemberIII > 200 ? 200 : Indo.MemberIII;
            b_Response.Codeinfo.AwardStatusIII = Indo.MemberIII >= 200 ? 2 : Indo.MemberStatusIII; 

            b_Reply(b_Response);
            return true;

        }
    }
}