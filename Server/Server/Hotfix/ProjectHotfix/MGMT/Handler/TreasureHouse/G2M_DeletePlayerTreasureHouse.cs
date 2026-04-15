using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_DeletePlayerTreasureHouseHandler : AMHandler<G2M_DeletePlayerTreasureHouse>
    {
        protected override async Task<bool> Run(G2M_DeletePlayerTreasureHouse b_Request)
        {
            var component = Root.MainFactory.GetCustomComponent<MGMTTreasureHouse>();
            if (component == null)
            {
                return false;
            }

            if (component.TemporaryPlayer.ContainsKey(b_Request.GameUserID))
                component.TemporaryPlayer.Remove(b_Request.GameUserID);

            return true;
        }
    }
}
