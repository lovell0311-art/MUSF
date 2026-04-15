
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using TencentCloud.Bri.V20190328.Models;


namespace ETHotfix
{
    [EventMethod(typeof(RushGradeActivity), EventSystemType.INIT)]
    public class RushGradeActivityEventOnInit : ITEventMethodOnInit<RushGradeActivity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(RushGradeActivity b_Component)
        {
            b_Component.OnInit();
        }
    }
    public static partial class RushGradeActivityComponentSystem
    {
        public static void OnInit(this RushGradeActivity b_Component)
        {

            var Json = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelUpRewards_RewardConfigJson>().JsonDic;
            var ItemCreateAttr = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelUpActivity_RewardPropsConfigJson>().JsonDic;
            if (Json != null)
            {
                b_Component.ActivitID = Json[1].ActivityID;
                foreach (var item in Json)
                {
                    RushGradeRewardItem structRewardItem = new RushGradeRewardItem();
                    structRewardItem.RewardID = item.Key;//奖励ID
                    structRewardItem.RewardLimit = item.Value.Limit;//领取限制
                    foreach (var RewardItemID in item.Value.ItemID)
                    {
                        StructItem structItem1 = new StructItem
                        {
                            ItemID = ItemCreateAttr[RewardItemID].ItemId,//奖励道具ID
                            ItemInfo = ItemCreateAttr[RewardItemID].ToItemCreateAttr()//奖励道具属性
                        };
                        structRewardItem.ItemList.Add(structItem1);//物品列表
                    }

                    b_Component.ItemList.Add(structRewardItem.RewardID, structRewardItem);//奖励列表
                }

            }
            var Json2 = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Activity_InfoConfigJson>().JsonDic;
            if (Json2 != null)
            {
                b_Component.Parent.GetCustomComponent<ActivitiesComponent>().CheckActivitTime(Json2[Json[1].ActivityID].OpenTime, Json2[Json[1].ActivityID].EndTime, Json[1].ActivityID);
            }
        }

        public static bool PlayerReceiveActivityRewards(this RushGradeActivity b_Component, int RewardId, Player PlayerInfo, out int ErrorID)
        {
            if (b_Component.ItemList.TryGetValue(RewardId, out RushGradeRewardItem Info) != false)
            {
                if (Info.RewardLimit <= PlayerInfo.GetCustomComponent<GamePlayer>().Data.Level)
                {
                    List<Item> ItemList = new List<Item>();
                    BackpackComponent mBackpackComponent = PlayerInfo.GetCustomComponent<BackpackComponent>();
                    // 锁格子用的，类似 lock_guard
                    using var backpackLockList = ItemsBoxStatus.LockList.Create();
                    foreach (var ItemInfo in Info.ItemList)
                    {
                        var RewardItem = ItemFactory.TryCreate(ItemInfo.ItemID, PlayerInfo.GameAreaId, ItemInfo.ItemInfo);

                        if (RewardItem != null)
                        {
                            int posX = 0, posY = 0;
                            if (mBackpackComponent.mItemBox.CheckStatus(RewardItem.ConfigData.X, RewardItem.ConfigData.Y, ref posX, ref posY))
                            {
                                ItemList.Add(RewardItem);
                                backpackLockList.Add(mBackpackComponent.mItemBox.LockGrid(RewardItem.ConfigData.X, RewardItem.ConfigData.Y, posX, posY));
                            }
                            else
                            {
                                ErrorID = 2404;
                                return false;
                            }
                        }
                    }
                    // 手动释放锁
                    backpackLockList.Dispose();
                    if (ItemList.Count >= 1)
                    {
                        foreach (var Reward in ItemList)
                        {
                            if (mBackpackComponent.AddItem(Reward, "活动领取") == false)
                                Log.PLog("Activit", $"活动{b_Component.ActivitID}奖励领取异常 Item{Reward.ConfigID}");
                        }
                    }
                    var Json = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelUpRewards_RewardConfigJson>().JsonDic;
                    if (Json[RewardId].MiracleCoin != 0 || Json[RewardId].GoldCoin != 0)
                    {
                        var mGamePlayer = PlayerInfo.GetCustomComponent<GamePlayer>();
                        PlayerInfo.GetCustomComponent<PlayerShopMallComponent>().SetMiracleCoin(Json[RewardId].MiracleCoin, $"活动{b_Component.ActivitID}领取");
                        mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, Json[RewardId].GoldCoin, $"活动{b_Component.ActivitID}领取");

                        G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                        mChangeValue_notice.GameUserId = mGamePlayer.InstanceId;
                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                        mBattleKVData.Key = (int)E_GameProperty.GoldCoin;
                        mBattleKVData.Value = mGamePlayer.Data.GoldCoin;
                        G2C_BattleKVData mBattleKVData1 = new G2C_BattleKVData();
                        mBattleKVData1.Key = (int)E_GameProperty.MiracleCoin;
                        mBattleKVData1.Value = mGamePlayer.Data.MiracleCoin;
                        mChangeValue_notice.Info.Add(mBattleKVData);
                        PlayerInfo.Send(mChangeValue_notice);

                        DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, PlayerInfo.GameAreaId);
                        var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(PlayerInfo.GameAreaId);
                        mWriteDataComponent.Save(PlayerInfo.GetCustomComponent<GamePlayer>().Data, dBProxy).Coroutine();
                        mWriteDataComponent.Save(PlayerInfo.GetCustomComponent<PlayerShopMallComponent>().dBPlayerShopMall, dBProxy).Coroutine();
                    }
                    ErrorID = 0;
                    return true;
                }
            }
            ErrorID = 2405;
            return false;
        }
    }
}
