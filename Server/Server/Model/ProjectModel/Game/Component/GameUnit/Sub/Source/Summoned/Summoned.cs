using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ETModel
{
    public sealed partial class Summoned : CombatSource
    {
        public override bool OpenObstacle => true;

        [BsonIgnore]
        public Enemy_InfoConfig Config { get; set; }
        [BsonIgnore]
        public GamePlayer GamePlayer { get; set; }

        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }

            GamePlayer = null;
            Config = null;

            base.Dispose();
        }
    }
}