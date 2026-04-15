
namespace ETModel
{   
    /// <summary>
    /// 格子节点
    /// </summary>
    public class AstarNode
    {
        public int AreaIndex { get; set; }
        /// <summary>
        /// 所在区域X、Y
        /// </summary>
        public int AreaPosX { get; set; }
        public int AreaPosY { get; set; }

        public AstarSceneData Map;

        /// <summary>
        /// 格子x、y
        /// </summary>
        public int x;
        public int z;

        public float hCost;
        public float gCost;

        public float FCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public bool isWalkable = true;

        public AstarNode parentNode;

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="node"></param>
        /// <returns>false:不相等  true：相等 </returns>
        public bool Compare(AstarNode node)
        {

            if (node == null || node.x != this.x || node.z != this.z)
                return false;
            return true;
        }

        public void Add(AstarNode node)
        {
            if (node == null) return;
            this.x += node.x;
            this.z += node.z;
        }


        
      

        public override string ToString()
        {
            return $"x:{x} z:{z}";
        }
    }
}
