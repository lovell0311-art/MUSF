using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 幸运属性
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 获取幸运属性
        /// </summary>
        /// <param name="list"></param>
        public void GetLuckyAtr(ref List<string> list)
        {
            if (GetProperValue(E_ItemValue.LuckyEquip) is int value && value == 0)
                return;

            list.Add($"<color={ColorTools.LuckyItemColor}>幸运(灵魂宝石成功机率+25%)</color>");
            list.Add($"<color={ColorTools.LuckyItemColor}>幸运(会心一击率+5%)</color>");
            list.Add("");
        }
    }
}
