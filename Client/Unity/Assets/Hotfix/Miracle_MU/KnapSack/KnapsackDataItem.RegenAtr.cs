using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 再生属性（只有一条）
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 是否拥有再生属性
        /// </summary>
        public bool IsHaveReginAtr => GetProperValue(E_ItemValue.OrecycledID) != 0;
        /// <summary>
        /// 获取再生属性
        /// </summary>
        /// <param name="list"></param>
        public void GetReginAtr(ref List<string> list)
        {
            if (IsHaveReginAtr == false) return;
            //获取再生属性配置表
            ItemAttrEntry_RegenConfig itemAttrEntry_Regen = ConfigComponent.Instance.GetItem<ItemAttrEntry_RegenConfig>(GetProperValue(E_ItemValue.OrecycledID));
            if (itemAttrEntry_Regen == null) 
            {
                Log.DebugRed($"{GetProperValue(E_ItemValue.OrecycledID)} 再生属性不存在");
                return;
            }
            list.Add($"<color={ColorTools.ZaiShengItemColor}>{string.Format(itemAttrEntry_Regen.Name,GetValue())}</color>");
            list.Add("");
            ///获取对应等级的值
            int GetValue() => GetProperValue(E_ItemValue.OrecycledLevel) switch 
            {
             0=>itemAttrEntry_Regen.Value0,
             1=>itemAttrEntry_Regen.Value1,
             2=>itemAttrEntry_Regen.Value2,
             3=>itemAttrEntry_Regen.Value3,
             4=>itemAttrEntry_Regen.Value4,
             5=>itemAttrEntry_Regen.Value5,
             6=>itemAttrEntry_Regen.Value6,
             7=>itemAttrEntry_Regen.Value7,
             8=>itemAttrEntry_Regen.Value8,
             9=>itemAttrEntry_Regen.Value9,
             10=>itemAttrEntry_Regen.Value10,
             11=>itemAttrEntry_Regen.Value11,
             12=>itemAttrEntry_Regen.Value12,
             13=>itemAttrEntry_Regen.Value13,
             14=>itemAttrEntry_Regen.Value14,
             15=>itemAttrEntry_Regen.Value15,
             _=>0
            };
        }
    }
}
