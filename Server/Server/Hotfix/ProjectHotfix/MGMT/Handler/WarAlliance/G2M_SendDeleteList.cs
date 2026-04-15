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
    public class G2M_SendDeleteListHandler : AMHandler<G2M_SendDeleteList>
    {
        protected override async Task<bool> Run(G2M_SendDeleteList b_Request)
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

            WarAllianceInfo.MemberList2.Remove(b_Request.GameUserID);
            return true;
        }
    }
}
