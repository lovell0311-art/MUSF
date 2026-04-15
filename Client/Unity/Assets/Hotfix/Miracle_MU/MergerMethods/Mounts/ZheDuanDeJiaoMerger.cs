using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 折断的角合成
    /// </summary>
    [MergerSystem(202)]
    public class ZheDuanDeJiaoMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 10_000_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10017;
            mergerMethod = "ZheDuanDeJiaoSynthesis";
            (bool IsHaveMengShouDeJiaoJia, bool IsHaveSuiJiaoPian, bool IsHaveMayaStone) MustItem;
           
            //标题
            AddTextTitle("折断的角合成");
            ///必须材料
            AddMustItemInfoText(isHave: MustItem.IsHaveMengShouDeJiaoJia = IsHaveItem(itemConfigId: 320008, 10, out int count), isEnough: count >= 10, str: $"猛兽的脚甲\t\tx10({count})");
            AddMustItemInfoText(isHave: MustItem.IsHaveSuiJiaoPian = IsHaveItem(itemConfigId: 320009, 5, out int linghuncount), isEnough: linghuncount >= 5, str: $"碎角片\t\tx5({linghuncount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 3, out int linghuncount1), isEnough: linghuncount1 >= 3, str: $"玛雅之石\t\tx3({linghuncount1})");
            //AddMustItemInfoText("玛雅之石\t\tx3", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(),3,out int linghuncount),isEnough:linghuncount>=3);
            IsCanMerger = MustItem == (true, true, true);
            return CheckItemCount();
        }
    }
}