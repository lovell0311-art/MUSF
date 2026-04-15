
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
    [ItemUseRule(typeof(Use310150))]
    public class Use310150 : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            var mWarAlliance = b_Player.GetCustomComponent<PlayerWarAllianceComponent>();
            if (mWarAlliance.WarAllianceID == 0)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            mWarAlliance.AllianceScore += b_Item.ConfigData.Value;
            mWarAlliance.UpDateWarAlliancePlayerInfo();
            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
            mGoldCoinData.Key = (int)E_GameProperty.AllianceScoreChange;
            mGoldCoinData.Value = mWarAlliance.AllianceScore;
            mChangeValueMessage.Info.Add(mGoldCoinData);
            mGamePlayer.Player.Send(mChangeValueMessage);
            return;
        }
    }
}