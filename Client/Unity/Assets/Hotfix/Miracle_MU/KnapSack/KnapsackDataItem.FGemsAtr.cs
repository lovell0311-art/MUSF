using ILRuntime.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 荧光宝石属性
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 荧光宝石属性
        /// </summary>
        /// <param name="list"></param>
        public void GetGemsAtr(ref List<string> list)
        {
            if (ItemType != E_ItemType.FGemstone.ToInt32() || GetProperValue(E_ItemValue.FluoreAttr) == 0)
                return;
            FluoreSet_AttrConfig fluoreSet_Attr = ConfigComponent.Instance.GetItem<FluoreSet_AttrConfig>(GetProperValue(E_ItemValue.FluoreAttr) / 100);
            if (fluoreSet_Attr == null) return;
            list.Add(string.Format(fluoreSet_Attr.Info, GetAtrValue(GetProperValue(E_ItemValue.Level))));
            list.Add("");
            float GetAtrValue(int value) => value switch
            {
                0 => (float)fluoreSet_Attr.Level0/ 10000,
                1 => (float)fluoreSet_Attr.Level1 / 10000,
                2 => (float)fluoreSet_Attr.Level2 / 10000,
                3 => (float)fluoreSet_Attr.Level3/ 10000,
                4 => (float)fluoreSet_Attr.Level4 / 10000,
                5 => (float)fluoreSet_Attr.Level5/ 10000,
                6 => (float)fluoreSet_Attr.Level6 / 10000,
                7 => (float)fluoreSet_Attr.Level7/ 10000,
                8 => (float)fluoreSet_Attr.Level8 / 10000,
                9 => (float)fluoreSet_Attr.Level9/ 10000,
                _ => 0,
            };
        }
    }
}
