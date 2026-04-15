using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ETModel
{
    /// <summary>
    /// 
    /// </summary>
    public class GridHelp
    {
        /// <summary>
        /// 计算两点间经过的格子
        /// </summary>
        /// <param name="from">起始点</param>
        /// <param name="to">目标点</param>
        /// <returns></returns>
        public static List<Vector2Int> GetTouchedPosBetweenTwoPoint(AstarNode from, AstarNode to)
        {
            Vector2Int vectorfrom = new Vector2Int(from.x,from.z);
            Vector2Int vectorto = new Vector2Int(to.x,to.z);
            List<Vector2Int> touchedGrid = GetTouchedPosBetweenOrigin2Target(vectorto - vectorfrom);
            touchedGrid.Offset(vectorfrom);
            return touchedGrid;
        }
      

        static List<Vector2Int> GetTouchedPosBetweenOrigin2Target(Vector2Int target)
        {
         List<Vector2Int> touched = new List<Vector2Int>();
            bool steep = Mathf.Abs(target.y) > Mathf.Abs(target.x);
            int x = steep ? target.y : target.x;
            int y = steep ? target.x : target.y;

            //斜率
            float tangent = (float)y / x;

            float delta = x > 0 ? .5f : -.5f;

            for (int i = 1; i < 2 * Mathf.Abs(x); i++)
            {
                float tempX = i * delta;
                float tempY = tangent * delta;
                bool isOnEdge = Mathf.Abs(tempY - Mathf.FloorToInt(tempY)) != 0.0f;
                //偶数 格子内部判断
                if ((i & 1) == 0)
                {
                    
                    //在边缘 则上下两个格子都满足条件
                    if (isOnEdge)
                    {
                        touched.AddUnique(new Vector2Int(Mathf.RoundToInt(tempX), Mathf.CeilToInt(tempY)));
                        touched.AddUnique(new Vector2Int(Mathf.RoundToInt(tempX), Mathf.FloorToInt(tempY)));
                    }
                    //不在边缘 就所处格子满足条件
                    else
                    {
                        touched.AddUnique(new Vector2Int(Mathf.RoundToInt(tempX), Mathf.RoundToInt(tempY)));
                    }
                }
                //奇数 格子边缘 判断
                else
                {
                    //在格子 交点处 不视为障碍 忽略
                    if (isOnEdge)
                    {
                        continue;
                       
                    }
                    else 
                    {
                        //否则 左右两个格子满足
                        touched.AddUnique(new Vector2Int(Mathf.CeilToInt(tempX), Mathf.RoundToInt(tempY)));
                        touched.AddUnique(new Vector2Int(Mathf.FloorToInt(tempX), Mathf.RoundToInt(tempY)));

                    }
                }
            }

            if (steep)
            {
              
                //镜像翻转 交换 x y
                for (int i = 0; i < touched.Count; i++)
                {

                    Vector2Int v = touched[i];
                    v.x = v.x ^ v.y;
                    v.y = v.x ^ v.y;
                    v.x = v.x ^ v.y;
                    touched[i] = v;
                }
            
            }
            touched.Except(new List<Vector2Int>() { Vector2Int.zero, target });

            return touched;
        }
    }
}