using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_DisbandTheWarAllianceRequestHandler : AMActorRpcHandler<G2M_DisbandTheWarAllianceRequest, M2G_DisbandTheWarAllianceResponse>
    {
        protected override async Task<bool> Run(G2M_DisbandTheWarAllianceRequest b_Request, M2G_DisbandTheWarAllianceResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2508);
                b_Reply(b_Response);
                return false;
            }

            var warAllianceMmber = Waralliancecomponent.GetWarAlliance(b_Request.WarAllianceID);
            if (warAllianceMmber == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520); ;
                b_Reply(b_Response);
                return false;
            }

            foreach (var Member in warAllianceMmber.MemberList)
            {
                M2G_DisbandTheWarAllianceNotice m2G_DisbandTheWarAllianceNotice = new M2G_DisbandTheWarAllianceNotice();
                m2G_DisbandTheWarAllianceNotice.ActorId = Member.Value.MemberID;
                m2G_DisbandTheWarAllianceNotice.GameUserID = Member.Value.MemberID;

                if(Member.Value.MeberState == 0)
                    warAllianceMmber.SendNoticeMember(m2G_DisbandTheWarAllianceNotice, Member.Value.MeberServerID);
            }
            int ActorId = (int)( b_Request.AppendData >> 16);
            await Waralliancecomponent.DeleteMemberList(b_Request.WarAllianceID, ActorId);
            await Waralliancecomponent.DeleteWarAlliance(b_Request.WarAllianceID, ActorId);

            b_Reply(b_Response);
            return true;

        }
    }
}