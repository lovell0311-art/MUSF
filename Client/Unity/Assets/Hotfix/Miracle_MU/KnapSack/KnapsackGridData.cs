using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 背包单个格子数据类
    /// </summary>
    public class KnapsackGridData
    {
        /// <summary>
        /// 格子的UUID
        /// </summary>
        public long UUID { get; set; }
        public Vector2Int Point1 { get; set; } = Vector2Int.zero;
        public Vector2Int Point2 { get; set; } = Vector2Int.zero;
        /// <summary>
        /// 是否是单个格子
        /// </summary>
        public bool IsSinglePoint => Point1 == Point2;
        //是否是删除格子
        public bool IsDelPoint { get { return Point1.x == -1; } }
        /// <summary>
        /// 物品所属的装备部位
        /// 1->武器
        /// 2->盾牌
        /// 3->头盔
        /// 4->铠甲
        /// 5->护腿
        /// 6->护手
        /// 7->靴子
        /// 8->翅膀
        /// 9->守护
        /// 10->项链
        /// 11->左戒指
        /// 12->右戒指
        /// 13->旗帜
        /// 14->宠物
        /// 15->背部武器
        /// 16->背部盾牌
        /// </summary>
        public int EquipmentPart { get; set; }
        /// <summary>
        /// 物品属性
        /// </summary>
        public KnapsackDataItem ItemData { get; set; }
        /// <summary>
        /// 格子类型
        /// </summary>
        public E_Grid_Type Grid_Type = E_Grid_Type.Knapsack;

        public void SetSinglePoint(Vector2Int vector2Int)
        {
            Point1 = vector2Int;
            Point2 = vector2Int;
        }
    }

}
