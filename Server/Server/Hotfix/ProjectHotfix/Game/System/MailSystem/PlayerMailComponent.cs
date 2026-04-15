using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork;




namespace ETHotfix
{
    public static partial class PlayerMailComponentSystem
    {
        /// <summary>
        /// 加载邮件
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="mAreaId"></param>
        /// <returns></returns>
        public async static Task<bool> PlayerLoadMail(this PlayerMailComponent b_Component, int mAreaId)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Component.Parent.AddCustomComponent<DataCacheManageComponent>();
            var MaildataCache = mDataCacheComponent.Get<DBMailData>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
            if (MaildataCache == null)
            {
                MaildataCache = await mDataCacheComponent.Add<DBMailData>(dBProxy2, p => p.GameUserID == b_Component.Parent.GameUserId && p.IsDisabled != 1);
            }
            //// 从数据库读取邮件中的物品到缓存
            //await mDataCacheComponent.Append<DBItemData>(dBProxy2, 
            //    p => p.InComponent == EItemInComponent.Mail &&
            //    p.GameUserId == b_Component.Parent.GameUserId &&
            //    p.IsDispose == 0);
            
            var MailList = MaildataCache.DataQuery(p => p.GameUserID == b_Component.Parent.GameUserId && p.IsDisabled != 1);
            bool Unread = false;
            if (b_Component.mailInfos.Count == 0)
            {
                long beforeone = Help_TimeHelper.GetNowSecond();
                foreach (var Info in MailList)
                {
                    if (Info.MailValidTime <= beforeone)
                    {
                        Info.IsDisabled = 1;
                        continue;
                    }
                    MailInfo mailInfo = new MailInfo();
                    mailInfo.MailId = Info.MaliID;
                    mailInfo.MailName = Info.MailName;

                    if (Info.MailAcceptanceTime != 0)
                        mailInfo.MailAcceptanceTime = Info.MailAcceptanceTime;
                    else if (Info.MailAcceptanceTime == 0)
                    {
                        Info.MailAcceptanceTime = beforeone;
                        mailInfo.MailAcceptanceTime = beforeone;
                    }

                    mailInfo.MailValidTime = Info.MailValidTime;
                    mailInfo.MailContent = Info.MailContent;
                    mailInfo.MailState = Info.MailState;
                    if(Info.MailState == 0) Unread =true;
                    mailInfo.ReceiveOrNot = Info.ReceiveOrNot;
                    mailInfo.MailEnclosure = Help_JsonSerializeHelper.DeSerialize<List<MailItem>>(Info.MailEnclosure);
                    b_Component.mailInfos.Add(mailInfo.MailId, mailInfo);
                }
            }
            if (Unread)
            {
                G2C_LoadMaillUnreadMessage g2C_LoadMaillUnreadMessage = new G2C_LoadMaillUnreadMessage();
                b_Component.Parent.Send(g2C_LoadMaillUnreadMessage);
            }
            long Tiem = b_Component.Parent.GetCustomComponent<GamePlayer>().Data.NewServerMailTime;
            long Create = b_Component.Parent.GetCustomComponent<GamePlayer>().Data.CreateTimeTick;
            var ServerMailList = await dBProxy2.Query<DBMailData>(p => p.GameUserID ==0 && p.MailAcceptanceTime > Tiem && p.MailAcceptanceTime> Create);
            if (ServerMailList.Count > 0)
            {
                //long Tiem = b_Component.Parent.GetCustomComponent<GamePlayer>().Data.NewServerMailTime;
                //long Create = b_Component.Parent.GetCustomComponent<GamePlayer>().Data.CreateTimeTick;
                DBMailData dBMailData = new DBMailData();
                foreach (var Info in ServerMailList)
                {
                    dBMailData = Info as DBMailData;
                    //if (Tiem < dBMailData.MailAcceptanceTime && Create < dBMailData.MailAcceptanceTime)
                    {
                        MailInfo mailinfo = new MailInfo();
                        mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mAreaId);
                        mailinfo.MailName = dBMailData.MailName;
                        mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                        mailinfo.MailContent = dBMailData.MailContent;
                        mailinfo.MailState = 0;
                        mailinfo.ReceiveOrNot = 0;
                        mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                        
                        mailinfo.MailEnclosure.AddRange(MongoHelper.FromJson<List<MailItem>>(dBMailData.MailEnclosure));
                        //await MailSystem.SendMail(b_Component.Parent.GameUserId, mailinfo, mAreaId);
                        
                        if (Tiem < dBMailData.MailAcceptanceTime)
                        {
                            b_Component.ServerMail.Add(mailinfo.MailId, mailinfo);
                            Tiem = dBMailData.MailAcceptanceTime;
                            b_Component.Parent.GetCustomComponent<GamePlayer>().Data.NewServerMailTime = dBMailData.MailAcceptanceTime;
                        }
                    }
                }
            }
            await dBProxy2.Save(b_Component.Parent.GetCustomComponent<GamePlayer>().Data);
            return true;
        }
        public async static Task SendServerMaill(this PlayerMailComponent b_Component, int mAreaId)
        {
            if (b_Component.ServerMail.Count > 0)
            {
                // 遍历引用时，不能使用await(中断过程中，字典添加移除数据，会造成遍历报错)。可以拷贝个新对象进行遍历
                var array = b_Component.ServerMail.Values.ToArray();
                foreach (var item in array)
                {
                    await MailSystem.SendMail(b_Component.Parent.GameUserId, item, mAreaId);
                }
                b_Component.ServerMail.Clear();
            }
        }
        public static bool ReadMail(this PlayerMailComponent b_Component, long MailID, int mAreaId)
        {
            var dBProxy2 = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);

            if (b_Component.mailInfos.TryGetValue(MailID, out MailInfo ItemInfo))
            {
                ItemInfo.MailState = 1;

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                DataCacheManageComponent mDataCacheComponent = b_Component.Parent.AddCustomComponent<DataCacheManageComponent>();
                var MaildataCache = mDataCacheComponent.Get<DBMailData>();
                var Mail = MaildataCache.DataQuery(p => p.MaliID == MailID);
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mAreaId);
                Mail[0].MailState = 1;
                mWriteDataComponent.Save(Mail[0], dBProxy).Coroutine();
                return true;
            }
            return false;
        }

        public static async Task<bool> LingQuDaoJu(this PlayerMailComponent b_Component, long MailID, int mAreaId)
        {
            if (b_Component == null && MailID == 0) return false;
            BackpackComponent mBackpackComponent = b_Component.mPlayer.GetCustomComponent<BackpackComponent>();
            if (mBackpackComponent == null) return false;

            var dBProxy2 = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);
            if (dBProxy2 == null) return false;

            if (b_Component.mailInfos.TryGetValue(MailID, out MailInfo ItemInfo))
            {
                if (ItemInfo.ReceiveOrNot == 1) return false;
                if(ItemInfo.MailValidTime <= Help_TimeHelper.GetNowSecond()) return false;

                int posX = 0;
                int posY = 0;
                bool IsOk = false;
                List<(Item item,int x,int y)> DropItemList = new List<(Item, int, int)>();
                // 锁格子用的，类似 lock_guard
                using var backpackLockList = ItemsBoxStatus.LockList.Create();
                foreach (var ItemList in ItemInfo.MailEnclosure)
                {
                    List<Item> mDropItem = null;
                    if (ItemList.ItemID != 0)
                    {
                        var mDBItem = await ItemFactory.CreateFormDB(ItemList.ItemID, ItemList.AreaId);
                        if (mDBItem == null)
                        {
                            Log.Warning($"mDBItem is NULL ItemID:{ItemList.ItemID}Config:{ItemList.ItemConfigID}");
                            continue;
                        }
                        mDropItem = new List<Item>() { mDBItem };
                        
                        //mDropItem = new List<Item>
                        //{
                           
                        //    await ItemFactory.CreateFormDB(ItemList.ItemID, ItemList.AreaId)
                        //};
                    }
                    else
                    {
                        mDropItem = ItemFactory.CreateMany(ItemList.ItemConfigID, b_Component.mPlayer.GameAreaId, ItemList.CreateAttr);
                    }
                    
                    foreach(Item dropItem in mDropItem)
                    {
                        if (!mBackpackComponent.mItemBox.CheckStatus(dropItem.ConfigData.X, dropItem.ConfigData.Y, ref posX, ref posY))
                        {
                            var itemList = DropItemList.ToArray();
                            foreach (var v in itemList)
                            {
                                v.item.Dispose();
                            }
                            dropItem.Dispose();
                            return false;
                        }
                        backpackLockList.Add(mBackpackComponent.mItemBox.LockGrid(dropItem.ConfigData.X, dropItem.ConfigData.Y, posX, posY));
                        DropItemList.Add((dropItem, posX, posY));
                    }
                }
                // 手动释放锁
                backpackLockList.Dispose();
                foreach (var v in DropItemList)
                {
                    {
                        if (mBackpackComponent.AddItem(v.item,v.x,v.y, "邮件获取") == false)
                        {
                            // 运行到这里，说明代码出问题了
                            Log.Error($"角色:{b_Component.mPlayer.GameUserId}道具ID:{v.item.ConfigID}数量:{v.item.GetProp(EItemValue.Quantity)} 领取失败 背包不足！！！");
                            return false;
                        }
                        else
                        {
                            IsOk = true;
                            Log.PLog("MailItem", $"角色:{b_Component.mPlayer.GameUserId}道具ID:{v.item.ConfigID}数量:{v.item.GetProp(EItemValue.Quantity)} 领取成功");
                        }
                    }
                }

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                DataCacheManageComponent mDataCacheComponent = b_Component.Parent.AddCustomComponent<DataCacheManageComponent>();
                var MaildataCache = mDataCacheComponent.Get<DBMailData>();
                var Mail = MaildataCache.DataQuery(p => p.MaliID == MailID);
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mAreaId);
                if (IsOk)
                {
                    ItemInfo.ReceiveOrNot = 1;
                    ItemInfo.MailState = 1;
                    Mail[0].ReceiveOrNot = 1;
                    Mail[0].MailState = 1;
                }
                mWriteDataComponent.Save(Mail[0], dBProxy).Coroutine();
            }

            return true;
        }

        public static bool DeleteMail(this PlayerMailComponent b_Component, long MailID, int mAreaId)
        {
            if (b_Component == null) return false;

            if (b_Component.mailInfos.TryGetValue(MailID, out MailInfo mailInfo))
            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                DataCacheManageComponent mDataCacheComponent = b_Component.Parent.AddCustomComponent<DataCacheManageComponent>();
                var MaildataCache = mDataCacheComponent.Get<DBMailData>();
                if (MaildataCache != null)
                {
                    var Mail = MaildataCache.DataQuery(p => p.MaliID == MailID);
                    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mAreaId);
                    Mail[0].IsDisabled = 1;
                    mWriteDataComponent.Save(Mail[0], dBProxy).Coroutine();
                }
                b_Component.mailInfos.Remove(MailID);
            }
            return true;
        }
    }
}