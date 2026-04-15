using CustomFrameWork.Baseic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ETModel
{
    namespace EventType
    {
        /// <summary>
        /// 背包添加物品
        /// </summary>
        public class BackpackAddItem
        {
            public static readonly BackpackAddItem Instance = new BackpackAddItem();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 添加的物品
            /// </summary>
            public Item item;
            public int posX;
            public int posY;
        }

        /// <summary>
        /// 背包移出物品
        /// </summary>
        public class BackpackRemoveItem
        {
            public static readonly BackpackRemoveItem Instance = new BackpackRemoveItem();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 删除的物品
            /// </summary>
            public Item item;
        }

        /// <summary>
        /// 通过背包丢弃物品
        /// </summary>
        public class DiscardItemFromBackpack
        {
            public static readonly DiscardItemFromBackpack Instance = new DiscardItemFromBackpack();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 丢弃到地面的物品
            /// </summary>
            public MapItem mapItem;
            /// <summary>
            /// 所在地图
            /// </summary>
            public MapComponent map;
        }

        /// <summary>
        /// 背包中的物品数量变动
        /// </summary>
        public class ItemCountChangeInBackpack
        {
            public static readonly ItemCountChangeInBackpack Instance = new ItemCountChangeInBackpack();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 数量变动的物品
            /// </summary>
            public Item item;
            /// <summary>
            /// 变动前的数量
            /// </summary>
            public int oldCount;
        }
        public class ChecJinBiChangeInBackpack
        {
            public static readonly ChecJinBiChangeInBackpack Instance = new ChecJinBiChangeInBackpack();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
        }
        

    }



    [BsonIgnoreExtraElements]
    public class BackpackComponent: TCustomComponent<Player>
    {
        [BsonIgnore]
        public const int I_PackageWidth = 8;
        [BsonIgnore]
        public const int I_PackageHigh = 12;
        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }
        public ItemsBoxStatus mItemBox;
        /// <summary>
        /// Key:ItemUID
        /// </summary>
        public Dictionary<long, Item> mItemDict = new Dictionary<long, Item>();
        /// <summary>
        /// Key:ItemConfigId
        /// </summary>
        public Dictionary<int, Dictionary<long, Item>> _ConfigId2Item = new Dictionary<int, Dictionary<long, Item>>();
        [BsonIgnore]
        public DBBackpackItem BackPack_DB;
        /// <summary>
        /// 坐骑数据
        /// </summary>
        public Dictionary<long, Item> mMountItemDict = new Dictionary<long, Item>();
        public override void Dispose()
        {
            if (IsDisposeable) return;

            //清理数据
            if (mItemDict != null && mItemDict.Count > 0)
            {
                var mTemp = mItemDict.Values.ToList();
                for (int i = 0, len = mTemp.Count; i < len; i++)
                {
                    mTemp[i].Dispose();
                }
                mItemDict.Clear();
            }
            _ConfigId2Item.Clear();
            mMountItemDict.Clear();
            BackPack_DB?.Dispose();
            base.Dispose();

        }
    }
}
