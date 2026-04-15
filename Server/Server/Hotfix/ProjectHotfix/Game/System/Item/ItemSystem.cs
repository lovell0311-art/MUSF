using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ETHotfix
{
    /// <summary>
    /// 物品组件
    /// </summary>
    public static partial class ItemSystem
    {

        public static void Init(this Item self, DBItemData data)
        {
            self.data = data;
            self.ConfigID = data.ConfigID;
            self.Type = (EItemType)(data.ConfigID / 10000);
            self.ItemUID = data.Id;
            self.__Property.Clear();
            foreach (var kv in data.PropertyData)
            {
                self.__Property.Add((EItemValue)kv.Key, kv.Value);
            }
        }


        public static void SaveDB(this Item item, Player player)
        {
            item.SaveDBAsync(player).Coroutine();
        }

        public static async Task SaveDBAsync(this Item item, Player player)
        {
            item.NotSevedToDB = false;
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, item.data.GameAreaId);
            if (ItemFactory.GetData(item.ItemUID, player) != null)
            {
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(item.data.GameAreaId);
                await mWriteDataComponent.Save(item.data, dBProxy);
            }
            else
            {
                player.AddCustomComponent<DataCacheManageComponent>().Get<DBItemData>().DataAdd(item.data);
                await dBProxy.Save(item.data);
            }
        }

        /// <summary>
        /// 现在，立刻保存
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static async Task SaveDBNow(this Item item)
        {
            try
            {
                item.NotSevedToDB = false;
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, item.data.GameAreaId);
                if ((await dBProxy.Save(item.data)) == false)
                {
                    Log.Warning($"保存物品失败！item={item.data.ToJson()}");
                }
            }
            catch(Exception e)
            {
                string debugJson = "";
                try
                {
                    debugJson = item.data.ToJson();
                }
                catch (Exception e2)
                {
                    Log.Error(e2);
                }
                Log.Error(debugJson,e);
            }
        }



        /// <summary>
        /// 只对数据进行保存，不会主动检查DataCache中有没有这个物品。效率比 SaveDB 高，只对变动频繁且不重要的数据进行保存标记。
        /// </summary>
        /// <param name="item"></param>
        public static void OnlySaveDB(this Item item)
        {
            item.NotSevedToDB = false;
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, item.data.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(item.data.GameAreaId);
            mWriteDataComponent.Save(item.data, dBProxy).Coroutine();
        }

        /// <summary>
        /// 从数据库里标记该Item实体已删除
        /// 用于在地面上的物品销毁时调用，或者销毁时需要标记数据库已销毁时调用
        /// 最后会销毁Item实体
        /// </summary>
        /// <param name="item"></param>
        /// <param name="AreaId"></param>
        /// <param name="log">销毁原因</param>
        public static void DisposeDB(this Item item, string log)
        {
            var curData = item.data;
            if (curData != null)
            {
                curData.IsDispose = CustomFrameWork.TimeHelper.ClientNowSeconds();
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, curData.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(curData.GameAreaId);
                mWriteDataComponent.Save(curData, dBProxy).Coroutine();
                Log.PLog("Item",$"销毁物品({log}) {item.ToLogString()}");
            }
            item.Dispose();
        }

        /// <summary>
        /// 删除物品,当物品在数据库时，会从数据库中删除
        /// </summary>
        /// <param name="item"></param>
        /// <param name="log"></param>
        public static void Delete(this Item item,string log)
        {
            if(item.NotSevedToDB)
            {
                item.Dispose();
            }
            else
            {
                item.DisposeDB(log);
            }
        }


        public static void SendAllPropertyData(this Item item, Player player, ItemPropertyNotice b_Scene = ItemPropertyNotice.None)
        {
            var notice = new G2C_ItemsPropChange_notice();
            foreach (var propItem in item.__Property)
            {
                notice.PropList.Add(new Struct_Property()
                {
                    PropID = (int)propItem.Key,
                    Value = (int)propItem.Value
                });
            }
            notice.ItemUUID = item.ItemUID;
            notice.GameUserId = item.data.GameUserId;
            notice.Scene = (int)b_Scene;
            player.Send(notice);
        }

        /// <summary>
        /// 发送所有的词条属性
        /// </summary>
        /// <param name="item"></param>
        /// <param name="player">目标玩家</param>
        public static void SendAllEntryAttr(this Item item, Player player, ItemPropertyNotice b_Scene = ItemPropertyNotice.None)
        {
            Item.G2CItemAttrEntryChangeNotice.ExecllentEntry.Clear();
            Item.G2CItemAttrEntryChangeNotice.ExtraEntry.Clear();
            Item.G2CItemAttrEntryChangeNotice.SpecialEntry.Clear();
            Item.G2CItemAttrEntryChangeNotice.ItemUUID = item.ItemUID;
            Item.G2CItemAttrEntryChangeNotice.GameUserId = item.data.GameUserId;
            // 卓越属性
            foreach (var entryId in item.data.ExcellentEntry)
            {
                Struct_AttrEntry pbAttrEntry = new Struct_AttrEntry();
                pbAttrEntry.PropId = entryId;
                pbAttrEntry.Level = 0;
                Item.G2CItemAttrEntryChangeNotice.ExecllentEntry.Add(pbAttrEntry);
            }
            // 额外属性
            foreach (var kv in item.data.ExtraEntry)
            {
                Struct_AttrEntry pbAttrEntry = new Struct_AttrEntry();
                pbAttrEntry.PropId = kv.Key;
                pbAttrEntry.Level = kv.Value;
                Item.G2CItemAttrEntryChangeNotice.ExtraEntry.Add(pbAttrEntry);
            }
            // 特殊属性
            foreach (var kv in item.data.SpecialEntry)
            {
                Struct_AttrEntry pbAttrEntry = new Struct_AttrEntry();
                pbAttrEntry.PropId = kv.Key;
                pbAttrEntry.Level = kv.Value;
                Item.G2CItemAttrEntryChangeNotice.SpecialEntry.Add(pbAttrEntry);
            }
            Item.G2CItemAttrEntryChangeNotice.Scene = (int)b_Scene;
            player.Send(Item.G2CItemAttrEntryChangeNotice);
        }

        public static Struct_ItemAllProperty ToItemAllProperty(this Item self)
        {
            if (self.StructItemAllProperty == null)
            {
                self.StructItemAllProperty = new Struct_ItemAllProperty();
            }
            self.StructItemAllProperty.PropList.Clear();
            self.StructItemAllProperty.ExecllentEntry.Clear();
            self.StructItemAllProperty.ExtraEntry.Clear();
            self.StructItemAllProperty.SpecialEntry.Clear();
            self.StructItemAllProperty.ItemUUID = self.ItemUID;
            self.StructItemAllProperty.ConfigId = self.ConfigID;
            foreach (var propItem in self.__Property)
            {
                self.StructItemAllProperty.PropList.Add(new Struct_Property()
                {
                    PropID = (int)propItem.Key,
                    Value = (int)propItem.Value
                });
            }
            // 卓越属性
            foreach (var entryId in self.data.ExcellentEntry)
            {
                Struct_AttrEntry pbAttrEntry = new Struct_AttrEntry();
                pbAttrEntry.PropId = entryId;
                pbAttrEntry.Level = 0;
                self.StructItemAllProperty.ExecllentEntry.Add(pbAttrEntry);
            }
            // 额外属性
            foreach (var kv in self.data.ExtraEntry)
            {
                Struct_AttrEntry pbAttrEntry = new Struct_AttrEntry();
                pbAttrEntry.PropId = kv.Key;
                pbAttrEntry.Level = kv.Value;
                self.StructItemAllProperty.ExtraEntry.Add(pbAttrEntry);
            }
            // 特殊属性
            foreach (var kv in self.data.SpecialEntry)
            {
                Struct_AttrEntry pbAttrEntry = new Struct_AttrEntry();
                pbAttrEntry.PropId = kv.Key;
                pbAttrEntry.Level = kv.Value;
                self.StructItemAllProperty.SpecialEntry.Add(pbAttrEntry);
            }
            return self.StructItemAllProperty;
        }

        public static MailItem ToMailItem(this Item self)
        {
            return self.data.ToMailItem();
        }

        public static string DebugString(this Item self)
        {
            string ret = $"{self.ConfigData.Name}\n";
            ret += $"├─Uid:{self.ItemUID}\n";
            ret += $"├─Id:{self.ConfigID}\n";
            ret += $"├─Wight,Height:{self.ConfigData.X},{self.ConfigData.Y}\n";
            ret += $"└─属性:\n";
            foreach (var kv in self.__Property)
            {
                if (kv.Key != self.__Property.Last().Key)
                {
                    ret += $"  ├─{kv.Key}:{kv.Value}\n";
                }
                else
                {
                    ret += $"  └─{kv.Key}:{kv.Value}\n";
                }
            }
            ret += $"└─卓越属性:\n";

            return ret;
        }

        public static string ToLogString(this Item self)
        {
            if(self.GetProp(EItemValue.Level) > 0)
            {
                return $"uid={self.ItemUID},id={self.ConfigID},name={self.ConfigData.Name},lv={self.GetProp(EItemValue.Level)}";
            }
            else
            {
                return $"uid={self.ItemUID},id={self.ConfigID},name={self.ConfigData.Name}";
            }
        }


        #region 检测物品性质方法
        /// <summary>
        /// 物品是否是装备
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsEquipment(this Item self)
        {
            return self.ConfigData.Slot > 0;
        }

        /// <summary>
        /// 物品强化就等级是否足够
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsEnoughLevel(this Item self, int itemLevel)
        {
            return self.GetProp(EItemValue.Level) >= itemLevel;
        }

        /// <summary>
        /// 是否拥有卓越属性
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsExcellentEquip(this Item self)
        {
            return self.data.ExcellentEntry.Count > 0;
        }

        /// <summary>
        /// 是否拥有套装属性
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsSetEquip(this Item self)
        {
            return self.GetProp(EItemValue.SetId) > 0;
        }
        #endregion
    }
}
