
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
    /// 解毒剂
    /// </summary>
    [ItemUseRule(typeof(Use310008))]
    public class Use310008 : C_ItemUseRule<Player, Item, IResponse>
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

            // 解除中毒状态

            var mCurrentTemp = b_Player.GetCustomComponent<GamePlayer>();

            if (mCurrentTemp.HealthStatsDic.ContainsKey(E_BattleSkillStats.Curse))
            {
                var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, b_Player.GameUserId);
                if (mMapComponent == null)
                {
                    b_Response.Error = 99;
                    b_Response.Message = "参数不对";
                    return;
                }
                var mBattleComponent = mMapComponent.GetCustomComponent<BattleComponent>();

                mCurrentTemp.RemoveHealthState(E_BattleSkillStats.Curse, mBattleComponent, true);
                mCurrentTemp.UpdateHealthState();
            }

            return;
        }
    }
}