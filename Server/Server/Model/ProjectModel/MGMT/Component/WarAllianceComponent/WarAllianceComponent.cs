using System;
using System.Collections.Generic;
using System.Diagnostics;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Threading.Tasks;
using ETHotfix;
using CustomFrameWork.Component;
using System.Collections;
using Org.BouncyCastle.Utilities;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ETModel
{
    public class MemberInfo
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        public long MemberID { get; set; } = 0;
        /// <summary>
        /// 成员名称
        /// </summary>
        public string MemberName { get; set; } = "";
        /// <summary>
        /// 成员等级
        /// </summary>
        public int MemberLevel { get; set; } = 0;
        /// <summary>
        /// 成员职业类型
        /// </summary>
        public int MemberClassType { get; set; } = 0;
        /// <summary>
        /// 成员职务 0成员  1队长  2副盟主  3盟主 各个职位只有一人
        /// </summary>
        public int MemberPost { get; set; } = 0;
        /// <summary>
        /// 成员所在线路
        /// </summary>
        public int MeberServerID { get; set; } = 0;
        ///<summary>
        ///在线状态 0在线 1离线
        /// </summary>
        public int MeberState { get; set; } = 0;
        /// <summary>
        /// 战盟积分
        /// </summary>
        public int AllianceScore { get; set; } = 0;
    }
    public class WarAllianceInfo
    {
        /// <summary>
        /// 战盟ID
        /// </summary>
        public long WarAllianceID { get; set; } = 0;
        /// <summary>
        /// 战盟名称
        /// </summary>
        public string WarAllianceName { get; set; } = "";
        /// <summary>
        /// 战盟徽章 8*8的矩阵
        ///   0 | 1 | 2 | 3 | 4 | 5 | 6 | 7
        ///   8 | 9 | 10| 11| 12| 13| 14| 15
        ///   。。。。。。以此类推至最后一位
        /// </summary>
        public int[] WarAllianceBadge { get; set; } = new int[64];
        /// <summary>
        /// 战盟等级(人数)盟主等级/10      圣导师MAX:80人 非圣导师MAX:40人
        /// </summary>
        public int WarAllianceLevel { get; set; } = 0;
        /// <summary>
        /// 战盟公告
        /// </summary>
        public string WarAllianNotice { get; set; } = "";
        /// <summary>
        /// 盟主名称
        /// </summary>
        public string AllianceLeaderName { get; set; } = "";
        /// <summary>
        /// 数据库大区ID
        /// </summary>
        public int mAreaId { get; set; } = 0;
        /// <summary>
        /// 战盟成员列表
        /// </summary>
        public Dictionary<long, MemberInfo> MemberList { get; set; } = new Dictionary<long, MemberInfo>();
        /// <summary>
        /// 申请列表
        /// </summary>
        public Dictionary<long, MemberInfo> MemberList2 { get; set; } = new Dictionary<long, MemberInfo>();
    }
    public class WarAllianceComponent : TCustomComponent<MainFactory>
    {
        public Dictionary<long, WarAllianceInfo> WarAllianceList = new Dictionary<long, WarAllianceInfo>();
        public Dictionary<long, (long, long)> keyValuePairs = new Dictionary<long, (long, long)>();
        public List<int> DBID= new List<int>();
        public override void Awake()
        {
            WarAllianceList.Clear();
            DBID.Clear();
            Log.Info($"ServerMGMT  Loading......");
        }
        public override void Dispose()
        {
            WarAllianceList.Clear();
            DBID.Clear();
        }
    }
    public class RankComponent : TCustomComponent<MainFactory>
    {
        public Dictionary<RankType, RankInfo> rankInfo { get; set; }
        public Dictionary<RankType,List<RankStructure>> AllRankList;
        public override void Dispose()
        {
            AllRankList.Clear();
            rankInfo.Clear();
        }
    }
}
