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
    public class G2M_UpdatePlayerWarAllianceHandler : AMHandler<G2M_UpdatePlayerWarAlliance>
    {
        protected override async Task<bool> Run(G2M_UpdatePlayerWarAlliance b_Request)
        {
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                return false;
            }
            var WarAllianceInfo = Waralliancecomponent.GetWarAlliance(b_Request.WarAllianceID);
            if (WarAllianceInfo == null)
            { 
                return false;
            }

            if (b_Request.Type == 1)
            {
                var MemebrInfo2 = WarAllianceInfo.GetMemberInfo(b_Request.GameUserID,1);//申请列表
                if (MemebrInfo2 == null)
                {
                    return false;
                }
                WarAllianceInfo.MemberList2.Remove(b_Request.GameUserID);

                return true;
            }
            var MemebrInfo = WarAllianceInfo.GetMemberInfo(b_Request.GameUserID);
            if (MemebrInfo == null)
            {
                return false;
            }
            if (MemebrInfo.MemberPost == 3)
            {
                WarAllianceInfo.WarAllianceLevel = b_Request.GameLevel / 10;
            }
            MemberInfo MemberInfoDate = new MemberInfo();
            //Log.Warning($"更新战盟积分前{b_Request.GameUserID}:{MemebrInfo.AllianceScore}");
            MemberInfoDate = MemebrInfo;
            MemberInfoDate.MemberLevel = b_Request.GameLevel;
            MemberInfoDate.MeberServerID = b_Request.GameServerID;
            MemberInfoDate.MeberState = b_Request.MeberState;
            MemberInfoDate.MemberName = b_Request.Name;
            MemberInfoDate.AllianceScore = b_Request.AllianceScore;
            WarAllianceInfo.SetMemberInfo(MemberInfoDate);
            Waralliancecomponent.UpMemberInfo(MemberInfoDate, OptionComponent.Options.ZoneId).Coroutine();
            return true;
        }
    }
}
