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
    public class M2G_SearchWarAllianceRequestHandler : AMActorRpcHandler<M2G_SearchWarAllianceRequest, M2G_SearchWarAllianceResponse>
    {
        protected override async Task<bool> Run(M2G_SearchWarAllianceRequest b_Request, M2G_SearchWarAllianceResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2526); ;// .ERR_WarAllianceDataError;
                b_Reply(b_Response);
                return false;
            }

            var WarAllianceInfo =Waralliancecomponent.GetNameWarAlliance(b_Request.WarAllianceName);
            if (WarAllianceInfo == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520);// .ERR_WarAllianceDataError;
                b_Reply(b_Response);
                return false;
            }

            b_Response.WarAllianceID = WarAllianceInfo.WarAllianceID;
            b_Response.WarAllianceName = WarAllianceInfo.WarAllianceName;
            b_Response.WarAllianceLevel= WarAllianceInfo.MemberList.Count;
            b_Response.AllianceLeaderName = WarAllianceInfo.AllianceLeaderName;
            b_Reply(b_Response);
            return true ;

        }
    }
}