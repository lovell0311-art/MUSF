using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 装备数据 拓展类
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 是否可以拖拽使用
        /// </summary>
        /// <returns></returns>
        public bool IsCanDragUser() 
        {
            //宝石
            if (ItemType == (int)E_ItemType.Gemstone)
            {
                return true;
            }
            switch (ConfigId)
            {
                case 310074://翅膀重置卡
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 是否是宝箱钥匙
        /// </summary>
        /// <returns></returns>
        public bool IsTheTresureKey()
        {
            switch (ConfigId)
            {
                case 320433://白银宝箱钥匙
                    return true;
                case 320434://黄金宝箱钥匙
                    return true;
                case 320435://钻石宝箱钥匙
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 该物品是否为可被打开的宝箱
        /// </summary>
        /// <returns></returns>
        public bool IsCanOpen() {
            switch (ConfigId)
            {
                case 320407://白银宝箱
                    return true;
                case 320408://黄金宝箱
                    return true;
                case 320409://钻石宝箱
                    return true;
            }
            return false;
        }
        
    }
}
