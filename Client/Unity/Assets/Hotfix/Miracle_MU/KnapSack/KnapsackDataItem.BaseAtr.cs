using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// 基础属性(直接读配置表)
    /// </summary>
    public partial class KnapsackDataItem
    {
        public void GetBaseAtrs(ref List<string> list)
        {
       
            if (item_Info.BaseAttrId == null || item_Info.BaseAttrId.Count == 0) return;
            ItemAttrEntry_BaseConfig itemAttrEntry_Base;
           
            for (int i = 0, length= item_Info.BaseAttrId.Count; i < length; i++)
            {
                itemAttrEntry_Base = ConfigComponent.Instance.GetItem<ItemAttrEntry_BaseConfig>(item_Info.BaseAttrId[i]);
                if (itemAttrEntry_Base == null) { continue; }

                if (GetValue() + GetOutsattrib() == 0) continue;
                list.Add($"<color={ColorTools.NormalItemNameColor}>{string.Format(itemAttrEntry_Base.Name, GetValue() + GetOutsattrib())}</color>");
            }
            list.Add("");

            ///获取 属性值
            int GetValue() => GetProperValue(E_ItemValue.Level) switch
            {
                0 => itemAttrEntry_Base.Value0,
                1 => itemAttrEntry_Base.Value1,
                2 => itemAttrEntry_Base.Value2,
                3 => itemAttrEntry_Base.Value3,
                4 => itemAttrEntry_Base.Value4,
                5 => itemAttrEntry_Base.Value5,
                6 => itemAttrEntry_Base.Value6,
                7 => itemAttrEntry_Base.Value7,
                8 => itemAttrEntry_Base.Value8,
                9 => itemAttrEntry_Base.Value9,
                10 => itemAttrEntry_Base.Value10,
                11 => itemAttrEntry_Base.Value11,
                12 => itemAttrEntry_Base.Value12,
                13 => itemAttrEntry_Base.Value13,
                14 => itemAttrEntry_Base.Value14,
                15 => itemAttrEntry_Base.Value15,
                _ => 0
            };

            //获取额外的卓越属性
            int GetOutsattrib()
            {
                if (this.ItemType == (int)E_ItemType.Pet || this.ItemType == (int)E_ItemType.Necklace || this.ItemType == (int)E_ItemType.Rings || this.ItemType == (int)E_ItemType.QiZhi || this.ItemType == (int)E_ItemType.Guard || this.ItemType == (int)E_ItemType.WristBand)
                {
                    if (this.IsHaveExecllentEntry)
                    {
                        return itemAttrEntry_Base.Outsattrib;
                    }
                }
                return 0;
            }

        }
    }
}
