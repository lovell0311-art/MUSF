using System;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    public partial class CombatSource
    {
        #region AOI缓存
        public static List<int> leaveAroundField = new List<int>();
        public static List<int> currentAroundField = new List<int>();
        public static List<int> intoAroundField = new List<int>();
        #endregion

        public DBPlayerUnitData UnitData { get; set; }

        public MapCellAreaComponent currentCell;
        public Vector2Int position = new Vector2Int();

        /// <summary> 单位所在的地图中的Cell </summary>
        public MapCellAreaComponent CurrentCell { get { return currentCell; } }

        /// <summary> 单位所在的地图 </summary>
        public MapComponent CurrentMap { 
            get
            {
                if (currentCell == null) return null;
                return currentCell.Parent;
            }
        }
        /// <summary> 单位在地图中的位置 </summary>
        public Vector2Int Position { get => position; }

        public virtual bool OpenObstacle { get; } = false;

        /// <summary>
        /// 开始移动时间
        /// </summary>
        public long MoveStartTime { get; set; }
        /// <summary>
        /// 移动时间
        /// </summary>
        public long MoveNeedTime { get; set; }
        /// <summary>
        /// 移动静默时间
        /// </summary>
        public long MoveSleepTime { get; set; }
        /// <summary>
        /// 移动停止时休息时间
        /// </summary>
        public long MoveRestTime { get; set; }
        /// <summary>
        /// 死亡复活等待时间
        /// </summary>
        public long DeathSleepTime { get; set; }

        /// <summary>
        /// 寻路数据
        /// </summary>
        public List<C_FindTheWay2D> Pathlist { get; set; }

        private void ClearUnitData()
        {
            UnitData = null;
            currentCell = null;
            position.x = 0;
            position.y = 0;
            MoveStartTime = default;
            MoveNeedTime = default;
            MoveSleepTime = default;
            MoveRestTime = default;
            DeathSleepTime = default;

            if (Pathlist != null)
                Pathlist = null;
        }
    }
}