using CustomFrameWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ETModel
{
    /// <summary>
    /// 角色技能
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DBMasterData : DBBase
    {
        [DBMongodb(1)]
        public long GameUserId { get; set; }

        public int PropertyPoint { get; set; }

        public string Skill { get; set; } = "{}";
        /// <summary>
        /// 1 代表删除了
        /// </summary>
        public int IsDispose { get; set; } = 0;
        /// <summary>
        /// 额外点数
        /// </summary>
        public int ExtraPoints { get; set; } = 0;

        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<int, int> SkillId { get; set; }

        public void DeSerialize()
        {
            SkillId = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(Skill);
        }
        public void Serialize()
        {
            Skill = Help_JsonSerializeHelper.Serialize(SkillId);
        }
    }
}