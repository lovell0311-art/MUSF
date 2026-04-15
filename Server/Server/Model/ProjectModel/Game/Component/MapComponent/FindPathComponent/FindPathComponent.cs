using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public partial class FindPathComponent : TCustomComponent<MapComponent>
    {
        /// <summary>
        /// 是否允许斜着行走
        /// </summary>
        private bool _InclineWalk;

        private int _MapWightMin;
        private int _MapHightMin;
        private int _MapWightMax;
        private int _MapHightMax;
        /// <summary>
        /// 生成的地形信息
        /// </summary>
        //private readonly Dictionary<int, Dictionary<int, C_FindTheWay2D>> _FindTheWayDic = new Dictionary<int, Dictionary<int, C_FindTheWay2D>>();
        private C_FindTheWay2D[,] _FindTheWayDic;
        private C_FindTheWay2D_Data[,] mFindTheWayDicData;

        HashSet<C_FindTheWay2D_Data> mOpenPosList = new HashSet<C_FindTheWay2D_Data>();
        HashSet<C_FindTheWay2D> mClosePosList = new HashSet<C_FindTheWay2D>();

        List<C_FindTheWay2D> mAroundPoints = new List<C_FindTheWay2D>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            _MapWightMin = 0;
            _MapHightMin = 0;
            _MapWightMax = 0;
            _MapHightMax = 0;

            FindTheWayDicClear();
            FindTheWayDicDataClear();
            mAroundPoints.Clear();

            base.Dispose();
        }

        void FindTheWayDicDataClear()
        {
            if (_FindTheWayDic != null && mFindTheWayDicData.Length > 0)
            {
                for (int i = 0, len = mFindTheWayDicData.GetLength(0); i < len; i++)
                {
                    for (int j = 0, jlen = mFindTheWayDicData.GetLength(1); j < jlen; j++)
                    {
                        mFindTheWayDicData[i, j].Dispose();
                    }
                }
                mFindTheWayDicData = null;
            }
        }

        void FindTheWayDicClear()
        {
            if (_FindTheWayDic != null && _FindTheWayDic.Length > 0)
            {
                for (int i = 0, len = _FindTheWayDic.GetLength(0); i < len; i++)
                {
                    for (int j = 0, jlen = _FindTheWayDic.GetLength(1); j < jlen; j++)
                    {
                        if(_FindTheWayDic[i, j] != null)
                            _FindTheWayDic[i, j].Dispose();
                    }
                }
                _FindTheWayDic = null;
            }
        }



        /// <summary>
        /// 初始化 以左下点向左向上延伸
        /// </summary>
        /// <param name="b_MapWightMax">x轴最大</param>
        /// <param name="b_MapHightMax">y轴最大</param>
        /// <param name="b_LeftDownPoint">左下点</param>
        /// <param name="b_XMinLength">单位长度 每一个单位与下一个单位之间的距离</param>
        /// <param name="b_YMinLength">单位长度 每一个单位与下一个单位之间的距离</param>
        /// <param name="b_InclineWalk">是否允许斜着行走</param>
        /// <returns>点</returns>
        public C_FindTheWay2D[,] PathFindingLeftUpInit(
            int b_MapWightMax, int b_MapHightMax, Vector3 b_LeftDownPoint, float b_XMinLength, float b_YMinLength, bool b_InclineWalk)
        {
            this._InclineWalk = b_InclineWalk;
            this._MapWightMin = 0;
            this._MapHightMin = 0;
            this._MapWightMax = b_MapWightMax;
            this._MapHightMax = b_MapHightMax;

            FindTheWayDicClear();
            _FindTheWayDic = new C_FindTheWay2D[this._MapWightMax, this._MapHightMax];
            mFindTheWayDicData = new C_FindTheWay2D_Data[this._MapWightMax, this._MapHightMax];
            for (int i = 0; i < b_MapWightMax; i++)
            {
                for (int j = 0; j < b_MapHightMax; j++)
                {
                    _FindTheWayDic[i, j] = CustomFrameWork.Root.CreateBuilder.GetInstance<C_FindTheWay2D, int, int, Vector3>
                        (i, j, b_LeftDownPoint + new Vector3(i * b_XMinLength, j * b_YMinLength, 0));
                }
            }
            return _FindTheWayDic;
        }
        /// <summary>
        /// 初始化 以中心点向左向上延伸
        /// </summary>
        /// <param name="b_MapWightMax">x轴最大</param>
        /// <param name="b_MapHightMax">y轴最大</param>
        /// <param name="b_CenterPoint">中心点</param>
        /// <param name="b_XMinLength">最小x轴长度 每一个单位与下一个单位之间的距离</param>
        /// <param name="b_YMinLength">最小y轴长度 每一个单位与下一个单位之间的距离</param>
        /// <param name="b_InclineWalk">是否允许斜着行走</param>
        /// <returns>点</returns>
        public C_FindTheWay2D[,] PathFindingCenterInit(
            int b_MapWightMax, int b_MapHightMax, Vector3 b_CenterPoint, float b_XMinLength, float b_YMinLength, bool b_InclineWalk)
        {
            this._InclineWalk = b_InclineWalk;
            this._MapWightMin = 0;
            this._MapHightMin = 0;
            this._MapWightMax = b_MapWightMax;
            this._MapHightMax = b_MapHightMax;
            int mapwight = (b_MapWightMax) / 2;
            int maphight = (b_MapHightMax) / 2;

            FindTheWayDicClear();
            _FindTheWayDic = new C_FindTheWay2D[this._MapWightMax, this._MapHightMax];
            mFindTheWayDicData = new C_FindTheWay2D_Data[this._MapWightMax, this._MapHightMax];
            for (int i = 0; i < b_MapWightMax; i++)
            {
                for (int j = 0; j < b_MapHightMax; j++)
                {
                    _FindTheWayDic[i, j] = CustomFrameWork.Root.CreateBuilder.GetInstance<C_FindTheWay2D, int, int, Vector3>
                        (i, j, b_CenterPoint + new Vector3((i - mapwight) * b_XMinLength, (j - maphight) * b_YMinLength, 0));
                }
            }
            return _FindTheWayDic;
        }
        /// <summary>
        /// 如果返回null,说明没找到路径 反之,则找到路径
        /// </summary>
        /// <param name="b_StartPos">开始点</param>
        /// <param name="b_TargetPosTemp">结束点</param>
        /// <returns>路径</returns>
        public List<C_FindTheWay2D> AStarFindTheWay(C_FindTheWay2D b_StartPos, C_FindTheWay2D b_TargetPosTemp, int b_OffsetX = 0, int b_OffsetY = 0)
        {
            {
                GetAroundPoint(b_StartPos, ref mAroundPoints);
                if (mAroundPoints.Count == 0)
                {
                    return null;
                }
            }
            C_FindTheWay2D mNewTargetPos = b_TargetPosTemp;

            // 当前数据缓存 创建数据 清理数据
            C_FindTheWay2D_Data Create_FindData(C_FindTheWay2D b_FindTheWay2D)
            {
                C_FindTheWay2D_Data mData = mFindTheWayDicData[b_FindTheWay2D.X, b_FindTheWay2D.Y];
                if (mData == null)
                {
                    mData = CustomFrameWork.Root.CreateBuilder.GetInstance<C_FindTheWay2D_Data, int, int>(b_FindTheWay2D.X, b_FindTheWay2D.Y);
                    mFindTheWayDicData[b_FindTheWay2D.X, b_FindTheWay2D.Y] = mData;
                }
                return mData;
            }
            List<C_FindTheWay2D_Data> Create_FindDatas(List<C_FindTheWay2D> b_FindTheWay2Ds)
            {
                List<C_FindTheWay2D_Data> mResult = null;
                if (b_FindTheWay2Ds.Count > 0)
                {
                    mResult = new List<C_FindTheWay2D_Data>();
                }
                for (int i = 0, len = b_FindTheWay2Ds.Count; i < len; i++)
                {
                    var mFindTheWay2D = b_FindTheWay2Ds[i];
                    mResult.Add(Create_FindData(mFindTheWay2D));
                }
                return mResult;
            }



            var mStartPosData = Create_FindData(b_StartPos);
            mStartPosData.ParentPoint = null;

            mOpenPosList.Add(mStartPosData);

            var mTargetPosData = Create_FindData(mNewTargetPos);

            bool mIsSuccess = false;

            int findCount = 40;
            while (mOpenPosList.Count > 0 && findCount > 0)
            {
                --findCount;
                C_FindTheWay2D mCurrentPoint = FindMinPoint(mOpenPosList);
                var mCurrentPointData = Create_FindData(mCurrentPoint);

                mOpenPosList.Remove(mCurrentPointData);
                mClosePosList.Add(mCurrentPoint);
                GetAroundPoint(mCurrentPoint,ref mAroundPoints);
                if (mAroundPoints.Count == 0) continue;
                PointInspectFilter(mAroundPoints, mClosePosList);
                if (mAroundPoints.Count == 0) continue;

                for (int i = 0; i < mAroundPoints.Count; i++)
                {
                    // 拿出一个点 从要计算的附近点里面
                    C_FindTheWay2D mAroundPoint = mAroundPoints[i];
                    if (mNewTargetPos != mAroundPoint && (Math.Abs(mAroundPoint.X - b_StartPos.X) > b_OffsetX || Math.Abs(mAroundPoint.Y - b_StartPos.Y) > b_OffsetY))
                    {
                        continue;
                    }

                    var mAroundPointData = Create_FindData(mAroundPoint);
                    if (mOpenPosList.Contains(mAroundPointData)==true)
                    {
                        // 计算移动代价
                        float newG = CalculationG(mAroundPointData, mCurrentPointData);
                        if (newG < mAroundPointData.G)
                        {
                            // 如果从这里走 消耗小的话
                            mAroundPointData.UpdateParentPoint(mCurrentPointData, newG);
                        }
                    }
                    else
                    {
                        mAroundPointData.ParentPoint = mCurrentPointData;
                        CalculationFGH(mAroundPointData, mTargetPosData);
                        mOpenPosList.Add(mAroundPointData);
                    }
                }
                // 如果目标点找到了
                if (mOpenPosList.Contains(mTargetPosData) == true)
                {
                    mIsSuccess = true;
                    break;
                }
            }
            if(findCount <= 0 && mOpenPosList.Count > 0)
            {
                var targetPos = mOpenPosList.First();
                foreach (var v in mOpenPosList)
                {
                    if (targetPos.H > v.H)
                    {
                        targetPos = v;
                    }
                }
                mTargetPosData = targetPos;
                mIsSuccess = true;
            }
            if (mIsSuccess)
            {// 逆向读取列表
                List<C_FindTheWay2D> mResults = new List<C_FindTheWay2D>();
                var mFindTheWayData = mTargetPosData.ParentPoint;
                mResults.Add(_FindTheWayDic[mTargetPosData.X, mTargetPosData.Y]);
                while (mFindTheWayData != null)
                {
                    mResults.Add(_FindTheWayDic[mFindTheWayData.X, mFindTheWayData.Y]);
                    mFindTheWayData = mFindTheWayData.ParentPoint;
                }
                mOpenPosList.Clear();
                mClosePosList.Clear();
                // 逆向的逆向 正向列表
                mResults.Reverse();
                return mResults;
            }
            else
            {
                mOpenPosList.Clear();
                mClosePosList.Clear();
                return null;
            }
        }

        /// <summary>
        /// 点检查器 检查源里面有没有已经排除的点 如果有则移除
        /// </summary>
        /// <param name="b_Source"></param>
        /// <param name="b_ClosePoints"></param>
        private void PointInspectFilter(List<C_FindTheWay2D> b_Source, HashSet<C_FindTheWay2D> b_ClosePoints)
        {
            for(int i = b_Source.Count - 1;i>=0;--i)
            {
                if (b_ClosePoints.Contains(b_Source[i]))
                {
                    b_Source.RemoveAt(i);
                }

            }
        }
        /// <summary>
        /// 获取F值最小的点 方块的总移动代价
        /// </summary>
        /// <param name="b_OpenLists">集合</param>
        /// <returns>点</returns>
        private C_FindTheWay2D FindMinPoint(HashSet<C_FindTheWay2D_Data> b_OpenLists)
        {
            float mPointMinF = float.MaxValue;

            C_FindTheWay2D mResult = null;
            foreach (var mTemp in b_OpenLists)
            {
                //C_FindTheWay2D_Data mTemp = b_OpenLists[i];
                if (mTemp.F < mPointMinF)
                {
                    mResult = _FindTheWayDic[mTemp.X, mTemp.Y];
                    mPointMinF = mTemp.F;
                }
            }
            return mResult;
        }
        /// <summary>
        /// 获取目标点的八个方向上的点
        /// </summary>
        /// <param name="b_CurrentPoint">点</param>
        /// <returns>点</returns>
        private void GetAroundPoint(C_FindTheWay2D b_CurrentPoint,ref List<C_FindTheWay2D> mAroundWays)
        {
            mAroundWays.Clear();
            C_FindTheWay2D mUpPoint = null, mDownPoint = null, mLeftPoint = null, mRightPoint = null;
            C_FindTheWay2D mUpLeftPoint, mUpRightPoint, mDownLeftPoint, mDownRightPoint;
            int mUpPointIndex = b_CurrentPoint.Y + 1;
            int mRightPointIndex = b_CurrentPoint.X + 1;
            if (mUpPointIndex < _MapHightMax)
            {
                mUpPoint = _FindTheWayDic[b_CurrentPoint.X, mUpPointIndex];
                if (mUpPoint.IsObstacle == false)
                    mAroundWays.Add(mUpPoint);
            }
            if (b_CurrentPoint.Y > 0)
            {
                mDownPoint = _FindTheWayDic[b_CurrentPoint.X, b_CurrentPoint.Y - 1];
                if (mDownPoint.IsObstacle == false)
                    mAroundWays.Add(mDownPoint);
            }
            if (b_CurrentPoint.X > 0)
            {
                mLeftPoint = _FindTheWayDic[b_CurrentPoint.X - 1, b_CurrentPoint.Y];
                if (mLeftPoint.IsObstacle == false)
                    mAroundWays.Add(mLeftPoint);
            }
            if (mRightPointIndex < _MapWightMax)
            {
                mRightPoint = _FindTheWayDic[mRightPointIndex, b_CurrentPoint.Y];
                if (mRightPoint.IsObstacle == false)
                    mAroundWays.Add(mRightPoint);
            }
            if (_InclineWalk)
            {
                if (mUpPoint != null && mLeftPoint != null && mLeftPoint.IsObstacle == false && mUpPoint.IsObstacle == false)
                {
                    mUpLeftPoint = _FindTheWayDic[b_CurrentPoint.X - 1, b_CurrentPoint.Y + 1];
                    if (mUpLeftPoint.IsObstacle == false)
                        mAroundWays.Add(mUpLeftPoint);
                }
                if (mUpPoint != null && mRightPoint != null && mUpPoint.IsObstacle == false && mRightPoint.IsObstacle == false)
                {
                    mUpRightPoint = _FindTheWayDic[b_CurrentPoint.X + 1, b_CurrentPoint.Y + 1];
                    if (mUpRightPoint.IsObstacle == false)
                        mAroundWays.Add(mUpRightPoint);
                }
                if (mDownPoint != null && mRightPoint != null && mDownPoint.IsObstacle == false && mRightPoint.IsObstacle == false)
                {
                    mDownRightPoint = _FindTheWayDic[b_CurrentPoint.X + 1, b_CurrentPoint.Y - 1];
                    if (mDownRightPoint.IsObstacle == false)
                        mAroundWays.Add(mDownRightPoint);
                }
                if (mDownPoint != null && mLeftPoint != null && mDownPoint.IsObstacle == false && mLeftPoint.IsObstacle == false)
                {
                    mDownLeftPoint = _FindTheWayDic[b_CurrentPoint.X - 1, b_CurrentPoint.Y - 1];
                    if (mDownLeftPoint.IsObstacle == false)
                        mAroundWays.Add(mDownLeftPoint);
                }
            }
            //return mAroundWays;
        }
        /// <summary>
        /// 计算当前点的移动代价
        /// </summary>
        /// <param name="b_CurrentPos">当前点</param>
        /// <param name="b_TargetPos">目标点</param>
        private void CalculationFGH(C_FindTheWay2D_Data b_CurrentPos, C_FindTheWay2D_Data b_TargetPos)
        {
#if !SERVER
            float h = Mathf.Abs(b_TargetPos.X - b_CurrentPos.X) + Mathf.Abs(b_TargetPos.Y - b_CurrentPos.Y);
#else
            float h = MathF.Abs(b_TargetPos.X - b_CurrentPos.X) + MathF.Abs(b_TargetPos.Y - b_CurrentPos.Y);
#endif

            float g = 0;
            if (b_CurrentPos.ParentPoint == null)
            {
                g = 0;
            }
            else
            {
                g = CalculationG(b_CurrentPos, b_CurrentPos.ParentPoint);
            }
            float f = g + h;
            b_CurrentPos.F = f;
            b_CurrentPos.G = g;
            b_CurrentPos.H = h;
        }
        /// <summary>
        /// 预估计算父节点到当前方块的移动代价
        /// </summary>
        /// <param name="b_CurrentPos">当前点</param>
        /// <param name="b_ParentPos">父节点</param>
        /// <returns>移动代价</returns>
        private float CalculationG(C_FindTheWay2D_Data b_CurrentPos, C_FindTheWay2D_Data b_ParentPos)
        {
            if (b_CurrentPos.X == b_ParentPos.X || b_ParentPos.Y == b_CurrentPos.Y)
            {
                return 10 + b_ParentPos.G;
            }
            else
            {
                return 14 + b_ParentPos.G;
            }
            //return Vector2.Distance(new Vector2(now.X, now.Y), new Vector2(parent.X, parent.Y)) + parent.G;
        }



    }
}