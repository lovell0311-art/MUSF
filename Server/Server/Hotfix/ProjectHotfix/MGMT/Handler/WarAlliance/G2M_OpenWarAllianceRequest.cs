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
    public class G2M_OpenWarAllianceRequestHandler : AMActorRpcHandler<G2M_OpenWarAllianceRequest, M2G_OpenWarAllianceResponse>
    {
        protected override async Task<bool> Run(G2M_OpenWarAllianceRequest b_Request, M2G_OpenWarAllianceResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520);//  0;// .ERR_WarAllianceDataError;
                b_Reply(b_Response);
                return false;
            }

            var WarAllianceInfo = Waralliancecomponent.GetWarAlliance(b_Request.WarAllianceID);
            if (WarAllianceInfo == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520);// 0;// .ERR_WarAllianceDataError;
                b_Reply(b_Response);
                return false;
            }
            var memeberInfo = WarAllianceInfo.GetMemberInfo(b_Request.GameUserID);
            if (memeberInfo == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2525);//  0;// .ERR_MemberInfoError;
                b_Reply(b_Response);
                return false;
            }
            b_Response.Info = new GMStruct_WarAllinceInfo();
            b_Response.Info.WarAllianceID = WarAllianceInfo.WarAllianceID;
            b_Response.Info.WarAllianceName = WarAllianceInfo.WarAllianceName;
            b_Response.Info.LeaderName = WarAllianceInfo.AllianceLeaderName;
            b_Response.Info.MemberPost = memeberInfo.MemberPost;
            b_Response.Info.WarAllianceBadge.AddRange(WarAllianceInfo.WarAllianceBadge);
            b_Response.Info.WarAllianceLevel = WarAllianceInfo.WarAllianceLevel;
            b_Response.Info.WarAllianceNotice = WarAllianceInfo.WarAllianNotice;
            b_Response.Info.Currentquantity = WarAllianceInfo.MemberList.Count;
            b_Response.MemberListCont = WarAllianceInfo.MemberList.Count;
            b_Reply(b_Response);
            return true;   
        }
    }
}