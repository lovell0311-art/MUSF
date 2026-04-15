
using UnityEngine;
using System;

namespace ETModel
{
    public partial class FindPathComponent
    {
        /// <summary>
        /// 获取目标点附近最近且不为障碍物的点 如果为null,则附近的点都是障碍物
        /// </summary>
        /// <param name="b_CurrentPoint">查找的点</param>
        /// <param name="b_TargetPos">目标点</param>
        /// <returns>附近的点</returns>
        public C_FindTheWay2D GetNearByPoint(C_FindTheWay2D b_CurrentPoint, C_FindTheWay2D b_TargetPos)
        {
            int max = _MapHightMax;
            if (max < _MapWightMax)
                max = _MapWightMax;
#if !SERVER
            float distance = Mathf.Abs(b_TargetPos.X - b_CurrentPoint.X) + Mathf.Abs(b_TargetPos.Y - b_CurrentPoint.Y);
#else
            float distance = MathF.Abs(b_TargetPos.X - b_CurrentPoint.X) + MathF.Abs(b_TargetPos.Y - b_CurrentPoint.Y);
#endif
            C_FindTheWay2D result = null;
            // 应该判断距离 距离近者优先
            void CompareH(C_FindTheWay2D findTheWay)
            {
                if (findTheWay.IsObstacle == false)
                {
#if !SERVER
                    float h = Mathf.Abs(b_TargetPos.X - findTheWay.X) + Mathf.Abs(b_TargetPos.Y - findTheWay.Y);
#else
                    float h = MathF.Abs(b_TargetPos.X - findTheWay.X) + MathF.Abs(b_TargetPos.Y - findTheWay.Y);
#endif
                    if (distance > h)
                    {
                        distance = h;
                        result = findTheWay;
                    }
                }
            }
            for (int i = 1; i < max; i++)
            {
                int left = b_CurrentPoint.X - i;
                int right = b_CurrentPoint.X + i;
                int up = b_CurrentPoint.Y + i;
                int down = b_CurrentPoint.Y - i;
                // 上下左右
                if (left >= _MapWightMin)
                {
                    C_FindTheWay2D findTheWay = _FindTheWayDic[left, b_CurrentPoint.Y];
                    CompareH(findTheWay);
                }
                if (up < _MapHightMax)
                {
                    C_FindTheWay2D findTheWay = _FindTheWayDic[b_CurrentPoint.X, up];
                    CompareH(findTheWay);
                }
                if (right < _MapWightMax)
                {
                    C_FindTheWay2D findTheWay = _FindTheWayDic[right, b_CurrentPoint.Y];
                    CompareH(findTheWay);
                }
                if (down >= _MapHightMin)
                {
                    C_FindTheWay2D findTheWay = _FindTheWayDic[b_CurrentPoint.X, down];
                    CompareH(findTheWay);
                }

                if (result != null)
                {
                    return result;
                }

                for (int x = 1; x < i; x++)
                {
                    int b = b_CurrentPoint.Y + x;
                    int viceb = b_CurrentPoint.Y - x;
                    if (left >= _MapWightMin)
                    {
                        if (b < _MapHightMax)
                        {
                            C_FindTheWay2D findTheWay = _FindTheWayDic[left, b];
                            CompareH(findTheWay);
                        }
                        if (viceb >= _MapHightMin)
                        {
                            C_FindTheWay2D findTheWay = _FindTheWayDic[left, viceb];
                            CompareH(findTheWay);
                        }
                    }
                    if (right < _MapWightMax)
                    {
                        if (b < _MapHightMax)
                        {
                            C_FindTheWay2D findTheWay = _FindTheWayDic[right, b];
                            CompareH(findTheWay);
                        }
                        if (viceb >= _MapHightMin)
                        {
                            C_FindTheWay2D findTheWay = _FindTheWayDic[right, viceb];
                            CompareH(findTheWay);
                        }
                    }

                    int a = b_CurrentPoint.X + x;
                    int vicea = b_CurrentPoint.X - x;
                    if (down >= _MapHightMin)
                    {
                        if (a < _MapWightMax)
                        {
                            C_FindTheWay2D findTheWay = _FindTheWayDic[a, down];
                            CompareH(findTheWay);
                        }
                        if (vicea >= _MapWightMin)
                        {
                            C_FindTheWay2D findTheWay = _FindTheWayDic[vicea, down];
                            CompareH(findTheWay);
                        }
                    }
                    if (up < _MapHightMax)
                    {
                        if (a < _MapWightMax)
                        {
                            C_FindTheWay2D findTheWay = _FindTheWayDic[a, up];
                            CompareH(findTheWay);
                        }
                        if (vicea >= _MapWightMin)
                        {
                            C_FindTheWay2D findTheWay = _FindTheWayDic[vicea, up];
                            CompareH(findTheWay);
                        }
                    }

                    if (result != null)
                    {
                        return result;
                    }
                }

                // 左上 左下 右上 右下
                if (left >= _MapWightMin)
                {
                    if (up < _MapHightMax)
                    {
                        C_FindTheWay2D findTheWay = _FindTheWayDic[left, up];
                        CompareH(findTheWay);
                    }
                    if (down >= _MapHightMin)
                    {
                        C_FindTheWay2D findTheWay = _FindTheWayDic[left, down];
                        CompareH(findTheWay);
                    }
                }
                if (right < _MapWightMax)
                {
                    if (up < _MapHightMax)
                    {
                        C_FindTheWay2D findTheWay = _FindTheWayDic[right, up];
                        CompareH(findTheWay);
                    }
                    if (down >= _MapHightMin)
                    {
                        C_FindTheWay2D findTheWay = _FindTheWayDic[right, down];
                        CompareH(findTheWay);
                    }
                }

                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
