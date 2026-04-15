using CustomFrameWork;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBStallItem : DBBase
    {
        /// <summary>
        /// 所属玩家，对应DBGamePlayer的ID，Player的GameUserId
        /// </summary>
        [DBMongodb(1, true)]
        public long GameUserId { get; set; }
        public int AreaId { get; set; }
        public string ItemList { get; set; } = "{}";
        public int IsDispose { get; set; }   //1代表摆摊

        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<long, (int, int)> StallItemlist { get; set; }

        public void DeSerialize()
        {
             StallItemlist = Help_JsonSerializeHelper.DeSerialize<Dictionary<long, (int, int)>>(ItemList);
        }
        public void Serialize()
        {
            ItemList = Help_JsonSerializeHelper.Serialize(StallItemlist);
        }
    }
}
