using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Smpn.V20190822.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_SendMessageWarAllianceChatRequestHandler : AMActorRpcHandler<G2M_SendMessageWarAllianceChatRequest, G2M_SendMessageWarAllianceChatResponse>
    {
        protected override async Task<bool> Run(G2M_SendMessageWarAllianceChatRequest b_Request, G2M_SendMessageWarAllianceChatResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2510);// .ERR_WarAllianceDataError;
                b_Reply(b_Response);
                return false;
            }

            var warAllianceMmber = Waralliancecomponent.GetWarAlliance(b_Request.WarAllianceID);
            if (warAllianceMmber == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520);
                b_Reply(b_Response);
                return false;
            }

            var MemberList = Waralliancecomponent.GetWarAllianceMember(b_Request.WarAllianceID);
            if (MemberList != null)
            {
                foreach (var Member in MemberList)
                {
                    if (Member.Value.MeberState == 0)
                    {
                        G2M_SendMessageWarAllianceChatNotice g2M_SendMessageWarAllianceChatNotice = new G2M_SendMessageWarAllianceChatNotice();
                        g2M_SendMessageWarAllianceChatNotice.ActorId = Member.Value.MemberID;
                        g2M_SendMessageWarAllianceChatNotice.GameUserID = Member.Value.MemberID;
                        g2M_SendMessageWarAllianceChatNotice.SendUserName = b_Request.SendUserName;
                        g2M_SendMessageWarAllianceChatNotice.SendGameUserId = b_Request.SendGameUserId;
                        g2M_SendMessageWarAllianceChatNotice.ChatMessage = b_Request.ChatMessage;
                        warAllianceMmber.SendNoticeMember(g2M_SendMessageWarAllianceChatNotice, Member.Value.MeberServerID);
                    }
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}