using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ETModel.Robot
{
    public class Map : Entity
    {
        public int MapId = 0;
        public AstarFindPath Astar = null;
        public byte[,] _SafeArea = null;


        public Dictionary<long, Unit> UnitDict = new Dictionary<long, Unit>();

        public Dictionary<long, Unit> PlayerDict = new Dictionary<long, Unit>();
        public Dictionary<long, Unit> MonsterDict = new Dictionary<long, Unit>();
        public Dictionary<long, Unit> ItemDict = new Dictionary<long, Unit>();
        public Dictionary<long, Unit> PetDict = new Dictionary<long, Unit>();
        public Dictionary<long, Unit> SummonedDict = new Dictionary<long, Unit>();
        public Dictionary<long, Unit> NpcDict = new Dictionary<long, Unit>();

        public Queue<Unit> LeavedUnitQueue = new Queue<Unit>();



        public bool IsSafeArea(Vector2Int pos)
        {
            if (_SafeArea == null) return false;
            if (pos.x < 0 ||
                pos.y < 0 ||
                pos.x >= Astar.Wight ||
                pos.y >= Astar.Height) return false;
            return (_SafeArea[pos.x, pos.y] == 1);
        }
    }
}
