using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
namespace ETModel
{
    public sealed partial class GamePlayer : CombatSource
    {
        protected override int PoolCountMax => 0; 

        [BsonIgnore]
        public CreateRole_InfoConfig Config { get; set; }
        [BsonIgnore]
        public DBGamePlayerData Data { get; set; }

        [BsonIgnore]
        public Player Player { get; set; }
        [BsonIgnore]
        public HolyteacherSummoned HolyteacherSummoned { get; set; }


        /// <summary>
        /// 召唤物
        /// </summary>
        [BsonIgnore]
        public Summoned Summoned { get; set; }
        [BsonIgnore]
        public Pets Pets { get; set; }
        [BsonIgnore]
        public Dictionary<long, Pets> PetsList { get; set; }

        [BsonIgnore]
        public DBCharacterDroplimit dBCharacterDroplimit { get; set; }
        /// <summary>
        /// 忽略的传送点Id
        /// </summary>
        public int MoveIgnoreTransferId { get; set; }
        /// <summary>
        /// 聊天CD时间共用于世界和附件
        /// </summary>
        public long SendMassgeTime { get; set; }
        public E_PKModel _PKModel { get; set; } = E_PKModel.Peace;

        /// <summary>
        /// 月卡副本复活次数
        /// </summary>
        public int CopyLiveCnt { get; set; } = 0;
        public int RankIndex { get; set; } = 100;
        /// <summary>
        /// 试炼塔所在层数
        /// </summary>
        public int CopyCount { get; set; } = 0;
        /// <summary>
        /// 试炼塔是否通知了
        /// </summary>
        public bool ISNo { get; set; } = false;
        public Dictionary<long, long> FanJiIdlist { get; set; } = new Dictionary<long, long>();
        public string Code { get; set; } = "";
        public override void Dispose()
        {
            if (this.IsDisposeable) return;
            RankIndex = 100;
            Config = null;
            Data = null;
            Player = null;
            Summoned = null;
            HolyteacherSummoned = null;
            MoveIgnoreTransferId = default;
            _PKModel = E_PKModel.Peace;
            if (FanJiIdlist.Count > 0) FanJiIdlist.Clear();

            if(PetsList != null)
                foreach (var item in PetsList)
                    item.Value.Dispose();

            if (Pets != null)
                Pets.Dispose();

            PetsList = null;
            Pets = null;

            base.Dispose();
        }
    }
}