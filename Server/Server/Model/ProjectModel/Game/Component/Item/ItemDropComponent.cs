using CustomFrameWork.Baseic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace ETModel
{
    /// <summary>
    /// 地图物品掉落组件
    /// </summary>
    public class ItemDropComponent : TCustomComponent<MapComponent>
    {
        /// <summary>
        /// 最大捡取距离，超过这个距离不可捡取
        /// </summary>
        public const int DROP_MAXDISTANCE = 10;
        /// <summary>
        /// 地图上掉落的所有物品
        /// key-ItemUID  value:Item
        /// </summary>
        public Dictionary<long, Item> ItemDict = new Dictionary<long, Item>();
    }
}
