using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 地图管理器
    /// </summary>
    public partial class MapManageComponent : TCustomComponent<C_ServerArea>
    {
        #region TempStruct
        /// <summary>
        /// 临时结构体 
        /// </summary>
        public class MapInfo
        {
            public int width { get; set; }
            public int height { get; set; }
            public int SceneInfoSize { get; set; }
            public List<int> SceneInfos { get; set; }

        }

        public class MapDeliveryAreaInfo
        {
            public int Index { get; set; }
            public int MapIndex { get; set; }
            public int PositionX { get; set; }
            public int PositionY { get; set; }
        }
        #endregion

        #region StaticVar
        public static readonly Dictionary<int, MapInfo> MapInfoCacheDic = new Dictionary<int, MapInfo>();
        public static readonly Dictionary<int, List<MapSafeAreaInfo>> MapSafeAreaInfoCacheDic = new Dictionary<int, List<MapSafeAreaInfo>>();
        public static readonly Dictionary<int, List<MapSafeAreaInfo>> MapSpawnPathCacheDic = new Dictionary<int, List<MapSafeAreaInfo>>();
        public static readonly Dictionary<int, List<MapDeliveryAreaInfo>> MapTransferPointPathCacheDic = new Dictionary<int, List<MapDeliveryAreaInfo>>();
        public static readonly Dictionary<int, Dictionary<int, List<MapSafeAreaInfo>>> MapEnemySpawnPathCacheDic = new Dictionary<int, Dictionary<int, List<MapSafeAreaInfo>>>();
        public static readonly Dictionary<int, List<MapSafeAreaInfo>> MapNpcPathCacheDic = new Dictionary<int, List<MapSafeAreaInfo>>();
        #endregion
        /// <summary>
        /// 全部地图
        /// </summary>
        public Dictionary<int, MapComponent> keyValuePairs = new Dictionary<int, MapComponent>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            if (keyValuePairs.Count > 0)
            {
                var mTemp = keyValuePairs.Values.ToList();
                for (int i = 0, len = mTemp.Count; i < len; i++)
                {
                    mTemp[i].Dispose();
                }
                keyValuePairs.Clear();
            }

            base.Dispose();
        }
    }
}