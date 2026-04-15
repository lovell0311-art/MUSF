using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 碎角片 合成
    /// </summary>
    [MergerSystem(201)]
    public class SuiJiaoPianMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 10_000_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10016;
            mergerMethod = "SuiJiaoPianSynthesis";
            (bool IsHavePoLanDeKaiJiaPian, bool IsHaveNvShenDeZhiHui, bool IsHaveMayaStone) MustItem;
           
            //标题
            AddTextTitle("碎角片合成");
            ///必须材料
            AddMustItemInfoText(isHave: MustItem.IsHavePoLanDeKaiJiaPian = IsHaveItem(itemConfigId: 320006, 3, out int count), isEnough: count >= 3, str: $"破烂的铠甲片\t\tx {count}/3");
            AddMustItemInfoText(isHave: MustItem.IsHaveNvShenDeZhiHui = IsHaveItem(itemConfigId: 320007, 3, out int linghuncount), isEnough: linghuncount >=3, str: $"女神的灵智\t\tx{linghuncount}/3");
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            IsCanMerger = MustItem == (true, true, true);
            return CheckItemCount();
        }
    }
}
