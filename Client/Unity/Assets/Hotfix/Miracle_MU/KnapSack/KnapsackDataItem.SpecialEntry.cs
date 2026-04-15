using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// 特殊属性-翅膀
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 特殊属性-翅膀
        /// key:配置表ID
        /// value:属性等级
        /// </summary>
        public Dictionary<int, int> SpecialEntryDic = new Dictionary<int, int>();

        public void SetSpecialEntry(Struct_AttrEntry property)
        {
            SpecialEntryDic[property.PropId] = property.Level;
        }
        public void SetSpecialEntry(int key,int value)
        {
            SpecialEntryDic[key] = value;
        }

        /// <summary>
        /// 获取特殊属性
        /// </summary>
        /// <param name="list"></param>
        public void GetSpecialEntry(ref List<string> list)
        {
            foreach (var item in SpecialEntryDic)
            {
                ItemAttrEntry_SpecialConfig itemAttrEntry_Special = ConfigComponent.Instance.GetItem<ItemAttrEntry_SpecialConfig>(item.Key);
                if (itemAttrEntry_Special == null) continue;
                switch(item.Key)
                {
                    case 600005:
                    case 600006:
                        list.Add($"<color={ColorTools.LuckyItemColor}>{string.Format(itemAttrEntry_Special.Name,50 + GetProperValue(E_ItemValue.Level) * 5)}</color>");
                        break;
                    case 600022:
                        list.Add($"<color={ColorTools.LuckyItemColor}>{string.Format(itemAttrEntry_Special.Name, 10 + GetProperValue(E_ItemValue.Level) * 5)}</color>");
                        break;
                    default:
                        list.Add($"<color={ColorTools.LuckyItemColor}>{string.Format(itemAttrEntry_Special.Name, GetValue(itemAttrEntry_Special,item.Value))}</color>");
                        break;
                }
              
            }
            if (SpecialEntryDic.Count == 0) return;
            list.Add("");

            ///获取 属性值
            int GetValue(ItemAttrEntry_SpecialConfig itemAttrEntry_SpecialConfig, int value) => value switch
            {
                0 => itemAttrEntry_SpecialConfig.Value0,
                1 => itemAttrEntry_SpecialConfig.Value1,
                2 => itemAttrEntry_SpecialConfig.Value2,
                3 => itemAttrEntry_SpecialConfig.Value3,
                _ => 0
            };
        }
    }
}
