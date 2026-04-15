using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using ETModel;

namespace ETHotfix
{
    [BackpackRouter(320294)]    // 金币
    public class BackpackRouter_AddGoldCoin : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            GamePlayer gamePlayer = backpack.mPlayer.GetCustomComponent<GamePlayer>();

            int count = item.GetProp(EItemValue.Quantity);
            gamePlayer.UpdateCoin(E_GameProperty.GoldCoin, count, log);

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, backpack.mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(backpack.mPlayer.GameAreaId);
            mWriteDataComponent.Save(gamePlayer.Data, dBProxy).Coroutine();

            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            G2C_BattleKVData mGoldCoinData = new G2C_BattleKVData();
            mGoldCoinData.Key = (int)E_GameProperty.GoldCoinChange;
            mGoldCoinData.Value = count;

            mGoldCoinData = new G2C_BattleKVData();
            mGoldCoinData.Key = (int)E_GameProperty.GoldCoin;
            mGoldCoinData.Value = gamePlayer.Data.GoldCoin;

            mChangeValueMessage.Info.Add(mGoldCoinData);

            backpack.mPlayer.Send(mChangeValueMessage);

            item.Delete($"进入背包({log})");

            // 发布 ItemCountChangeInBackpack 事件
            ETModel.EventType.ChecJinBiChangeInBackpack.Instance.player = backpack.mPlayer;
            Root.EventSystem.OnRun("ChecJinBiChangeInBackpack", ETModel.EventType.ChecJinBiChangeInBackpack.Instance);
        }
    }
}
