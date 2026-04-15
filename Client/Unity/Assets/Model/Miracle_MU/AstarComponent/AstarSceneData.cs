using System.Collections.Generic;


namespace ETModel
{
    /// <summary>
    /// 地图的格子信息
    /// </summary>
    public class AstarSceneData
    {
        public string SceneName { get; set; }
        public int Width { get; set; }
        public int Hieght { get; set; }
        public AstarNode[][] Nodes { get; set; }

        /// <summary>
        /// 地图ID
        /// </summary>
        public int Id;


        /// <summary>
        /// key：区域X
        /// value: key:区域Y value:X_Y 的区域
        /// </summary>

        public Dictionary<int, Dictionary<int, MapCellFieldComponent>> MapCellFieldDic { get; set; } = new Dictionary<int, Dictionary<int, MapCellFieldComponent>>();

        /// <summary>
        /// 通过区域坐标 获取区域
        /// </summary>
        /// <param name="b_AreaPosX"></param>
        /// <param name="b_AreaPosY"></param>
        /// <returns></returns>
        public MapCellFieldComponent GetMapAreaByAreaPos(int b_AreaPosX, int b_AreaPosY)
        {
            if (MapCellFieldDic.TryGetValue(b_AreaPosX, out var mRowMapCellFieldDic))
            {
                if (mRowMapCellFieldDic.TryGetValue(b_AreaPosY, out var mCenterMapCellField))
                {
                    return mCenterMapCellField;
                }
            }
            return null;
        }
        /// <summary>
        /// 根据 格子坐标 获取格子所在区域
        /// </summary>
        /// <param name="b_X"></param>
        /// <param name="b_Y"></param>
        /// <returns></returns>
        public MapCellFieldComponent GetMapCellFieldByPos(int b_X, int b_Y)
        {
            if (b_X < 0) return null;
            if (b_Y < 0) return null;
            if (b_X >= Width) return null;
            if (b_Y >= Hieght) return null;

            var mFindTheWay = Nodes[b_X][b_Y];
            if (mFindTheWay != null)
            {
                return GetMapAreaByAreaPos(mFindTheWay.AreaPosX, mFindTheWay.AreaPosY);
            }
            return null;
        }


      
    }


    public class MapCellFieldComponent
    {
        public long Id { get; set; } = IdGenerater.GenerateId();

        /// <summary>
        /// 当前区域 ID
        /// </summary>
        public int AreaIndex { get; set; }
        /// <summary>
        /// 区域 X
        /// </summary>
        public int AreaPosX { get; set; }
        /// <summary>
        /// 区域 Y
        /// </summary>
        public int AreaPosY { get; set; }
        /// <summary>
        /// 当前区域 附近的八个区域和当前区域
        /// </summary>

        public Dictionary<int, MapCellFieldComponent> AroundFieldDic { get; set; }

        /// <summary>
        /// 周围区域ID 集合
        /// </summary>
        public List<int> AroundField { get; set; }

        /// <summary>
        /// 周围的区域范围
        /// </summary>
        public MapCellFieldComponent[] AroundFieldArray { get; set; }

        /// <summary>
        /// 当前区域 新加实体
        /// </summary>
        /// <param name="unitEntity"></param>
        public void Add(long unitEntityId)
        {
            //缓存全部实体
            if(AstarComponent.Instance.AreaAdd!=null)
           AstarComponent.Instance.AreaAdd.Invoke(AreaIndex,unitEntityId);
        }
        /// <summary>
        /// 移除当前区域 的实体
        /// </summary>
        /// <param name="unitEntity"></param>
        public void Remove(long unitEntityId)
        {
            if (AstarComponent.Instance.AreaRemove!=null)
           AstarComponent.Instance.AreaRemove.Invoke(AreaIndex, unitEntityId);

        }
      
    }
}
