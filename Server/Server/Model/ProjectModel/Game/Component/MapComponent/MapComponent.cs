using System.Collections.Generic;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    namespace EventType.NSMapComponent
    {
        /// <summary>
        /// 通知 Hotfix 层，自己将被销毁
        /// </summary>
        public class Destory
        {
            public static readonly Destory Instance = new Destory();
            public ETModel.MapComponent self;
        }
    }

    public partial class MapComponent : TCustomComponent<MapManageComponent>
    {
        public int MapId { get; set; }
        public int MapWidth { get; set; }
        public int MapHight { get; set; }

        public int monsterId { get; set; }

        /// <summary>
        /// 分域
        /// </summary>
        public Dictionary<int, Dictionary<int, MapCellAreaComponent>> MapCellFieldDic { get; set; } = new Dictionary<int, Dictionary<int, MapCellAreaComponent>>();
        public List<MapCellAreaComponent> MapCellFieldlist { get; set; } = new List<MapCellAreaComponent>();

        public FindPathComponent FindPathComponent { get; set; }
        public C_FindTheWay2D[,] FindTheWayDic { get; set; }
        /// <summary>
        /// 安全区
        /// </summary>
        public Dictionary<int, Dictionary<int, C_FindTheWay2D>> SafeFindTheWayDic { get; set; } = new Dictionary<int, Dictionary<int, C_FindTheWay2D>>();
        /// <summary>
        /// 出生地
        /// </summary>
        public Dictionary<int, Dictionary<int, C_FindTheWay2D>> SpawnSafeFindTheWayDic { get; set; } = new Dictionary<int, Dictionary<int, C_FindTheWay2D>>();

        /// <summary>
        /// 传送点
        /// </summary>
        public Dictionary<int, List<C_FindTheWay2D>> TransferPointFindTheWayDic { get; set; } = new Dictionary<int, List<C_FindTheWay2D>>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            // 自己将被销毁
            EventType.NSMapComponent.Destory.Instance.self = this;
            Root.EventSystem.OnRun("NSMapComponent.Destory", EventType.NSMapComponent.Destory.Instance);

            MapId = default;
            MapWidth = default;
            MapHight = default;
            monsterId = default;

            FindPathComponent = null;
            FindTheWayDic = null;
            if (SafeFindTheWayDic.Count > 0)
            {
                SafeFindTheWayDic.Clear();
            }
            if (SpawnSafeFindTheWayDic.Count > 0)
            {
                SpawnSafeFindTheWayDic.Clear();
            }
            if (TransferPointFindTheWayDic.Count > 0)
            {
                TransferPointFindTheWayDic.Clear();
            }

            if (MapCellFieldDic.Count > 0)
            {
                MapCellFieldDic.Clear();
            }
            if (MapCellFieldlist.Count > 0)
            {
                for (int i = 0, len = MapCellFieldlist.Count; i < len; i++)
                {
                    var mMapCellField = MapCellFieldlist[i];

                    mMapCellField.Dispose();
                }
                MapCellFieldlist.Clear();
            }

            base.Dispose();
        }
    }
}