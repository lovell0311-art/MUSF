using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ETModel
{
    public sealed partial class Enemy : CombatSource
    {
        public override bool OpenObstacle => true;

        public int SourcePosX { get; set; }
        public int SourcePosY { get; set; }

        public long CreatePlayerId { get; set; } = 0;
        public long CreateItemUID { get; set; } = 0;
        /// <summary>
        /// 特殊掉落组ID，用于一怪对多组掉落
        /// </summary>
        public int DropId { get; set; } = 0;
        /// <summary>
        /// 保底掉落
        /// </summary>
        public (int, int) MGItem { get; set; } = (0, 0);
        public List<(int, int)> SourcePoslist { get; set; }
        /// <summary>
        /// 怪物的伤害排行
        /// </summary>
        public Dictionary<long, (string, int)> DamageRanking { get; set; }
        [BsonIgnore]
        public Enemy_InfoConfig Config { get; set; }

        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }

            Config = null;
            if (SourcePoslist != null) SourcePoslist = null;
            if (DamageRanking != null) DamageRanking.Clear();
            base.Dispose();
        }
    }
}