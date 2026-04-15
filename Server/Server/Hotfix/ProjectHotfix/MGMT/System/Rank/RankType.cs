using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Aop.Api.Domain;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETHotfix.ProjectHotfix.MGMT.Handler.Rank;
using System.Numerics;
using System.Linq;
namespace ETHotfix
{
    public static class RankClass
    {
        public static RankStructure CreateRankStructure(RankType rankType = RankType.Unknown)
        {
            RankStructure rankStructure = new RankStructure();
            rankStructure.RankType = rankType;
            return rankStructure;
        }
        /// <summary>
        /// 指定规则并根据规则排序
        /// </summary>
        public static void Rank(this RankComponent self,RankType rankType = RankType.Unknown)
        {
            switch (rankType)
            {
                case RankType.LevelRank:
                    {
                        if (self.AllRankList.TryGetValue(RankType.LevelRank, out var m_LevelList))
                        {
                            RankByLevel(m_LevelList);
                        }
                        else
                            Log.Warning("等级排行榜类型异常！！！");
                    }
                    break;
                default:
                    Log.Warning($"{rankType}排行榜类型异常！！！");
                    break;
            }
        }
        public static void AddRank(this RankComponent self, RankType rankType, RankStructure Number)
        {
            if (self.AllRankList.TryGetValue(rankType, out var List))
            {
                switch (rankType)
                {
                    case RankType.LevelRank:
                        var mNumInfo = List.Find(P => P.GetGameUserId() == Number.GetGameUserId());
                        if (mNumInfo != null)
                            List.Remove(mNumInfo);
                        break;
                }
                List.Add(Number);
                self.Rank(rankType);
            }
            else
                self.AllRankList.Add(rankType, new List<RankStructure>() { Number });

            self.SetDB(rankType).Coroutine();
        }
        public static List<RankStructure> GetRankList(this RankComponent self, RankType rankType)
        { 
            if(self.AllRankList.TryGetValue(rankType,out var rankStructures))
                return rankStructures;
            else
                return new List<RankStructure>();
        }
        public static RankType GetRankType(this RankStructure self)
        {
            return self.RankType;
        }
        public static void SetRankType(this RankStructure self, RankType rankType)
        {
            self.RankType = rankType;
        }
        #region 等级排行
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static void SetGameUserId(this RankStructure self, long GameUserId)
        {
            self.Int64_A = GameUserId;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static long GetGameUserId(this RankStructure self)
        {
            return self.Int64_A;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static void SetLevel(this RankStructure self, int level)
        {
            self.Int32_A = level;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static int GetLevel(this RankStructure self)
        {
            return self.Int32_A;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static void SetReincarnate(this RankStructure self, int level)
        {
            self.Int32_C = level;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static int GetReincarnate(this RankStructure self)
        {
            return self.Int32_C;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static void SetRanking(this RankStructure self, int RanKing)
        {
            self.Int32_B = RanKing;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static int GetRanking(this RankStructure self)
        {
            return self.Int32_B;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static void SetName(this RankStructure self, string Name)
        {
            self.Str_A = Name;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static string GetName(this RankStructure self)
        {
            return self.Str_A;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static void SetClassType(this RankStructure self, string ClassType)
        {
            self.Str_B = ClassType;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static string GetClassType(this RankStructure self)
        {
            return self.Str_B;
        }
        /// <summary>
        /// 等级排行接口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Name"></param>
        public static void RankByLevel(List<RankStructure> ranks)
        {
            ranks.Sort((x, y) =>
            {
                int reincarnateComparison = y.GetReincarnate().CompareTo(x.GetReincarnate());
                if (reincarnateComparison != 0)
                {
                    return reincarnateComparison; // 按重生次数降序
                }
                return y.GetLevel().CompareTo(x.GetLevel()); // 如果重生次数相等，按等级降序
            });
            if (ranks.Count > LevelRank.BeforeReservation)
            {
                ranks.RemoveRange(LevelRank.BeforeReservation, ranks.Count - LevelRank.BeforeReservation);
            }
            int currentRank = 1; 
            for (int i = 0; i < ranks.Count; i++)
            {
                ranks[i].SetRanking(currentRank);
                currentRank++;
            }
        }
        #endregion
    }
    
}
