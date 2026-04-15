using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{

    public static class ETreasureType
    {
        /// <summary> ≤ÿ±¶Õº </summary>
        public const int TreasureMap = 1;
        /// <summary> ¡˙Õı±¶≤ÿ </summary>
        public const int LongWangBaoZang = 2;
        /// <summary> ≥‡—Ê ﬁ∑ıªØµ∞ </summary>
        public const int FuHuaDan = 3;
        /// <summary> ƒ…Ω‰ </summary>
        public const int NaJie = 4;
        /// <summary> –°ÃÏ π </summary>
        public const int XiaoTianShi = 5;
    }

    public class TreasurePoint
    {
        public long Id;
        public int TreasureType;
        public int MapId;
        public int NpcConfigId;
        public int PosX;
        public int PosY;
    }


    [ObjectSystem]
    public class TreasureMapComponentAwake : AwakeSystem<TreasureMapComponent>
    {
        public override void Awake(TreasureMapComponent self)
        {
            TreasureMapComponent.Instance = self;
        }
    }

    [ObjectSystem]
    public class TreasureMapComponentDestroy : DestroySystem<TreasureMapComponent>
    {
        public override void Destroy(TreasureMapComponent self)
        {
            self.Clear();
            TreasureMapComponent.Instance = null;
        }
    }


    public class TreasureMapComponent : Component
    {
        public static TreasureMapComponent Instance;

        public Dictionary<long, TreasurePoint> AllPoint = new Dictionary<long, TreasurePoint>();

        public void Clear()
        {
            AllPoint.Clear();
        }

    }
}