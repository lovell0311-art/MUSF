
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
    /// 经验药水
    /// </summary>
    [ItemUseRule(typeof(Use310035_36_37_38_39_40_41_42_43_44))]
    public class Use310035_36_37_38_39_40_41_42_43_44 : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();
            int addPer = b_Item.ConfigData.Value;

            var mUseResult = mGamePlayer.AddExprience(addPer);
            if (mUseResult == false)
            {
                b_Response.Error = 778;
            }
            return;
        }
    }
}