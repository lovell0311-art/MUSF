using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using MongoDB.Bson.Serialization.Attributes;
using PF;
using TencentCloud.Tics.V20181115.Models;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class PlayerWarAllianceComponent : TCustomComponent<Player>
    {
        [BsonIgnore]
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }
        /// <summary>
        /// 战盟ID
        /// </summary>
        public long WarAllianceID { get; set; }
        /// <summary>
        /// 战盟名称
        /// </summary>
        public string WarAllianceName { get; set; }
        /// <summary>
        /// 战盟徽章 8*8的矩阵
        /// </summary>
        public int[] WarAllianceBadge { get; set; }
        /// <summary>
        /// 战盟等级(人数)      圣导师MAX:80人 非圣导师MAX:40人
        /// </summary>
        public int WarAllianceLevel { get; set; }
        /// <summary>
        /// 战盟公告
        /// </summary>
        public string WarAllianNotice { get; set; }
        /// <summary>
        /// 职务 0成员  1队长  2副盟主  3盟主 各个职位只有一人
        /// </summary>
        public int MemberPost { get; set; }
        /// <summary>
        /// 解散时间
        /// </summary>
        public long DeleteTime { get; set; }
        /// <summary>
        /// 退出时间
        /// </summary>
        public long ExitTime { get; set; }
        /// <summary>
        /// 申请记录
        /// </summary>
        public long[] WarAllianceList { get; set; }
        /// <summary>
        /// 盟主名称
        /// </summary>
        public string AllianceLeaderName { get; set; }
        /// <summary>
        /// 战盟积分
        /// </summary>
        public int AllianceScore { get; set; }
        public override void Dispose()
        {
            WarAllianceID = 0;
            WarAllianceName = "";
            WarAllianceBadge = new int[64];
            WarAllianceLevel = 0;
            WarAllianNotice = "";
            MemberPost = 0;
            DeleteTime = 0;
            ExitTime = 0;
            WarAllianceList = new long[5];
            AllianceScore = 0;
        }
    }
   
    public enum PostType
    {
        /// <summary>
        /// 成员
        /// </summary>
        TeamMember = 0,
        /// <summary>
        /// 小队长
        /// </summary>
        Captain    = 1,
        /// <summary>
        /// 副盟主
        /// </summary>
        ViceAllianceLeader = 2,
        /// <summary>
        /// 盟主
        /// </summary>
        AllianceLeader = 3,
    }
}
