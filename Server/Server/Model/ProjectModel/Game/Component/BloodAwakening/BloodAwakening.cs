using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using CustomFrameWork.Baseic;
using System.Net.NetworkInformation;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class PlayerBloodAwakeningComponent : TCustomComponent<Player>
    {
        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }
        public Dictionary<int, DBBloodAwakeningInfo> BloodAwakeningInfo { get; set; }
        /// <summary>
        /// 正在使用的血脉ID
        /// </summary>
        public int UseBloodAwakeningId { get; set; } = 0;

        public override void Dispose()
        {
            BloodAwakeningInfo.Clear();
            UseBloodAwakeningId = 0;
        }
    }
    public enum SynchronizationType
    { 
        /// <summary>
        /// 同步血脉效果给其他玩家
        /// </summary>
        Blood = 30,
    }
    public class DBBloodAwakeningInfo : DBBase
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public long GameUserId { get; set; } = 0;
        /// <summary>
        /// 血脉Id
        /// </summary>
        public int BloodAwakeningId { get; set; } = 0;
        /// <summary>
        /// 节点数据
        /// </summary>
        public string AttributeNodeStr { get; set; } = "";
        /// <summary>
        /// 激活下一环的时间
        /// </summary>
        public long ActivateNeedTime { get; set; } = 0;
        /// <summary>
        /// 拥有的环数
        /// </summary>
        public int UrrentRingNumber { get; set; } = 0;
        /// <summary>
        /// 正在使用
        /// </summary>
        public bool IsUse { get; set; } = false;
        /// <summary>
        /// 是否有效
        /// </summary>
        public int IsDisabled { get; set; } = 0;

        /// <summary>
        /// 属性节点
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public Dictionary<int, List<int>> AttributeNode { get; set; } = new Dictionary<int, List<int>>();
        /// <summary>
        /// 属性节点
        /// </summary>
        [BsonIgnore]
        [JsonIgnore]
        public List<int> Attribute { get; set; } = new List<int>();
    }
}
