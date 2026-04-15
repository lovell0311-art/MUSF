using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 背包工具类
    /// </summary>
    public static class KnapsackTools
    {

        private static readonly KnapsackGrid[][] Grids = new KnapsackGrid[UIKnapsackComponent.LENGTH_Knapsack_X][];
       
        static readonly int LENGTH_Knapsack_X = UIKnapsackComponent.LENGTH_Knapsack_X;
        static readonly int LENGTH_Knapsack_Y = UIKnapsackComponent.LENGTH_Knapsack_Y;

        ///静态构造方法在加载是就会调用 而且全局只会调用一次
        static KnapsackTools()
        {
          
            for (int i = 0; i < Grids.Length; i++)
            {
                Grids[i] = new KnapsackGrid[UIKnapsackComponent.LENGTH_Knapsack_Y];
            }
            for (int j = 0; j < UIKnapsackComponent.LENGTH_Knapsack_Y; j++)
            {
                for (int i = 0; i < UIKnapsackComponent.LENGTH_Knapsack_X; i++)
                {
                    Grids[i][j] = new KnapsackGrid() { isOccupy = false };
                }
            }
        }

        public static void Clean() 
        {
            for (int j = 0; j < LENGTH_Knapsack_Y; j++)
            {
                for (int i = 0; i < LENGTH_Knapsack_X; i++)
                {
                    Grids[i][j].isOccupy = false;
                }
            }
        }
        /// <summary>
        /// 添加或移除装备
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="isadd"></param>
        public static void AddEquip(KnapsackDataItem dataItem) 
        {
            var Point1 = new Vector2Int(dataItem.PosInBackpackX, dataItem.PosInBackpackY);
            var Point2 = new Vector2Int(dataItem.PosInBackpackX + dataItem.X - 1, dataItem.PosInBackpackY + dataItem.Y - 1);
            bool IsSinglePoint = Point1 == Point2;
            //Log.DebugGreen($"是否是单个武器：{IsSinglePoint}  {Point1} {Point2}");
            if (IsSinglePoint)
            {
                Grids[Point1.x][Point1.y].isOccupy = true;
            }
            else
            {
               //List<KnapsackGrid> gris = GetAreaGrids(Point1.x, Point1.y, Point2.x, Point2.y, Grids);
               List<Vector2> gris = GetAreaGrids(Point1.x, Point1.y, Point2.x, Point2.y, Grids);

                for (int i = 0, length=gris.Count; i < length; i++)
                {
                  //  gris[i].isOccupy = true;
                    Grids[(int)gris[i].x][(int)gris[i].y].isOccupy = true;
                }

            }
        }

        public static void RemoveEquip(KnapsackGridData data)
        {
          //  Log.DebugRed($"移除空间:{data.Grid_Type} {data.Point1.x}:{data.Point1.y}");
            if (data.IsSinglePoint)
            {
                Grids[data.Point1.x][data.Point1.y].isOccupy = false;
            }
            else
            {
              //  List<KnapsackGrid> gris = GetAreaGrids(data.Point1.x, data.Point1.y, data.Point2.x, data.Point2.y, Grids);
                List<Vector2> gris = GetAreaGrids(data.Point1.x, data.Point1.y, data.Point2.x, data.Point2.y, Grids);
                for (int i = 0, length = gris.Count; i < length; i++)
                {
                   // gris[i].isOccupy = false;
                    Grids[(int)gris[i].x][(int)gris[i].y].isOccupy = false;
                }
            }
        }

        /// <summary>
        /// 改变背包格子状态
        /// </summary>
        /// <param name="start_x">装备起始 x坐标</param>
        /// <param name="start_y"></param>
        /// <param name="end_x"></param>
        /// <param name="end_y"></param>
        /// <param name="isoccupy">是否占用</param>
        public static void ChangeGridsStates(int start_x,int start_y,int end_x,int end_y,bool isoccupy=true) 
        {
            return;
            if (isoccupy == false)
            {
                Log.DebugYellow($"拾取失败：{start_x}_{start_y}");
            }
            var Point1 = new Vector2Int(start_x, start_y);
            var Point2 = new Vector2Int(start_x + end_x, start_y + end_y);
            if (Point1 == Point2)
            {
                Grids[Point1.x][Point2.y].isOccupy = isoccupy;
            }
            else
            {
                List<Vector2> gris = GetAreaGrids(start_x, start_y, end_x, end_y, Grids);
                for (int i = 0, length = gris.Count; i < length; i++)
                {
                    // gris[i].isOccupy = false;
                    Grids[(int)gris[i].x][(int)gris[i].y].isOccupy = isoccupy;
                }
            }
        }

        //自动拾取时 获取当前装备 在背包中的放置位置
        public static Vector2Int GetKnapsackItemPos(int x,int y) 
        {
            var Point1 =Vector2.zero;
            var Point2 = new Vector2Int(x - 1, y - 1);
            bool IsSinglePoint = Point1 == Point2;

            //全部清空
            for (int j = 0; j < LENGTH_Knapsack_Y; j++)
            {
                for (int i = 0; i < LENGTH_Knapsack_X; i++)
                {
                    Grids[i][j].isOccupy = false;
                }
            }
            var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
            foreach (var item in list)
            {
                if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id)==false) continue;
                var Point1_ = new Vector2Int(item.PosInBackpackX, item.PosInBackpackY);
                var Point2_ = new Vector2Int(item.PosInBackpackX + item.X - 1, item.PosInBackpackY + item.Y - 1);
                bool IsSinglePoint_ = Point1_ == Point2_;
                //Log.DebugGreen($"是否是单个武器：{IsSinglePoint}  {Point1} {Point2}");
                if (IsSinglePoint_)
                {
                    Grids[Point1_.x][Point1_.y].isOccupy = true;
                }
                else
                {
                    //List<KnapsackGrid> gris = GetAreaGrids(Point1_.x, Point1_.y, Point2_.x, Point2_.y, Grids);
                    List<Vector2> gris = GetAreaGrids(Point1_.x, Point1_.y, Point2_.x, Point2_.y, Grids);

                    for (int i = 0, length = gris.Count; i < length; i++)
                    {
                        // gris[i].isOccupy = true;
                        Grids[(int)gris[i].x][(int)gris[i].y].isOccupy = true;
                    }

                }
            }

            //遍历加入，是否可以加入
            for (int j = 0; j < LENGTH_Knapsack_Y; j++)
            {
                for (int i = 0; i < LENGTH_Knapsack_X; i++)
                {
                    //单个物体
                    if (IsSinglePoint)
                    {
                        if (Grids[i][j].isOccupy == false)
                        {
                            Grids[i][j].isOccupy = true;
                            return new Vector2Int(i, j);
                        }
                    }
                    //范围性物体
                    else
                    {
                        //如果超出边缘直接过滤
                        if (!ContainGridObj(i, j, i + Point2.x, j + Point2.y, Grids))
                        {
                            Point1=new Vector2Int(i, j);
                            ChangeGridsStates((int)Point1.x, (int)Point1.y, Point2.x, Point2.y,true);
                            return new Vector2Int(i, j);
                        }
                    }
                  
                }
             
            }
            return new Vector2Int(-1,-1);
        }

        #region 自动查询空余位置模块
        /// <summary>
        /// 获取背包空余空间
        /// </summary>
        /// <returns>返回在背包中的 坐标</returns>
        public static Vector2Int GetKnapsackItemUnoccupied(KnapsackGridData autoIndex)
        {
            //全部清空
            for (int j = 0; j < LENGTH_Knapsack_Y; j++)
            {
                for (int i = 0; i < LENGTH_Knapsack_X; i++)
                {
                    Grids[i][j].isOccupy = false;
                }
            }
            var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
            foreach (var item in list)
            {
                if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
                var Point1_ = new Vector2Int(item.PosInBackpackX, item.PosInBackpackY);
                var Point2_ = new Vector2Int(item.PosInBackpackX + item.X - 1, item.PosInBackpackY + item.Y - 1);
                bool IsSinglePoint_ = Point1_ == Point2_;
                //Log.DebugGreen($"是否是单个武器：{IsSinglePoint}  {Point1} {Point2}");
                if (IsSinglePoint_)
                {
                    Grids[Point1_.x][Point1_.y].isOccupy = true;
                }
                else
                {
                    //List<KnapsackGrid> gris = GetAreaGrids(Point1_.x, Point1_.y, Point2_.x, Point2_.y, Grids);
                    List<Vector2> gris = GetAreaGrids(Point1_.x, Point1_.y, Point2_.x, Point2_.y, Grids);

                    for (int i = 0, length = gris.Count; i < length; i++)
                    {
                        // gris[i].isOccupy = true;
                        Grids[(int)gris[i].x][(int)gris[i].y].isOccupy = true;
                    }

                }
            }
            //查询空余位置
            //遍历加入，是否可以加入
            for (int j = 0; j < LENGTH_Knapsack_Y; j++)
            {
                
                for (int i = 0; i < LENGTH_Knapsack_X; i++)
                {
                    //单个物体
                    if (autoIndex.IsSinglePoint)
                    {
                        if (Grids[i][j].isOccupy == false)
                        {
                            Grids[i][j].isOccupy = true;
                            return new Vector2Int(i, j);
                        }
                    }
                    //范围性物体
                    else
                    {
                        //如果超出边缘直接过滤
                        if (!ContainGridObj(i, j, i + autoIndex.Point2.x, j + autoIndex.Point2.y, Grids))
                        {
                          
                            ChangeGridsStates((int)autoIndex.Point1.x, (int)autoIndex.Point1.y, autoIndex.Point2.x, autoIndex.Point2.y, true);
                            return new Vector2Int(i, j);
                        }
                    }
                }
            }

            return new Vector2Int(-1,-1);
        }
        #endregion
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public static List<Vector2> GetAreaGrids(int startX, int startY, int endX, int endY, KnapsackGrid[][] Grids)
        {
            List<Vector2> results = new List<Vector2>();
            if (startX >= LENGTH_Knapsack_X || endX >= LENGTH_Knapsack_X || startY >= LENGTH_Knapsack_Y || endY >=LENGTH_Knapsack_Y ||
                startX < 0 || endX < 0 || startY < 0 || endY < 0)
            {
                return results;
            }
            //横向查询
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    try
                    {
                      //  results.Add(Grids[i][j]);
                        results.Add(new Vector2(i,j));
                    }
                    catch (System.Exception e)
                    {

                        return new List<Vector2>();
                    }
                }
            }

            return results;
        }

        public static bool ContainGridObj(int startX, int startY, int endX, int endY, KnapsackGrid[][] Grids)
        {
            List<Vector2> results = GetAreaGrids(startX, startY, endX, endY,Grids);

            //判断如果超出所以则失败
            if (results.Count <= 0)
            {
                //超出区域，默认占用
                return true;
            }

            foreach (var item in results)
            {
                //  if (item.isOccupy) return true;
                if (Grids[(int)item.x][(int)item.y].isOccupy) return true; 
            }

            return false;
        }
       

        public static KnapsackGridData CreateGridData(int startX, int startY, int endX, int endY, int EquipmentPart =0)
        {
            return new KnapsackGridData() 
            { 
                Point1 = new Vector2Int(startX, startY), 
                Point2 = new Vector2Int() { x = endX, y = endY }, 
                EquipmentPart = EquipmentPart 
            };
        }
        public static KnapsackGridData CreateSingleGridData(int x, int y, int EquipmentPart = 0)
        {
            KnapsackGridData data = new KnapsackGridData() { EquipmentPart = EquipmentPart };
            data.SetSinglePoint(new Vector2Int(x, y));
            return data;
        }
    }
}
