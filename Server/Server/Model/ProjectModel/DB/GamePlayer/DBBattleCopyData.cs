using MongoDB.Bson.Serialization.Attributes;

namespace ETModel
{
    [BsonIgnoreExtraElements]
    public class DBBattleCopyData : DBBase
    {
        /// <summary>
        /// 所属玩家，对应DBGamePlayer的ID，Player的GameUserId
        /// </summary>
        [DBMongodb(11)]
        public long GameUserId { get; set; }

        [DBMongodb(11)]
        public int GameAreaId { get; set; }
        //更新时间
        public long updateTime;
        /// <summary>
        /// 恶魔城堡挑战次数
        /// </summary>
        public int demonSquaeNum;
        /// <summary>
        /// 血色城堡挑战次数
        /// </summary>
        public int redCastleNum;
        /// <summary>
        /// 试炼塔次数
        /// </summary>
        public int TrialTowerNum;
        /// <summary>
        /// 试炼之地通关最高层次
        /// </summary>
        public int TrialGroundCnt;
        //今日更新时间
        public long updateTimer;

    }
}
