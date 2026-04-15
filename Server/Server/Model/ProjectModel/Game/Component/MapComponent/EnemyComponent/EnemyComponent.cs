using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 临时结构体 
    /// </summary>
    public class MapSafeAreaInfo
    {
        public int Index { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Direction { get; set; }

    }
    public partial class EnemyComponent : TCustomComponent<MapComponent>
    {
        public Dictionary<int, List<MapSafeAreaInfo>> EnemyDataDic = new Dictionary<int, List<MapSafeAreaInfo>>();
        public Dictionary<long, Enemy> AllEnemyDic = new Dictionary<long, Enemy>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            //TODO 清理数据
            if (AllEnemyDic != null && AllEnemyDic.Count > 0)
            {
                var mTemp = AllEnemyDic.Values.ToList();
                for (int i = 0, len = mTemp.Count; i < len; i++)
                {
                    mTemp[i].Dispose();
                }
                AllEnemyDic.Clear();
            }
            if (EnemyDataDic != null && EnemyDataDic.Count > 0)
            {
                EnemyDataDic.Clear();
            }

            base.Dispose();
        }
    }
}