using System.Collections.Generic;
using System.Linq;
using System;
using CustomFrameWork;

namespace ETModel.Robot
{
    public class RobotBackpackComponent : Entity
    {
        public const int I_PackageWidth = 8;
        public const int I_PackageHigh = 12;

        #region 状态
        /// <summary>
        /// 背包满了
        /// </summary>
        public bool IsFull = false;
        #endregion

        public ItemsBoxStatus ItemBox;
        /// <summary>
        /// Key:ItemUID
        /// </summary>
        protected Dictionary<long, RobotItem> ItemDict = new Dictionary<long, RobotItem>();
        /// <summary>
        /// Key:ItemConfigId
        /// </summary>
        protected Dictionary<int, Dictionary<long, RobotItem>> ConfigId2Item = new Dictionary<int, Dictionary<long, RobotItem>>();
        /// <summary>
        /// 根据不同类型，给装备分类
        /// </summary>
        protected Dictionary<EItemType, Dictionary<long, RobotItem>> ItemType2Item = new Dictionary<EItemType, Dictionary<long, RobotItem>>();

        public virtual void Init()
        {
            ItemBox = new ItemsBoxStatus();
            ItemBox.Init(RobotBackpackComponent.I_PackageWidth, RobotBackpackComponent.I_PackageWidth * RobotBackpackComponent.I_PackageHigh);
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            base.Dispose();

            foreach(RobotItem item in ItemDict.Values.ToArray())
            {
                item.Dispose();
            }
            ItemDict.Clear();
            ConfigId2Item.Clear();
            ItemType2Item.Clear();
        }

        public bool AddItem(RobotItem item, int posX, int posY)
        {
            if (ItemDict.ContainsKey(item.Id))
            {
                Log.Warning($"物品重复进入组件,uid={item.Id}");
                return false;
            }
            if (!ItemBox.AddItem(item.Config.X,item.Config.Y,posX,posY))
            {
                Log.Warning($"坐标重叠,uid={item.Id},pos=({posX},{posY})");
                return false;
            }
            ItemDict[item.Id] = item;

            Dictionary<long, RobotItem> dict = null;
            if (!ConfigId2Item.TryGetValue(item.Config.Id,out dict))
            {
                dict = new Dictionary<long, RobotItem>();
                ConfigId2Item[item.Config.Id] = dict;
            }
            dict.Add(item.Id, item);

            Dictionary<long, RobotItem> dict2 = null;
            if (!ItemType2Item.TryGetValue(item.Type, out dict2))
            {
                dict2 = new Dictionary<long, RobotItem>();
                ItemType2Item[item.Type] = dict2;
            }
            dict2.Add(item.Id, item);

            item.PosX = posX;
            item.PosY = posY;
            return true;
        }

        public RobotItem RemoveItem(long uid)
        {
            if (!ItemDict.TryGetValue(uid,out RobotItem item))
            {
                Log.Warning($"没找到要离开组件的物品,uid={uid}");
                return null;
            }
            ItemBox.RemoveItem(item.Config.X, item.Config.Y, item.PosX, item.PosY);
            ConfigId2Item[item.Config.Id].Remove(uid);
            ItemType2Item[item.Type].Remove(uid);
            ItemDict.Remove(uid);
            return item;
        }

        public void DeleteItem(long uid)
        {
            RobotItem item = RemoveItem(uid);
            item?.Dispose();
        }

        public RobotItem GetItemByUid(long uid)
        {
            RobotItem item = null;
            ItemDict.TryGetValue(uid, out item);
            return item;
        }

        public List<RobotItem> GetItemsByConfigId(int configId)
        {
            if(ConfigId2Item.TryGetValue(configId,out var dict))
            {
                return dict.Values.ToList();
            }
            return new List<RobotItem>();
        }

        public RobotItem[] GetAll()
        {
            return ItemDict.Values.ToArray();
        }

        public List<RobotItem> WhereFromItemType(HashSet<EItemType> typeList,Func<RobotItem,bool> predicate)
        {
            List<RobotItem> items = new List<RobotItem>();
            foreach(var kv in ItemType2Item)
            {
                if (!typeList.Contains(kv.Key)) continue;
                items.AddRange(kv.Value.Values.Where(predicate));
            }
            return items;
        }

        public List<RobotItem> WhereFromItemType(EItemType type, Func<RobotItem, bool> predicate)
        {
            List<RobotItem> items = new List<RobotItem>();
            if(ItemType2Item.TryGetValue(type, out var dict))
            {
                items.AddRange(dict.Values.Where(predicate));

            }
            return items;
        }
    }
}
