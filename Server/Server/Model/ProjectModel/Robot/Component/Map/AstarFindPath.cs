using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETModel.Robot
{
    public class AstarFindPath
    {
        public class AstarPoint
        {
            public int X, Y;       //点坐标，这里为了方便按照C++的数组来计算，x代表横排，y代表竖列
            public int F, G, H;    //F=G+H
            public AstarPoint Parent;
            public AstarPoint()
            {
                X = 0;
                Y = 0;
                F = 0;
                G = 0;
                H = 0;
                Parent = null;
            }
            public AstarPoint(int x,int y)
            {
                X = x;
                Y = y;
                F = 0;
                G = 0;
                H = 0;
                Parent = null;
            }
        }

        const int kCost1 = 10; //直移一格消耗
        const int kCost2 = 14; //斜移一格消耗

        public int Wight;
        public int Height;

        public byte[,] Maze;
        private AstarPoint[,] astarPointCache;

        public ushort[,] OtherCollision;

        HashSet<AstarPoint> openList = new HashSet<AstarPoint>();
        HashSet<AstarPoint> closeList = new HashSet<AstarPoint>();

        public AstarFindPath()
        {

        }

        public void Init(int width, int height)
        {
            this.Wight = width;
            this.Height = height;
            astarPointCache = new AstarPoint[width, height];
            OtherCollision = new ushort[width, height];
            Array.Clear(astarPointCache, 0, OtherCollision.Length);
        }

        public void FindPath(Vector2Int startPoint, Vector2Int endPoint, ref LinkedList<Vector2Int> pintList, int find_count = 40)
        {
            AstarPoint result = FindPath(startPoint, endPoint, true, find_count);
            while(result != null)
            {
                pintList.AddFirst(new Vector2Int() { x = result.X, y = result.Y });
                result = result.Parent;
            }
            openList.Clear();
            closeList.Clear();
        }


        private AstarPoint FindPath(Vector2Int startPoint, Vector2Int endPoint, bool isIgnoreCorner, int find_count)
        {
            // 限制搜索次数
            int now_find_count = find_count;    // 当前剩余搜索次数
            AstarPoint start = CreatePoint(startPoint.x, startPoint.y); //置入起点,拷贝开辟一个节点，内外隔离
            start.Parent = null;
            start.G = 0;
            start.H = 0;
            start.F = 0;
            openList.Add(start);
            AstarPoint end = CreatePoint(endPoint.x, endPoint.y); //置入结束点,拷贝开辟一个节点，内外隔离
            using ListComponent<AstarPoint> list = ListComponent<AstarPoint>.Create();
            List<AstarPoint> aroundPoints = list;
            while (openList.Count > 0 && now_find_count > 0)
            {
                --now_find_count;
                AstarPoint curPoint = GetLeastFpoint(); //找到F值最小的点
                openList.Remove(curPoint); //从开启列表中删除
                closeList.Add(curPoint); //放到关闭列表
                //1,找到当前周围八个格中可以通过的格子
                aroundPoints.Clear();
                GetAroundPoints(curPoint, isIgnoreCorner, ref aroundPoints);
                foreach (AstarPoint target in aroundPoints)
                {
                    //2,对某一个格子，如果它不在开启列表中，加入到开启列表，设置当前格为其父节点，计算F G H
                    if (IsInList(openList, target) == null)
                    {
                        target.Parent = curPoint;

                        target.G = CalcG(curPoint, target);
                        target.H = CalcH(target, end);
                        target.F = CalcF(target);

                        openList.Add(target);
                    }
                    //3，对某一个格子，它在开启列表中，计算G值, 如果比原来的大, 就什么都不做, 否则设置它的父节点为当前点,并更新G和F
                    else
                    {
                        int tempG = CalcG(curPoint, target);
                        if (tempG < target.G)
                        {
                            target.Parent = curPoint;

                            target.G = tempG;
                            target.F = CalcF(target);
                        }
                    }
                    AstarPoint resPoint = IsInList(openList, end);
                    if (resPoint != null)
                        return resPoint; //返回列表里的节点指针，不要用原来传入的endpoint指针，因为发生了深拷贝
                }
            }
            if (now_find_count <= 0)
            {
                AstarPoint openPoint = GetLeastHpoint(openList);
                if (openPoint != null)
                    return openPoint; //返回列表里的节点指针，不要用原来传入的endpoint指针，因为发生了深拷贝
            }
            return null;
        }

        public bool IsPass(int point_x, int point_y)
        {
            if (Maze[point_x, point_y] == 1 ||
                OtherCollision[point_x, point_y] > 0)
                return false;
            return true;
        }


        #region Core
        private AstarPoint CreatePoint(int x,int y)
        {
            if(astarPointCache[x,y] == null)
            {
                astarPointCache[x, y] = new AstarPoint() { X = x, Y = y };
            }
            return astarPointCache[x, y];
        }

        private AstarPoint GetLeastFpoint()
        {
            if (openList.Count > 0)
            {
                AstarPoint resPoint = openList.First();
                foreach (AstarPoint point in openList)
                {
                    if (point.F < resPoint.F)
                        resPoint = point;
                }
                return resPoint;
            }
            return null;
        }

        private void GetAroundPoints(AstarPoint point, bool isIgnoreCorner,ref List<AstarPoint> aroundPoints)
        {


	        for (int x = point.X - 1; x <= point.X + 1; x++)
		        for (int y = point.Y - 1; y <= point.Y + 1; y++)
			        if (IsCanreach(point, CreatePoint(x, y), isIgnoreCorner))
                        aroundPoints.Add(CreatePoint(x, y));
        }


        private AstarPoint IsInList(HashSet<AstarPoint> list,AstarPoint point)
        {
	        if (IsPass(point.X, point.Y))
	        {
		        // 目标点可通行
		        //判断某个节点是否在列表中
		        if(list.Contains(point))
                    return point;
	        }
	        else {
                // 目标点不可通行 取 H 为 1的列表
                foreach (AstarPoint p in list)
			        if (p.H <= 15)
				        return p;
	        }
            return null;
        }

        private bool IsCanreach(AstarPoint point, AstarPoint target, bool isIgnoreCorner)
        {
	        if (target.X<0 || target.X>(Wight - 1)
		        || target.Y<0 || target.Y>(Height - 1)
		        || !IsPass(target.X, target.Y)
		        || (target.X == point.X && target.Y == point.Y)
		        || IsInList(closeList, target) != null) //如果点与当前节点重合、超出地图、是障碍物、或者在关闭列表中，返回false
		        return false;
	        else
	        {
		        if (Math.Abs(point.X - target.X) + Math.Abs(point.Y - target.Y) == 1) //非斜角可以
			        return true;
		        else
		        {
			        //斜对角要判断是否绊住
			        if (Maze[point.X,target.Y] == 0 && Maze[target.X,point.Y] == 0)
				        return true;
			        else
				        return isIgnoreCorner;
		        }
            }
        }

        private AstarPoint GetLeastHpoint(HashSet<AstarPoint> list)
        {
            // 取出最近的一个列表
            if (list.Count > 0)
            {
                AstarPoint ret_point = list.First();
                foreach (AstarPoint p in list)
                {
                    if (ret_point.H > p.H)
                    {
                        ret_point = p;
                    }
                }
                // end for
                return ret_point;
            }
            return null;
        }

        private int CalcG(AstarPoint temp_start, AstarPoint point)
        {
            int extraG = (Math.Abs(point.X - temp_start.X) + Math.Abs(point.Y - temp_start.Y)) == 1 ? kCost1 : kCost2;
            //int parentG = point.Parent == null ? 0 : point.Parent.G; //如果是初始节点，则其父节点是空
            return temp_start.G + extraG;
        }

        private int CalcH(AstarPoint point, AstarPoint end)
        {
            //用简单的欧几里得距离计算H，这个H的计算是关键，还有很多算法，没深入研究^_^
            return (int)(Math.Sqrt((double)(end.X - point.X) * (double)(end.X - point.X) + (double)(end.Y - point.Y) * (double)(end.Y - point.Y)) * kCost1);
        }

        private int CalcF(AstarPoint point)
        {
            return point.G + point.H;
        }

        #endregion



    }



}
