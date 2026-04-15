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
    [MessageHandler(AppType.LoginCenter)]
    public class G2M_PromotionLevelHandler : AMHandler<G2M_PromotionLevel>
    {
        protected override async Task<bool> Run(G2M_PromotionLevel b_Request)
        {
            var component = Root.MainFactory.GetCustomComponent<PromotionComponent>();
            if (component == null)
            {
                return false;
            }

            await component.UPNumberOfGuests(b_Request.Level, b_Request.Code, b_Request.UserID);
            return true;
        }
    }
}
