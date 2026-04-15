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
    public class G2M_WarAllianceSignOutRequestHandler : AMActorRpcHandler<G2M_WarAllianceSignOutRequest, M2G_WarAllianceSignOutResponse>
    {
        protected override async Task<bool> Run(G2M_WarAllianceSignOutRequest b_Request, M2G_WarAllianceSignOutResponse b_Response, Action<IMessage> b_Reply)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2520);
                b_Reply(b_Response);
                return false;
            }

            var WarAllianceInfo = Waralliancecomponent.GetWarAlliance(b_Request.WarAllianceID);
            if (WarAllianceInfo == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2529);
                b_Reply(b_Response);
                return false;
            }

            var MemberInfo = WarAllianceInfo.DeleteMember(b_Request.GameUserID);
            if (MemberInfo == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2525);
                b_Reply(b_Response);
                return false;
            }
            DBMemberInfo dBMemberInfoData = new DBMemberInfo();
            dBMemberInfoData.MemberID = MemberInfo.MemberID;
            dBMemberInfoData.MemberName = MemberInfo.MemberName;
            dBMemberInfoData.MemberLevel = MemberInfo.MemberLevel;
            dBMemberInfoData.MemberPost = 0;
            dBMemberInfoData.DBWarAllianceID = b_Request.WarAllianceID;
            dBMemberInfoData.ExitTime = Help_TimeHelper.GetNowSecond();
            dBMemberInfoData.IsDisabled = 2;
            dBMemberInfoData.MemberClassType = MemberInfo.MemberClassType;
            int ActorId = (int)(b_Request.AppendData >> 16);
            await Waralliancecomponent.SetWarAllianceMemberDB(dBMemberInfoData, ActorId);

            b_Reply(b_Response);
            return true;

        }
    }
}