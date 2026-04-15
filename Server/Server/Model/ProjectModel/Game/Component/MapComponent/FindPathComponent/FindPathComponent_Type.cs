
using CustomFrameWork;
using UnityEngine;
using System.Collections.Generic;

namespace ETModel
{
    /// <summary>
    /// 寻路 2d 数据体
    /// </summary>
    [PrivateObject]
    public class C_FindTheWay2D : ADataContext<int, int, Vector3>
    {
        public void ClearMapCacheData()
        {
            Map = null;
            MapField = null;
            if (FieldPlayerDic.Count > 0) FieldPlayerDic.Clear();
            if (FieldEnemyDic.Count > 0) FieldEnemyDic.Clear();
            if (FieldSummonedDic.Count > 0) FieldSummonedDic.Clear();
            if (FieldPetsDic.Count > 0) FieldPetsDic.Clear();

            AreaIndex = default;
            AreaPosX = default;
            AreaPosY = default;
            X = default;
            Y = default;

            IsStaticObstacle = false;
            obstacleCount = 0;
            IsSafeArea = false;
            TransferPoint = default;
        }

        public long Id { get; private set; }
        public MapComponent Map { get; set; }

        public MapCellAreaComponent MapField { get; set; }

        public Dictionary<long, GamePlayer> FieldPlayerDic { get; private set; } = new Dictionary<long, GamePlayer>();
        public Dictionary<long, Enemy> FieldEnemyDic { get; private set; } = new Dictionary<long, Enemy>();
        public Dictionary<long, Summoned> FieldSummonedDic { get; private set; } = new Dictionary<long, Summoned>();
        public Dictionary<long, Pets> FieldPetsDic { get; private set; } = new Dictionary<long, Pets>();

        public bool HasUnit()
        {
            if (FieldPlayerDic.Count > 0 ||
                FieldEnemyDic.Count > 0 ||
                FieldPetsDic.Count > 0 ||
                FieldSummonedDic.Count > 0)
            {
                return true;
            }
            return false;
        }


        public int AreaIndex { get; set; }
        public int AreaPosX { get; set; }
        public int AreaPosY { get; set; }


        public bool IsSafeArea { get; set; }
        public int TransferPoint { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        /// <summary> 格子中存在的障碍物数量 </summary>
        public int obstacleCount = 0;
        /// <summary>
        /// 是不是障碍物
        /// </summary>
        public bool IsObstacle
        {
            get
            {
                if (IsStaticObstacle) return true;
                return obstacleCount > 0;
            }
        }
        /// <summary>
        /// 是不是障碍物
        /// </summary>
        public bool IsStaticObstacle { get; set; }
        /// <summary>
        /// 当前位置的坐标
        /// </summary>
        public Vector3 Position { get; private set; }
        public Vector3 Vector3Pos { get; private set; }
        public Vector2 Vector2Pos { get; private set; }
        public override void ContextAwake(int b_PointX, int b_PointY, Vector3 b_Position)
        {
            if (Id == 0) Id = Help_UniqueValueHelper.GetServerUniqueValue();

            this.X = b_PointX;
            this.Y = b_PointY;
            this.Position = b_Position;
            Vector2Pos = new Vector2(this.X, this.Y);
            obstacleCount = 0;
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            Id = default;

            obstacleCount = 0;
            Position = default;
            Vector2Pos = default;

            ClearMapCacheData();
            base.Dispose();
        }
    }

    /// <summary>
    /// 寻路时产生的数据
    /// </summary>
    public class C_FindTheWay2D_Data : ADataContext<int, int>
    {
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// 寻找的路线 上一个节点
        /// </summary>
        public C_FindTheWay2D_Data ParentPoint { get; set; }

        /// <summary>
        /// 开始点到当前方块的移动代价
        /// </summary>
        public float G { get; set; }
        /// <summary>
        /// 当前方块到结束点的预估移动代价
        /// </summary>
        public float H { get; set; }
        /// <summary>
        /// 方块的总移动代价
        /// </summary>
        public float F { get; set; }

        public override void ContextAwake(int b_PointX, int b_PointY)
        {
            this.X = b_PointX;
            this.Y = b_PointY;
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            ParentPoint = null;
            G = default;
            H = default;
            F = default;

            X = default;
            Y = default;
            base.Dispose();
        }

        /// <summary>
        /// 更新上一个节点 与 开始点到当前方块的移动代价
        /// </summary>
        /// <param name="b_ParentPoint"></param>
        /// <param name="b_G"></param>
        public void UpdateParentPoint(C_FindTheWay2D_Data b_ParentPoint, float b_G)
        {
            this.ParentPoint = b_ParentPoint;
            this.G = b_G;
            F = G + H;
        }
    }
}
