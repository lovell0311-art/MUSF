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
    public class G2M_ModifyAnnouncementRequestHandler : AMActorRpcHandler<G2M_ModifyAnnouncementRequest, M2G_ModifyAnnouncementResponse>
    {
        protected override async Task<bool> Run(G2M_ModifyAnnouncementRequest b_Request, M2G_ModifyAnnouncementResponse b_Response, Action<IMessage> b_Reply)
        {
            int AreaId = (int)(b_Request.AppendData >> 16);
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2524); ;// .ERR_WarAllianceDataError;
                b_Reply(b_Response);
                return false;
            }

            var warAllianceMmber = Waralliancecomponent.GetWarAlliance(b_Request.WarAllianceID);
            if (warAllianceMmber == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520); ;// .ERR_WarAllianceInfoError;
                b_Reply(b_Response);
                return false;
            }
            warAllianceMmber.WarAllianNotice = b_Request.Notice;

            DBWarAllianceData dBWarAllianceData = new DBWarAllianceData();
            dBWarAllianceData.DBWarAllianceID = warAllianceMmber.WarAllianceID;
            dBWarAllianceData.DBWarAllianceName = warAllianceMmber.WarAllianceName;
            dBWarAllianceData.DBWarAllianceBadge = warAllianceMmber.WarAllianceBadge;
            dBWarAllianceData.DBWarAllianceLevel = warAllianceMmber.WarAllianceLevel;
            dBWarAllianceData.DBWarAllianNotice = warAllianceMmber.WarAllianNotice;
            dBWarAllianceData.DBWarAllianAreaId = AreaId;
            dBWarAllianceData.IsDisabled = 0;
            if (!await Waralliancecomponent.SetWarAllianceDB(dBWarAllianceData, AreaId))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520); ;// .ERR_WarAllianceInfoError;
                b_Reply(b_Response);
                return false;
            }
            b_Reply(b_Response);
            return true;
        }
    }
}