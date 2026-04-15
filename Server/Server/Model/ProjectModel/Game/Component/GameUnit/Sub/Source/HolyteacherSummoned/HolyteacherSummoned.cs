using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ETModel
{
    public sealed partial class HolyteacherSummoned : CombatSource
    {
        public override bool OpenObstacle => true;

        [BsonIgnore]
        public Enemy_InfoConfig Config { get; set; }
        [BsonIgnore]
        public GamePlayer GamePlayer { get; set; }

        [BsonIgnore]
        public Item Item { get; set; }

        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }

            GamePlayer = null;
            Config = null;
            Item = null;

            base.Dispose();
        }
    }
}