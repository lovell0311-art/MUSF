
namespace ETModel
{
    /// <summary>
    /// 物品所在组件
    /// </summary>
    public enum EItemInComponent
    {
        None    = 0,    // 未知的
        Map = 1,        // 地图
        Backpack = 2,   // 背包
        Equipment = 3,  // 装备栏
        Warehouse = 4,  // 仓库
        Stall = 5,      // 摊位
        Mail = 6,       // 邮件
        TreasureHouse =7,//藏宝阁
        Mount = 8,//坐骑面板
        TemporarySpace =9,//临时空间
        Lost = 99,      // 丢失的物品，数据加载完成后，会通过邮件发给玩家
    }
}
