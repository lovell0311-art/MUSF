using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    [Timer(TimerType.CheckOnlineTime)]
    public class PlayerOnlineRewardTimer : ATimer<PlayerOnlineRewardComponent>
    {
        public override void Run(PlayerOnlineRewardComponent self)
        {
            if (self.Parent.OnlineStatus != EOnlineStatus.Online) return;   // 玩家正在进入游戏 或 正在下线
            //var Title = self.mPlayer.GetCustomComponent<PlayerTitle>();
            bool HaveTitle = false;//!Title.CheckTitle(60005);
            string DTime = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}";
            self.dBOnlineReward.OnlineTime++;
            if (self.Parent.GetCustomComponent<GamePlayer>().Data.Level < 30) return;
            if (self.dBOnlineReward.Time != DTime)
            {
                self.dBOnlineReward.Time = DTime;
                self.dBOnlineReward.OnlineTime = 1;
            }
            else
            {
                if (self.dBOnlineReward.OnlineTime == 60)//30
                {
                    self.SendOnlineReward(1, HaveTitle).Coroutine();
                }
                else if (self.dBOnlineReward.OnlineTime == 180)//60
                {
                    self.SendOnlineReward(2, HaveTitle).Coroutine();
                }
                else if (self.dBOnlineReward.OnlineTime == 300)//120
                {
                    self.SendOnlineReward(3, HaveTitle).Coroutine();
                }
                else if (self.dBOnlineReward.OnlineTime == 480)//240
                {
                    self.SendOnlineReward(4, HaveTitle).Coroutine();
                }
                else if (self.dBOnlineReward.OnlineTime == 720)//480
                {
                    self.SendOnlineReward(5, HaveTitle).Coroutine();
                }
                //else if (self.dBOnlineReward.OnlineTime == 720)//720
                //{
                //    self.SendOnlineReward(6, HaveTitle).Coroutine();
                //}
                //else if (self.dBOnlineReward.OnlineTime == 960)//960
                //{
                //    self.SendOnlineReward(7, HaveTitle).Coroutine();
                //}
                //else if (self.dBOnlineReward.OnlineTime == 1200)//1200
                //{
                //    self.SendOnlineReward(8, HaveTitle).Coroutine();
                //}
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, self.mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(self.mPlayer.GameAreaId);
            mWriteDataComponent.Save(self.dBOnlineReward, dBProxy2).Coroutine();
        }
    }
    public static class OnlineRewardSystem
    {
        public static async Task<bool> LoadOnlineReward(this PlayerOnlineRewardComponent self)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = self.mPlayer.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<DBOnlineReward>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, self.mPlayer.GameAreaId);
            if (mDataCache == null)
            {
                mDataCache = await mDataCacheComponent.Add<DBOnlineReward>(dBProxy2, p => p.GameUerId == self.mPlayer.GameUserId);
            }

            var ShopInfo = mDataCache.DataQuery(p => p.GameUerId == self.mPlayer.GameUserId);
            if (ShopInfo.Count >= 1)
            {
                self.dBOnlineReward = ShopInfo[0];
            }
            else
            {
                self.dBOnlineReward.Id = IdGeneraterNew.Instance.GenerateUnitId(self.mPlayer.GameAreaId);
                self.dBOnlineReward.GameUerId = self.mPlayer.GameUserId;
                self.dBOnlineReward.Time = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}";
                self.dBOnlineReward.OnlineTime = 1;
                await dBProxy2.Save(self.dBOnlineReward);
            }
            self.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000 * 60, TimerType.CheckOnlineTime, self);
            return true;
        }

        public static async Task SendOnlineReward(this PlayerOnlineRewardComponent self, int Type, bool HaveTitle)
        {
            var JsonGet = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var ItemInfo = JsonGet.GetJson<OnlineDuration_RewardConfigJson>().JsonDic;
            var ItemInfoList = JsonGet.GetJson<Activity_RewardPropsConfigJson>().JsonDic;
            if (ItemInfo.TryGetValue(Type, out var RewardConfigInfo))
            {
                MailInfo mailinfo = new MailInfo();
                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(self.mPlayer.GameAreaId);
                mailinfo.MailName = "累计在线奖励";
                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                mailinfo.MailContent = "恭喜玩家获得奖励道具";
                mailinfo.MailState = 0;
                mailinfo.ReceiveOrNot = 0;
                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                ItemCreateAttr itemCreateAttr1 = new ItemCreateAttr();
                ItemCreateAttr itemCreateAttr2 = new ItemCreateAttr();
                ItemCreateAttr itemCreateAttr3 = new ItemCreateAttr();

                //金币
                itemCreateAttr1.Quantity = 0;
                if (RewardConfigInfo.RewardCoins > 0)
                    itemCreateAttr1.Quantity = RewardConfigInfo.RewardCoins;

                if (HaveTitle)
                {
                    if (ItemInfo.TryGetValue(RewardConfigInfo.Title, out var Item1))
                        if (Item1.RewardCoins > 0)
                            itemCreateAttr1.Quantity += Item1.RewardCoins;
                }
                //U币
                itemCreateAttr2.Quantity = 0;
                if (RewardConfigInfo.RewardMiracleCoin > 0)
                    itemCreateAttr2.Quantity = RewardConfigInfo.RewardMiracleCoin;
                if (HaveTitle)
                {
                    if (ItemInfo.TryGetValue(RewardConfigInfo.Title, out var Item1))
                        if (Item1.RewardMiracleCoin > 0)
                            itemCreateAttr2.Quantity += Item1.RewardMiracleCoin;
                }
                //道具
                Dictionary<int, ItemCreateAttr> keyValuePairs = new Dictionary<int, ItemCreateAttr>();
                
                foreach (var Id in RewardConfigInfo.ItemID)
                {
                    itemCreateAttr3 = new ItemCreateAttr();
                    if (ItemInfoList.TryGetValue(Id,out var Info))
                    {
                        itemCreateAttr3.Quantity = Info.Quantity;
                        itemCreateAttr3.Level = Info.Level;
                        itemCreateAttr3.OptListId = Info.OptListId;
                        itemCreateAttr3.OptLevel = Info.OptLevel;
                        itemCreateAttr3.HaveSkill = Info.HasSkill == 1;
                        itemCreateAttr3.SetId = Info.SetId;
                        itemCreateAttr3.IsBind = Info.IsBind;
                        itemCreateAttr3.OptionExcellent = new List<int>(Info.OptionExcellent);
                        if(!string.IsNullOrEmpty(Info.CustomAttrMathod))
                            itemCreateAttr3.CustomAttrMethod.AddRange(Info.CustomAttrMathod.Split(","));

                        keyValuePairs.Add(Info.ItemId, itemCreateAttr3);
                    }
                    
                    //if (Id > 0)
                    //    itemCreateAttr3.Quantity = RewardConfigInfo.Number;
                    //itemCreateAttr3.IsBind = 1;
                    //if (HaveTitle)
                    //{
                    //    if (ItemInfo.TryGetValue(RewardConfigInfo.Title, out var Item1))
                    //        if (Item1.Number > 0)
                    //            itemCreateAttr3.Quantity += Item1.Number;
                    //}
                }
                //添加金币到邮件中
                if (itemCreateAttr1.Quantity > 0)
                {
                    MailItem mailItem1 = new MailItem
                    {
                        ItemConfigID = 320294,
                        ItemID = 0,
                        CreateAttr = itemCreateAttr1
                    };

                    mailinfo.MailEnclosure.Add(mailItem1);
                }
                //添加U币到邮件中
                if (itemCreateAttr2.Quantity > 0)
                {
                    MailItem mailItem2 = new MailItem
                    {
                        ItemConfigID = 320316,
                        ItemID = 0,
                        CreateAttr = itemCreateAttr2
                    };

                    mailinfo.MailEnclosure.Add(mailItem2);
                }
                //添加U币到邮件中
                if (keyValuePairs.Count > 0)
                {
                    foreach (var Item in keyValuePairs)
                    {
                        MailItem mailItem3 = new MailItem
                        {
                            ItemConfigID = Item.Key,
                            ItemID = 0,
                            CreateAttr = Item.Value
                        };

                        mailinfo.MailEnclosure.Add(mailItem3);
                    }
                }
                MailSystem.SendMail(self.mPlayer.GameUserId, mailinfo, self.mPlayer.GameAreaId).Coroutine();
            }
            else
                Log.Warning($"在线奖励没有找到对应的物品--类型:{Type}");
        }
    }
}
