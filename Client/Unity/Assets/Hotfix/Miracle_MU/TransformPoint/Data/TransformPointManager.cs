using System.Collections.Generic;
namespace ETHotfix
{
    public struct Point
    {
        public float x;
        public float y;
    }
    //传送点
    public class TransformPointManager : SimpleSingleton<TransformPointManager>
    {
        //可以传送的格子坐标点
        private List<Point> points = new List<Point>();
        public void AddPoint(long x,long y)
        {
            Point point = new Point();
            point.x = x;
            point.y = y;
            points.Add(point);
        }
        public bool ContainPoint(long x, long y)
        {
            for (int i = 0, len = points.Count; i < len; i++)
            {
                if (points[i].x == x && points[i].y == y)
                {
                    return true;
                }
            }
            return false;
        }
        public void RemovePoint(long x, long y)
        {
            for (int i = 0,len = points.Count; i < len; i++)
            {
                if (points[i].x == x && points[i].y == y)
                {
                    points.RemoveAt(i);
                }
            }
        }
        public void Clear()
        {
            points.Clear();
        }
    }

}
