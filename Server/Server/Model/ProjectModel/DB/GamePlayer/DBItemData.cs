using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Options;

namespace ETModel
{


    [BsonIgnoreExtraElements]
    public class DBItemData : DBBase
    {
        [DBMongodb(1,true)]
        [DBMongodb(12, true)]
        public long UserId { get; set; } = 0;
        /// <summary>
        /// 所属玩家，对应DBGamePlayer的ID，Player的GameUserId
        /// </summary>
        [DBMongodb(2, true)]
        [DBMongodb(11, true)]
        public long GameUserId { get; set; } = 0;

        public int ConfigID { get; set; }

        /// <summary>
        /// 属于哪个组件
        /// 记录这个物品由哪个组件接管
        /// 在打印日志时进行赋值
        /// </summary>
        [DBMongodb(3,true)]
        [DBMongodb(11, true)]
        [DBMongodb(12, true)]
        public EItemInComponent InComponent { get; set; } = EItemInComponent.None;
        /// <summary>
        /// 位置id
        /// </summary>
        public int posId { get; set; } = 0;
        /// <summary>
        /// 位置XY(背包或地图位置)
        /// </summary>
        public int posX { get; set; }
        public int posY { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        public long CreateTimeTick { get; set; }

        /// <summary>
        /// 销毁标志, >= 1 代表已删除
        /// </summary>
        [DBMongodb(4, true)]
        [DBMongodb(11, true)]
        [DBMongodb(12, true)]
        public long IsDispose { get; set; }

        /// <summary>
        /// 所在区服id
        /// </summary>
        [DBMongodb(5, true)]
        [DBMongodb(11, true)]
        [DBMongodb(12, true)]
        public int GameAreaId { get; set; }
        /// <summary>
        /// Key:(int)EItemValue
        /// 物品附加属性，多用于装备
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, int> PropertyData = new Dictionary<int, int>();
        /// <summary>
        /// 卓越属性词条
        /// </summary>
        public HashSet<int> ExcellentEntry = new HashSet<int>();
        /// <summary>
        /// 额外属性词条，用于套装
        /// k: entryId v: entryLevel
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, int> ExtraEntry = new Dictionary<int, int>();
        /// <summary>
        /// 特殊属性词条，用于翅膀
        /// k: entryId v: entryLevel
        /// </summary>
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<int, int> SpecialEntry = new Dictionary<int, int>();
        /// <summary>
        /// 物品耐久平滑值
        /// </summary>
        public int DurabilitySmall = 0;

    }
}
