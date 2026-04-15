using System.Collections.Generic;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using Newtonsoft.Json;

namespace ETModel
{


    /// <summary>
    /// 物品创建属性
    /// 新属性，可自己扩展
    /// </summary>
    [BsonIgnoreExtraElements]
    public partial class ItemCreateAttr
    {
        /// <summary>
        /// 物品强化等级
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [DefaultValue(0)]
        public int Level = 0;
        /// <summary>
        /// 物品数量
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(1)]
        [DefaultValue(1)]
        public int Quantity = 1;
        /// <summary>
        /// 使用配置表中的第几条追加属性 0 开始
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [DefaultValue(0)]
        public int OptListId = 0;
        /// <summary>
        /// 追加等级
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [DefaultValue(0)]
        public int OptLevel = 0;
        /// <summary>
        /// 有技能
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(false)]
        [DefaultValue(false)]
        public bool HaveSkill = false;
        /// <summary>
        /// 有幸运属性
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(false)]
        [DefaultValue(false)]
        public bool HaveLucky = false;
        /// <summary>
        /// 套装id
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [DefaultValue(0)]
        public int SetId = 0;
        /// <summary>
        /// 是绑定的物品 1.绑定账号 2.绑定角色
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [DefaultValue(0)]
        public int IsBind = 0;
        /// <summary>
        /// 是任务物品
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(false)]
        [DefaultValue(false)]
        public bool IsTask = false;
        /// <summary>
        /// 有效时间 秒 如: 7天到期写法, ValidTime = 60 * 60 * 24 * 7
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [DefaultValue(0)]
        public int ValidTime = 0;
        /// <summary>
        /// 过期时间戳 秒 (ExpireTimestamp > 0 会覆盖 ValidTime 参数)
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [DefaultValue(0)]
        public int ExpireTimestamp = 0;
        /// <summary>
        /// 荧光宝石属性 id:value/100  level:value%100
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [DefaultValue(0)]
        public int FluoreAttr = 0;
        /// <summary>
        /// 镶嵌孔数 0-5
        /// </summary>
        [BsonIgnoreIfDefault]
        [BsonDefaultValue(0)]
        [DefaultValue(0)]
        public int FluoreSlotCount = 0;
        /// <summary>
        /// 每个镶嵌孔洞的属性 id:value/100  level:value%100  Count <= 5
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public List<int> FluoreSlot = new List<int>();
        /// <summary>
        /// 卓越属性
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public List<int> OptionExcellent = new List<int>();
        /// <summary>
        /// 特殊属性 用于翅膀
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public Dictionary<int, int> OptionSpecial = new Dictionary<int, int>();
        /// <summary>
        /// 自定义属性方法
        /// 可参考 ItemRandomlyAddExcAttr_3.cs
        /// </summary>
        [JsonIgnore]
        [BsonIgnore]
        public List<string> CustomAttrMethod = new List<string>();
    }



    public partial class ItemCreateAttr
    {
        [JsonProperty("FluoreSlot")]
        [DefaultValue(null)]
        [BsonIgnoreIfDefault]
        [BsonElement("FluoreSlot")]
        [BsonDefaultValue(null)]
        private List<int> FluoreSlotExtensions
        {
            get => FluoreSlot?.Count > 0 ? FluoreSlot : null;
            set => FluoreSlot = value ?? new List<int>();
        }

        [JsonProperty("OptionExcellent")]
        [DefaultValue(null)]
        [BsonIgnoreIfDefault]
        [BsonElement("OptionExcellent")]
        [BsonDefaultValue(null)]
        private List<int> OptionExcellentExtensions
        {
            get => OptionExcellent?.Count > 0 ? OptionExcellent : null;
            set => OptionExcellent = value ?? new List<int>();
        }

        [JsonProperty("OptionSpecial")]
        [DefaultValue(null)]
        [BsonIgnoreIfDefault]
        [BsonElement("OptionSpecial")]
        [BsonDefaultValue(null)]
        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
        private Dictionary<int, int> OptionSpecialExtensions
        {
            get => OptionSpecial?.Count > 0 ? OptionSpecial : null;
            set => OptionSpecial = value ?? new Dictionary<int, int>();
        }

        [JsonProperty("CustomAttrMethod")]
        [DefaultValue(null)]
        [BsonIgnoreIfDefault]
        [BsonElement("CustomAttrMethod")]
        [BsonDefaultValue(null)]
        private List<string> CustomAttrMethodExtensions
        {
            get => CustomAttrMethod?.Count > 0 ? CustomAttrMethod : null;
            set => CustomAttrMethod = value ?? new List<string>();
        }
    }

}
