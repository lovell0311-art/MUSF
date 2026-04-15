using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///额外属性 套装附带
namespace ETHotfix
{
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 额外属性(套装附带)
        /// key:配置表ID
        /// value:属性等级
        /// </summary>
        public Dictionary<int, int> ExtraEntryDic = new Dictionary<int, int>();

        public void SetExtraEntryDic(Struct_AttrEntry property)
        {
            ExtraEntryDic[property.PropId] = property.Level;
        }
        /// <summary>
        /// 获取额外属性
        /// </summary>
        /// <param name="list"></param>
        public void GetExtraEntryAtr(ref List<string> list)
        {
           
            if (ExtraEntryDic.Count == 0) return;

            foreach (var item in ExtraEntryDic)
            {
                var itemExtraEntryInfo = ConfigComponent.Instance.GetItem<ItemAttrEntry_ExtraConfig>(item.Key);
                list.Add($"<color={ColorTools.ExcellenceItemColor}>{string.Format(itemExtraEntryInfo.Name, GetValue(itemExtraEntryInfo,item.Value))}</color>");
            }
            list.Add("");
            ///获取 属性值
            int GetValue(ItemAttrEntry_ExtraConfig itemAttrEntry_ExtraConfig, int value) => value switch
            {
                0 => itemAttrEntry_ExtraConfig.Value0,
                1 => itemAttrEntry_ExtraConfig.Value1,
                2 => itemAttrEntry_ExtraConfig.Value2,
                _ => 0
            };
        }
    }
}
