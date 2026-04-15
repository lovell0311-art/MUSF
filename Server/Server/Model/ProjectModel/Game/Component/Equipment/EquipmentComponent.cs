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
        /// 装备初始化完成
        /// </summary>
        public class EquipInitComplete
        {
            public static readonly EquipInitComplete Instance = new EquipInitComplete();
            /// <summary>
            /// 触发单位
            /// </summary>
            public GamePlayer unit;
        }

        /// <summary>
        /// 穿戴装备
        /// </summary>
        public class EquipItem
        {
            public static readonly EquipItem Instance = new EquipItem();
            /// <summary>
            /// 触发单位
            /// </summary>
            public GamePlayer unit;
            /// <summary>
            /// 穿戴的物品
            /// </summary>
            public Item item;
            /// <summary>
            /// 穿戴的位置
            /// </summary>
            public EquipPosition position;
        }

        /// <summary>
        /// 卸下装备
        /// </summary>
        public class UnloadEquipItem
        {
            public static readonly UnloadEquipItem Instance = new UnloadEquipItem();
            /// <summary>
            /// 触发单位
            /// </summary>
            public GamePlayer unit;
            /// <summary>
            /// 卸下的物品
            /// </summary>
            public Item item;
            /// <summary>
            /// 卸下的位置
            /// </summary>
            public EquipPosition position;
        }

    }



    [BsonIgnoreExtraElements]
    public class EquipmentComponent: TCustomComponent<Player>
    {
        public static readonly ETHotfix.G2C_ChangeValue_notice g2C_ChangeValue_notice = new ETHotfix.G2C_ChangeValue_notice();

        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }
        /// <summary>
        /// Key:(int)EquipPosition
        /// </summary>
        public Dictionary<int, Item> EquipPartItemDict = new Dictionary<int, Item>();

        /// <summary>
        /// 临时卡槽，不会保存的数据库
        /// </summary>
        public Dictionary<EquipPosition, Item> TempSlotDict = new Dictionary<EquipPosition, Item>();

        [BsonIgnore]
        public DBEquipmentItem Equipment_DB;

        public long TimerId;

        #region Cache
        // 装备加成缓存
        public Dictionary<E_GameProperty, int> EquipPropertyCacheDic = new Dictionary<E_GameProperty, int>();
        #endregion

        public override void Dispose()
        {
            if (IsDisposeable) return;

            //清理数据
            if (EquipPartItemDict != null && EquipPartItemDict.Count > 0)
            {
                var mTemp = EquipPartItemDict.Values.ToList();
                for (int i = 0, len = mTemp.Count; i < len; i++)
                {
                    mTemp[i].Dispose();
                }
                EquipPartItemDict.Clear();
            }
            Equipment_DB?.Dispose();
            TempSlotDict.Clear();

            base.Dispose();

        }
    }
}
