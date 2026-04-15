using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    public class NpcInfo
    {
        public int Index { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Direction { get; set; }
    }

    public class MapNpcInfoComponent : Entity
    {
        public List<NpcInfo> AllNpcSpawnPoint;
    }
}
