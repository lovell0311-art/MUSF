
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomFrameWork
{

    public class MathfHelper
    {
        /// <summary>
        /// ----------原理----------
        /// 注意到如果从P作水平向左的射线的话，如果P在多边形内部，那么这条射线与多边形的交点必为奇数，
        /// 如果P在多边形外部，则交点个数必为偶数(0也在内)。
        /// </summary>
        /// <param name="checkPoint"></param>
        /// <param name="b_PolygonPoints"></param>
        /// <returns></returns>
        public static bool IsInPolygon(Vector3 checkPoint, List<Vector2> b_PolygonPoints)
        {
            int nCross = 0, nCross2 = 0;

            int mPointCount = b_PolygonPoints.Count;

            // (1)如果射线正好穿过P1或者P2,那么这个交点会被算作2次，处理办法是如果P的从坐标与P1,P2中较小的纵坐标相同，则直接忽略这种情况
            // (2)如果射线水平，则射线要么与其无交点，要么有无数个，这种情况也直接忽略。
            // (3)如果射线竖直，而P0的横坐标小于P1,P2的横坐标，则必然相交。
            // (4)再判断相交之前，先判断P是否在边(P1, P2)的上面，如果在，则直接得出结论：P再多边形内部。

            Vector2 mlineStartPos, mlineEndPos;
            //第一个点和最后一个点作为第一条线，之后是第一个点和第二个点作为第二条线，之后是第二个点与第三个点，第三个点与第四个点...
            for (int i = 0, j = mPointCount - 1; i < mPointCount; j = i, i++)
            {
                mlineStartPos = b_PolygonPoints[i];
                mlineEndPos = b_PolygonPoints[j];

                // p1p2 与 x轴平行
                if (mlineStartPos.y == mlineEndPos.y) continue;

                var mMinY = Mathf.Min(mlineStartPos.y, mlineEndPos.y);
                var mMaxY = Mathf.Max(mlineStartPos.y, mlineEndPos.y);
                // 交点在p1p2延长线上
                if (checkPoint.y < mMinY) continue;
                if (checkPoint.y > mMaxY) continue;
                if (checkPoint.y == mMinY)
                {
                    continue;
                }
                // 点和线段最高点重合 视为相交
                if (checkPoint.y == mMaxY)
                {
                    // 斜率为正
                    var mSlope = (mlineEndPos.y - mlineStartPos.y) / (mlineEndPos.x - mlineStartPos.x);
                    if (mSlope > 0)
                    {
                        if (checkPoint.x <= Mathf.Max(mlineStartPos.x, mlineEndPos.x))
                            nCross++; // 只统计单边交点
                    }
                    else if (mSlope < 0)
                    {// 斜率为负
                        if (checkPoint.x <= Mathf.Min(mlineStartPos.x, mlineEndPos.x))
                            nCross++; // 只统计单边交点
                    }
                    continue;
                }

                // 根据斜率判断,从射线起点到线段的两个端点分别求斜率,在此假设设得到的斜率分别为a1、a2,
                // 如果线段起止点分别在1,2区间, 则可以相交的斜率范围为（负无穷大,a2）并（a1,正无穷大）；
                // 如果不是上面的情况,相交的斜率范围是（a1,a2）,所谓“判断”即是看看α是不是在这个范围内,如果在,一定能够相交.

                // 求交点：判断相交后,点一定在线段上,已知了线段的起始点这两个点,可以把线段所在的直线求出,知道了射线起点和射线斜率α ,
                // 射线所在的直线也可以求出,问题就转化为了求解两条直线的交点,解个二元一次方程组即可

                // 相似三角形
                // x / (mlineEndPos.y - checkPoint.y) ## (mlineEndPos.x - mlineStartPos.x) / (mlineEndPos.y - mlineStartPos.y);
                // 求宽x  mlineEndPos.x - x = 交点x
                // x  ## (mlineEndPos.y - checkPoint.y) * (mlineEndPos.x - mlineStartPos.x) / (mlineEndPos.y - mlineStartPos.y);

                //var widthX = (mlineEndPos.y - checkPoint.y) * (mlineEndPos.x - mlineStartPos.x) / (mlineEndPos.y - mlineStartPos.y);

                //if (mlineEndPos.x - widthX > checkPoint.x)
                //{
                //    nCross++; // 只统计单边交点 
                //}

                var mLeft = (mlineEndPos.y - checkPoint.y) * (mlineEndPos.x - mlineStartPos.x);
                var mRight = (mlineEndPos.x - checkPoint.x) * (mlineEndPos.y - mlineStartPos.y);
                if (mLeft > mRight)
                {
                    nCross++; // 只统计单边交点 
                }
            }

            // 单边交点为偶数，点在多边形之外 ---
            return (nCross % 2 == 1);
        }


        public static bool IsInPolygon2(Vector3 checkPoint, List<Vector2> b_PolygonPoints)
        {
            System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
            myGraphicsPath.Reset();

            //添加构建多边形的点
            List<PointF> mPointlist = new List<PointF>();
            for (int i = 0, len = b_PolygonPoints.Count; i < len; i++)
            {
                var mTemp = b_PolygonPoints[i];
                mPointlist.Add(new PointF(mTemp.x, mTemp.y));
            }
            myGraphicsPath.AddPolygon(mPointlist.ToArray());

            Region myRegion = new Region();
            myRegion.MakeEmpty();
            myRegion.Union(myGraphicsPath);

            //返回判断点是否在多边形里
            bool myPoint = false;
            myPoint = myRegion.IsVisible(new Point((int)checkPoint.x, (int)checkPoint.y));

            myGraphicsPath.Dispose();
            myRegion.Dispose();
            return myPoint;
        }
    }
}
