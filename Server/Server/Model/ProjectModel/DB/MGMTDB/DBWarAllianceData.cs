using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using System.Numerics;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBWarAllianceData : DBBase
    {
        /// <summary>
        /// 战盟ID
        /// </summary>
        [DBMongodb(1)]
        public long DBWarAllianceID { get; set; }
        /// <summary>
        /// 战盟名称
        /// </summary>
        [DBMongodb(11)]
        public string DBWarAllianceName { get; set; }
        /// <summary>
        /// 战盟徽章
        /// </summary>
        public int[] DBWarAllianceBadge { get; set; } = new int[64];
        /// <summary>
        /// 战盟等级(人数)      圣导师MAX:80人 非圣导师MAX:40人
        /// </summary>
        public int DBWarAllianceLevel { get; set; }
        /// <summary>
        /// 战盟公告
        /// </summary>
        public string DBWarAllianNotice { get; set; }
        /// <summary>
        /// 战盟大区ID
        /// </summary>
        public int DBWarAllianAreaId { get; set; }
        /// <summary>
        /// 是否解散 0有效 1解散
        /// </summary>
        [DBMongodb(2)]
        [DBMongodb(11)]
        public int IsDisabled { get; set; }
       
    }

    public class DBMemberInfo : DBBase
    {
        /// <summary>
        /// 战盟ID
        /// </summary>
        public long DBWarAllianceID { get; set; }
        /// <summary>
        /// 成员ID
        /// </summary>
        public long MemberID { get; set; }
        /// <summary>
        /// 成员名称
        /// </summary>
        public string MemberName { get; set; }
        /// <summary>
        /// 成员等级
        /// </summary>
        public int MemberLevel { get; set; }
        /// <summary>
        /// 成员职业类型
        /// </summary>
        public int MemberClassType { get; set; }
        /// <summary>
        /// 成员职务 0成员  1队长  2副盟主  3盟主 各个职位只有一人
        /// </summary>
        public int MemberPost { get; set; }
        /// <summary>
        /// 角色所在大区ID
        /// </summary>
        public int MemberAreaId { get; set; }
        /// <summary>
        /// 是否退盟 0申请 1成员 2退出 3无效
        /// </summary>
        public int IsDisabled { get; set; }
        ///<summary>
        ///解散时间
        /// </summary>
        public long DeleteTime { get; set; }
        /// <summary>
        /// 退出时间
        /// </summary>
        public long ExitTime { get; set; }
        /// <summary>
        /// 战盟积分
        /// </summary>
        public int AllianceScore { get; set; }
    }

    public class DBTheTestTowerRank : DBBase
    {
        public long UserID { get; set; } = 0;
        public string Name { get; set; } = "";
        public int Level { get; set; } = 0;
    }
    public enum RankType
    { 
        Unknown = 0,
        /// <summary>等级排行</summary>
        LevelRank = 1,
        /// <summary>充值排行</summary>
        TopUpRank = 2,
        /// <summary>攻城战结束通知奖励等,非排行 </summary>
        SiegeWarfare =100,
    }
    /// <summary>
    /// 成员访问只能通过扩展接口访问
    /// 接口写好注释
    /// </summary>
    public class RankStructure
    {
        public RankType RankType = RankType.Unknown;
        public int Int32_A;
        public int Int32_B;
        public int Int32_C;
        public int Int32_D;
        public long Int64_A;
        public long Int64_B;
        public long Int64_C;
        public long Int64_D;
        public string Str_A;
        public string Str_B;
        public string Str_C;
        public string Str_D;
    }

    public class RankInfo : DBBase
    {
        public int Type = 0;
        public string RankListInfo = "";
        public bool DataValidity = true;
    }
}
