
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 使用仓库扩展证书
    /// </summary>
    [ItemUseRule(typeof(Use310046))]
    public class Use310046 : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            var wareHouseComponent = b_Player.GetCustomComponent<WarehouseComponent>();
            int ErrorCode = wareHouseComponent.ExtendedWarehouse();
            if (ErrorCode != ErrorCodeHotfix.ERR_Success)
            {
                b_Response.Error = 99;
            }
            return;
        }
    }
}