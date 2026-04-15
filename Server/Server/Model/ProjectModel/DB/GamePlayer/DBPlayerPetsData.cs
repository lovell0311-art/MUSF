using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBPetsData : DBBase
    {
        /// <summary>
        /// 玩家ID
        /// </summary>
        [DBMongodb(11)]
        [DBMongodb(12)]
        public long GameUserId { get; set; }
        /// <summary>
        /// 宠物配置表ID
        /// </summary>
        public int ConfigID { get; set; }
        /// <summary>
        /// 宠物ID
        /// </summary>
        [DBMongodb(11)]
        public long PetsId { get; set; }
        /// <summary>
        /// 宠物名称
        /// </summary>
        public string PetsName { get; set; }
        /// <summary>
        /// 宠物等级
        /// </summary>
        public int PetsLevel { get; set; }
        /// <summary>
        /// 是否出战
        /// </summary>
        public int PetsUseState { get; set; }
        /// <summary>
        /// 宠物经验
        /// </summary>
        public long PetsExp { get; set; }
        /// <summary>
        /// 死亡计时(毫秒)
        /// </summary>
        public long DeathTime { get; set; }
        /// <summary>
        /// 升级加点
        /// </summary>
        public int AttributePoint { get; set; }
        /// <summary>
        /// 魔法值
        /// </summary>
        public int PetsMP { get; set; }
        /// <summary>
        /// 生命值
        /// </summary>
        public int PetsHP { get; set; }
        /// <summary>
        /// 力量
        /// </summary>
        public int PetsSTR { get; set; }
        /// <summary>
        /// 敏捷
        /// </summary>
        public int PetsDEX { get; set; }
        /// <summary>
        /// 体力
        /// </summary>
        public int PetsPSTR { get; set; }
        /// <summary>
        /// 智力
        /// </summary>
        public int PetsPINT { get; set; }
        /// <summary>
        /// 魔法系强化加智力
        /// </summary>
        public int PintAdd { get; set; } = 0;
        /// <summary>是否删除 0:未删 1:删除</summary>
        [DBMongodb(11)]
        [DBMongodb(12)]
        public int IsDisabled { get; set; }
        /// <summary>
        /// 使用中的技能
        /// </summary>
        public int UseSkillID { get; set; }
        /// <summary>
        /// 技能列表
        /// </summary>
        public string SkillID { get; set; } = "[]";
        /// <summary>
        /// 试用时间秒
        /// </summary>
        public long PetsTrialTime { get; set; }
        /// <summary>
        /// 卓越属性
        /// </summary>
        public string Excellent { get; set; } = "[]";
        /// <summary>
        /// 强化等级
        /// </summary>
        public short EnhanceLv { get; set; } = 0;
        /// <summary>
        /// 进阶等级
        /// </summary>
        public short AdvancedLevel { get; set; } = 0;
        [BsonIgnore]
        [JsonIgnore]
        public List<int> SkillId { get; set; } = new List<int>();
        [BsonIgnore]
        [JsonIgnore]
        public List<int> ExcellentId { get; set; } = new List<int>();
    }
}