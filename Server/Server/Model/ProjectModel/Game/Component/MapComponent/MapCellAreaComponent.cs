using System.Collections.Generic;
using System.Diagnostics;

using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{

    /// <summary>
    /// 地图分域
    /// </summary>
    public partial class MapCellAreaComponent : TCustomComponent<MapComponent>
    {
        public int AreaIndex { get; set; }
        public int AreaPosX { get; set; }
        public int AreaPosY { get; set; }

        public MapCellAreaComponent LeftUpNode { get; set; }
        public MapCellAreaComponent LeftMiddleNode { get; set; }
        public MapCellAreaComponent LeftDownNode { get; set; }


        public MapCellAreaComponent UpMiddleNode { get; set; }
        public MapCellAreaComponent CenterMiddleNode { get; set; }
        public MapCellAreaComponent DownMiddleNode { get; set; }


        public MapCellAreaComponent RightUpNode { get; set; }
        public MapCellAreaComponent RightMiddleNode { get; set; }
        public MapCellAreaComponent RightDownNode { get; set; }

        public long UpdateTick { get; set; }

        public int obServerCount;
        /// <summary> 观察者数量，大于0 开启ai <summary>
        public int ObServerCount { get { return obServerCount; } }
        public List<int> AroundField { get; set; }
        public MapCellAreaComponent[] AroundFieldArray { get; set; }
        public Dictionary<int, MapCellAreaComponent> AroundFieldDic { get; set; }

        public Dictionary<long, GamePlayer> FieldPlayerDic { get; private set; } = new Dictionary<long, GamePlayer>();
        public Dictionary<long, Enemy> FieldEnemyDic { get; private set; } = new Dictionary<long, Enemy>();
        public Dictionary<long, Summoned> FieldSummonedDic { get; private set; } = new Dictionary<long, Summoned>();
        public Dictionary<long, HolyteacherSummoned> FieldHolyteacherSummonedDic { get; private set; } = new Dictionary<long, HolyteacherSummoned>();

        public Dictionary<long, Pets> FieldPetsDic { get; private set; } = new Dictionary<long, Pets>();

        public Dictionary<long, GameNpc> FieldNpcDic { get; private set; } = new Dictionary<long, GameNpc>();
        /// <summary>
        /// k MapItem.Id v Entity
        /// </summary>
        public Dictionary<long, MapItem> MapItemRes = new Dictionary<long, MapItem>();

        public Dictionary<long, C_Stall> MapStallDic = new Dictionary<long, C_Stall>();

        /// <summary>
        /// 失落的地图捡取标记  k GameUserId v mapID
        /// </summary>
        public Dictionary<long, int> ShiLuoDic = new Dictionary<long, int>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            AreaIndex = default;
            AreaPosX = default;
            AreaPosY = default;
            obServerCount = 0;
            AroundField = null;
            AroundFieldArray = null;
            AroundFieldDic = null;

            if (FieldPlayerDic.Count > 0)
            {
                Log.Error($"Dispose 前，GamePlayer 没清理干净");
                FieldPlayerDic.Clear();
            }
            if (FieldEnemyDic.Count > 0)
            {
                Log.Error($"Dispose 前，Enemy 没清理干净");
                FieldEnemyDic.Clear();
            }
            if (FieldSummonedDic.Count > 0)
            {
                Log.Error($"Dispose 前，Summoned 没清理干净");
                FieldSummonedDic.Clear();
            }
            if (FieldHolyteacherSummonedDic.Count > 0)
            {
                Log.Error($"Dispose 前，HolyteacherSummoned 没清理干净");
                FieldHolyteacherSummonedDic.Clear();
            }

            if (FieldPetsDic.Count > 0)
            {
                Log.Error($"Dispose 前，Pets 没清理干净");
                FieldPetsDic.Clear();
            }
            if (FieldNpcDic.Count > 0) FieldNpcDic.Clear();
            if (MapItemRes.Count > 0)
            {
                foreach(MapItem mapItem in MapItemRes.Values)
                {
                    Log.Warning($"MapItem.InstanceId:{mapItem.InstanceId} 没清理");
                }
                Log.Error($"Dispose 前，MapItem 没清理干净");
                MapItemRes.Clear();
            }
            if (MapStallDic.Count > 0) MapStallDic.Clear();

            base.Dispose();
        }
    }
}