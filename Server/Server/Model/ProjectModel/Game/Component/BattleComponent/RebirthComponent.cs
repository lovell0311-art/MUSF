using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 复活组件
    /// </summary>
    [PrivateObject]
    public class RebirthComponent : TCustomComponent<CombatSource>
    {
        public MapComponent map;
        /// <summary> 地图实例id,使用对象池必须记录这个 </summary>
        public long mapInstanceId;

        public int deathPosX;
        public int deathPosY;

        public long timerId;

        /// <summary> 死亡时所在的地图 </summary>
        public MapComponent DeathMap
        {
            get
            {
                if (map == null) return null;
                if (map.Id != mapInstanceId) return null;
                return map;
            }
        }

        /// <summary> 死亡时所在的位置 </summary>
        public int DeathPosX { get { return deathPosX; } }
        /// <summary> 死亡时所在的位置 </summary>
        public int DeathPosY { get { return deathPosY; } }


    }
}
