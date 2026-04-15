using CustomFrameWork.Baseic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class WarehouseComponent : TCustomComponent<Player>
    {
        public static readonly ETHotfix.G2C_WarehouseGoldChange_notice g2C_WarehouseGoldChange_notice = new ETHotfix.G2C_WarehouseGoldChange_notice();
        [BsonIgnore]
        public const int I_WarehouseWidth = 8;
        [BsonIgnore]
        public const int I_WarehouseHigh = 11;

        public const int I_MaxExtenedCapacity = I_WarehouseWidth * I_WarehouseHigh * 10;
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
        [BsonIgnore]
        public DBWarehouseItem Warehouse_DB;

        #region Coin
#pragma warning disable CS0618
        public long Coin { get{ return Warehouse_DB.Coin; } private set { } }
#pragma warning restore CS0618
        #endregion

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
            Warehouse_DB?.Dispose();
            base.Dispose();

        }
    }
}
