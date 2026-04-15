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
    public class DBGameSkillData : DBBase
    {
        [DBMongodb(1, true)]
        public long GameUserId { get; set; }
        public string Skill { get; set; } = "[]";
        public string SkillUp { get; set; } = "{}";
        /// <summary>
        /// 1 代表删除了
        /// </summary>
        public int IsDispose { get; set; } = 0;


        [BsonIgnore]
        [JsonIgnore]
        public List<int> SkillId { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<int,int> SkillUpInfo { get; set; }

        public void DeSerialize()
        {
            SkillId = Help_JsonSerializeHelper.DeSerialize<List<int>>(Skill);
            SkillUpInfo = Help_JsonSerializeHelper.DeSerialize<Dictionary<int,int>>(SkillUp);
        }
        public void Serialize()
        {
            Skill = Help_JsonSerializeHelper.Serialize(SkillId);
            SkillUp = Help_JsonSerializeHelper.Serialize(SkillUpInfo);
        }
    }
}