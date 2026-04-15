using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using ETModel;

namespace ETHotfix
{
    [BackpackRouter(320316)]    // 奇迹币
    public class BackpackRouter_AddMiracleCoin : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            GamePlayer gamePlayer = backpack.mPlayer.GetCustomComponent<GamePlayer>();

            int count = item.GetProp(EItemValue.Quantity);
            gamePlayer.UpdateCoin(E_GameProperty.MiracleCoin, count, log);

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, backpack.mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(backpack.mPlayer.GameAreaId);
            mWriteDataComponent.Save(gamePlayer.Data, dBProxy).Coroutine();

            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            G2C_BattleKVData mMiracleCoinData = new G2C_BattleKVData();
            mMiracleCoinData.Key = (int)E_GameProperty.MiracleChange;
            mMiracleCoinData.Value = count;

            mMiracleCoinData = new G2C_BattleKVData();
            mMiracleCoinData.Key = (int)E_GameProperty.MiracleCoin;
            mMiracleCoinData.Value = gamePlayer.Data.MiracleCoin;

            mChangeValueMessage.Info.Add(mMiracleCoinData);

            backpack.mPlayer.Send(mChangeValueMessage);

            item.Delete($"进入背包({log})");
        }
    }
}
