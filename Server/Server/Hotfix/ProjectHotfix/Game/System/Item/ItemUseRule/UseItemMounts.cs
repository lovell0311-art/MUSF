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
    /// 使用血瓶
    /// </summary>
    [ItemUseRule(typeof(UseItemMounts))]
    public class UseItemMounts : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            if(!b_Item.CanUse(b_Player.GetCustomComponent<GamePlayer>()))
            {
                b_Response.Error = 2302;
                return;
            }

            if(b_Item.Type != EItemType.Mounts)
            {
                // 配置错误，物品不是坐骑
                b_Response.Error = 2300;
                return;
            }

            var bak = b_Player.GetCustomComponent<BackpackComponent>();
            if (bak.AddMount(b_Item.ItemUID, out var ErrorId))
            {
                b_Response.Error = ErrorId;
                return;
            }
            else
            {
                b_Response.Error = ErrorId;
                return;
            }
            
        }
    }
}