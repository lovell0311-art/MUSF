using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    public static class ItemFactory
    {
        /// <summary>
        /// 生成新物品，只用来创建白装，没有任何属性
        /// </summary>
        /// <param name="configData">物品配置</param>
        /// <param name="zone">区ID</param>
        /// <param name="quantity">物品数量</param>
        /// <returns></returns>
        public static Item Create(ItemConfig configData,int gameAreaId,int quantity = 1)
        {
            Item item = Root.CreateBuilder.GetInstance<Item>(); //ComponentFactory.create<Item>(IdGenerater.GenerateId());
            item.Awake(null);
            item.ConfigData = configData;
            item.NotSevedToDB = true;
            DBItemData data = new DBItemData();
            data.GameUserId = -1;   //需要在外面指定所属Player对象
            data.Id = IdGeneraterNew.Instance.GenerateUnitId(gameAreaId);
            data.ConfigID = configData.Id;
            data.posX = -1;
            data.posY = -1;
            data.CreateTime = DateTime.UtcNow;
            data.CreateTimeTick = CustomFrameWork.TimeHelper.ClientNowSeconds();
            data.GameAreaId = gameAreaId;
            item.Init(data);

            item.SetProp(EItemValue.Level, 0);
            item.SetProp(EItemValue.Quantity, quantity);
            
            item.OnlyUpdateDurability();
            item.SetProp(EItemValue.Durability, item.GetProp(EItemValue.DurabilityMax));

            item.UpdateProp();
            return item;
        }

        /// <summary>
        /// 创建新物品，只用来创建白装，没有任何属性
        /// </summary>
        /// <param name="configId">配置表中的物品id</param>
        /// <param name="zone">区Id</param>
        /// <param name="quantity">物品数量</param>
        /// <returns></returns>
        /// <exception cref="ItemConfigNotExistException">物品配置不存在时，会抛出此异常</exception>
        public static Item Create(int configId,int gameAreaId, int quantity = 1)
        {
            ItemConfig conf = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>().Get(configId);
            if(conf == null)
            {
                throw new ItemConfigNotExistException($"创建的物品配置不存在。 configId={configId}");
            }
            return Create(conf, gameAreaId, quantity);
        }

        /// <summary>
        /// 创建物品，可以通过 ItemCreateAttr 来创建特定属性的物品
        /// </summary>
        /// <param name="configData">物品配置</param>
        /// <param name="gameAreaId">区ID</param>
        /// <param name="itemCreateAttr">创建的物品属性信息</param>
        /// <returns></returns>
        /// <exception cref="ItemNotSupportAttrException">物品不支持指定属性时，会抛出此异常</exception>
        public static Item Create(ItemConfig configData, int gameAreaId,ItemCreateAttr itemCreateAttr)
        {
            var item = Create(configData, gameAreaId, itemCreateAttr.Quantity);
            item.SetProp(EItemValue.Level, itemCreateAttr.Level);
            // 追加
            if(configData.AppendAttrId.Count > itemCreateAttr.OptListId)
            {
                item.SetProp(EItemValue.OptValue, configData.AppendAttrId[itemCreateAttr.OptListId]);
                item.SetProp(EItemValue.OptLevel, itemCreateAttr.OptLevel);
            }
            // 技能
            if (itemCreateAttr.HaveSkill)
            {
                item.SetProp(EItemValue.SkillId, configData.Skill);
            }
            // 幸运
            if (itemCreateAttr.HaveLucky)
            {
                item.SetProp(EItemValue.LuckyEquip, 1);
            }
            // 套装
            if (itemCreateAttr.SetId > 0)
            {
                item.SetProp(EItemValue.SetId, itemCreateAttr.SetId);
                item.AddExtraEntry();   // 套装必出属性
            }
            // 是绑定的物品
            if (itemCreateAttr.IsBind != 0)
            {
                item.SetProp(EItemValue.IsBind, itemCreateAttr.IsBind);
            }
            // 是任务物品
            if (itemCreateAttr.IsTask)
            {
                item.SetProp(EItemValue.IsTask, 1);
            }
            // 过期时间戳
            if(itemCreateAttr.ExpireTimestamp > 0)
            {
                itemCreateAttr.ValidTime = (itemCreateAttr.ExpireTimestamp - (int)Help_TimeHelper.GetNowSecond());
            }
            // 过期时间
            if(itemCreateAttr.ValidTime > 0)
            {
                item.SetProp(EItemValue.ValidTime, (int)(Help_TimeHelper.GetNowSecond() + itemCreateAttr.ValidTime));
            }
            // 荧光宝石属性
            if (itemCreateAttr.FluoreAttr > 0)
            {
                item.SetProp(EItemValue.FluoreAttr, itemCreateAttr.FluoreAttr);
            }
            // 镶嵌孔洞
            if(itemCreateAttr.FluoreSlotCount > 0)
            {
                item.SetProp(EItemValue.FluoreSlotCount, itemCreateAttr.FluoreSlotCount);
                for(int i =0;i<itemCreateAttr.FluoreSlot.Count;++i)
                {
                    if(Enum.TryParse("FluoreSlot" + (i+1),out EItemValue slotEnum))
                    {
                        item.SetProp(slotEnum, itemCreateAttr.FluoreSlot[i]);
                    }
                }
            }
            // 卓越
            var itemAttrEntryManager = Root.MainFactory.GetCustomComponent<ItemAttrEntryManager>();
            foreach (var attrId in itemCreateAttr.OptionExcellent)
            {
                var entry = itemAttrEntryManager.GetOrCreate(attrId, 0);
                if (entry.Type == EItemAttrEntryType.Excellent)
                {
                    item.data.ExcellentEntry.Add(attrId);
                }
            }
            // 特殊-翅膀
            foreach (var kv in itemCreateAttr.OptionSpecial)
            {
                var entry = itemAttrEntryManager.GetOrCreate(kv.Key, kv.Value);
                if (entry.Type == EItemAttrEntryType.Special)
                {
                    item.data.SpecialEntry.TryAdd(kv.Key, kv.Value);
                }
            }
            // 自定义属性方法
            var methodCreateBuilder = Root.MainFactory.GetCustomComponent<ItemCustomAttrMethodCreateBuilder>();
            foreach(var customAttr in itemCreateAttr.CustomAttrMethod)
            {
                if (string.IsNullOrEmpty(customAttr)) continue;
                if(methodCreateBuilder.MethodDict.TryGetValue(customAttr,out var handler))
                {
                    try
                    {
                        handler.Run(item);
                    }
                    catch(ItemNotSupportAttrException e)
                    {
                        item.Dispose();
                        throw e;
                    }
                }
                else
                {
                    throw new ItemNotSupportAttrException($"找不到 CustomAttrMethod='{customAttr}'");
                }
            }

            item.OnlyUpdateDurability();
            item.SetProp(EItemValue.Durability, item.GetProp(EItemValue.DurabilityMax));
            item.UpdateProp();
            return item;
        }

        /// <summary>
        /// 创建物品，可以通过 ItemCreateAttr 来创建特定属性的物品
        /// </summary>
        /// <param name="configId">配置表中的物品id</param>
        /// <param name="gameAreaId">区ID</param>
        /// <param name="itemCreateAttr">创建的物品属性信息</param>
        /// <returns></returns>
        /// <exception cref="ItemNotSupportAttrException">物品不支持指定属性时，会抛出此异常</exception>
        /// <exception cref="ItemConfigNotExistException">物品配置不存在时，会抛出此异常</exception>
        public static Item Create(int configId, int gameAreaId, ItemCreateAttr itemCreateAttr)
        {
            ItemConfig conf = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>().Get(configId);
            if (conf == null)
            {
                throw new ItemConfigNotExistException($"创建的物品配置不存在。 configId={configId}");
            }
            return Create(conf, gameAreaId, itemCreateAttr);
        }


        public static List<Item> CreateMany(ItemConfig configData, int gameAreaId, ItemCreateAttr itemCreateAttr)
        {
            List<Item> items = new List<Item>();
            if (configData.StackSize <= 0 || itemCreateAttr.Quantity <= configData.StackSize)
            {
                items.Add(Create(configData, gameAreaId, itemCreateAttr));
                return items;
            }
            int count = itemCreateAttr.Quantity;
            ItemCreateAttr attr = itemCreateAttr.Clone();
            while (count > 0)
            {
                if(count >= configData.StackSize)
                {
                    attr.Quantity = configData.StackSize;
                    items.Add(Create(configData, gameAreaId, attr));
                }
                else
                {
                    attr.Quantity = count;
                    items.Add(Create(configData, gameAreaId, attr));
                }
                count -= configData.StackSize;
            }
            return items;
        }

        public static List<Item> CreateMany(int configId, int gameAreaId, ItemCreateAttr itemCreateAttr)
        {
            ItemConfig conf = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>().Get(configId);
            if (conf == null)
            {
                throw new ItemConfigNotExistException($"创建的物品配置不存在。 configId={configId}");
            }
            return CreateMany(conf, gameAreaId, itemCreateAttr);
        }

        /// <summary>
        /// 创建新物品，只用来创建白装，没有任何属性
        /// </summary>
        /// <param name="configId">配置表中的物品id</param>
        /// <param name="zone">区Id</param>
        /// <param name="quantity">物品数量</param>
        /// <returns></returns>
        public static Item TryCreate(int configId, int gameAreaId, int quantity = 1, bool printWarn = true)
        {
            try
            {
                return Create(configId, gameAreaId, quantity);
            }
            catch(ItemConfigNotExistException e)
            {
                if (printWarn) Log.Warning(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// 创建物品，可以通过 ItemCreateAttr 来创建特定属性的物品（物品不支持指定属性时，不会抛异常）
        /// </summary>
        /// <param name="configData"></param>
        /// <param name="gameAreaId"></param>
        /// <param name="itemCreateAttr"></param>
        /// <returns></returns>
        public static Item TryCreate(ItemConfig configData, int gameAreaId, ItemCreateAttr itemCreateAttr, bool printWarn = true)
        {
            try
            {
                return Create(configData, gameAreaId, itemCreateAttr);
            }
            catch (ItemNotSupportAttrException e)
            {
                if (printWarn) Log.Warning(e.ToString());
            }
            return null;
        }

        /// <summary>
        /// 创建物品，可以通过 ItemCreateAttr 来创建特定属性的物品（物品不支持指定属性时，不会抛异常）
        /// </summary>
        /// <param name="configId">配置表中的物品id</param>
        /// <param name="gameAreaId">区ID</param>
        /// <param name="itemCreateAttr">创建的物品属性信息</param>
        /// <returns></returns>
        public static Item TryCreate(int configId, int gameAreaId, ItemCreateAttr itemCreateAttr,bool printWarn = true)
        {
            try
            {
                return Create(configId, gameAreaId, itemCreateAttr);
            }
            catch (ItemConfigNotExistException e)
            {
                if (printWarn) Log.Warning(e.ToString());
            }
            catch(ItemNotSupportAttrException e)
            {
                if (printWarn) Log.Warning(e.ToString());
            }
            return null;
        }



        public static async Task<Item> CreateFormDB(long itemUID, int AreaId)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, AreaId);

            var info = await dBProxy2.Query<DBItemData>(p => p.Id == itemUID && p.IsDispose == 0);

            DBItemData curDB = null;
            if (info != null && info.Count == 1)
                curDB = info[0] as DBItemData;

            if (curDB == null) return null;

            Item item = TryCreate(curDB.ConfigID, AreaId);
            if (item != null)
            {
                item.NotSevedToDB = false;
                item.Init(curDB);
                item.UpdateProp();
            }
            else
            {
                Log.Error($"藏宝阁购买物品邮件领取 itemUID={itemUID}");
            }
            return item;
        }
        /// <summary>
        /// 从数据库里查找对应物品并生成Item
        /// </summary>
        /// <param name="itemUID"></param>
        /// <returns></returns>
        public static Item CreateFormDB(long itemUID,Player player)
        {
            var curDB = GetData(itemUID, player);
            if (curDB != null)
            {
                Item item = TryCreate(curDB.ConfigID, player.GameAreaId);
                if(item != null)
                {
                    item.NotSevedToDB = false;
                    item.Init(curDB);
                    item.UpdateProp();
                }
                else
                {
                    Log.Error($"'ConfigId' in 'curDB' does not exist. itemUID={itemUID} player.UserId={player.UserId} player.GameUserId={player.GameUserId}");
                }
                return item;
            }
            else
            {
                Log.Error($"curDB is null. itemUID={itemUID} player.UserId={player.UserId} player.GameUserId={player.GameUserId}");
            }
            return null;
        }

        public static DBItemData GetData(long itemUID,Player player)
        {
            DataCacheManageComponent mDataCacheManageComponent = player.AddCustomComponent<DataCacheManageComponent>();
            var dataCache = mDataCacheManageComponent.Get<DBItemData>();
            if (dataCache != null)
            {
                var itemData = dataCache.DataQueryById(itemUID);
                if (itemData != null && itemData.IsDispose == 0)
                {
                    return itemData;
                }
            }
            return null;
        }

        public static bool DleData(long itemUID, Player player)
        {
            DataCacheManageComponent mDataCacheManageComponent = player.AddCustomComponent<DataCacheManageComponent>();
            var dataCache = mDataCacheManageComponent.Get<DBItemData>();
            if (dataCache != null)
            {
                var itemData = dataCache.DataQueryById(itemUID);
                if (itemData != null )
                {
                    dataCache.DataRemove(itemUID);
                    return true;
                }
            }
            return false;
        }
    }
}
