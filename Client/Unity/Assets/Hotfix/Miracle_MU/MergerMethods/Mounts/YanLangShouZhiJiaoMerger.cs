using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 炎狼兽之角合成
    /// </summary>
    [MergerSystem(203)]
    public class YanLangShouZhiJiaoMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 10_000_0;
            SuccessRate = 40;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10018;
            mergerMethod = "YanLangShouJiaoSynthesis";
            (bool IsHaveZheDuanDeJiao, bool IsHaveShengMingGem, bool IsHaveMayaStone) MustItem;
           
            //标题
            AddTextTitle("炎狼兽之角合成");
            ///必须材料
            AddMustItemInfoText("折断的角\t\tx1", MustItem.IsHaveZheDuanDeJiao = IsHaveItem(itemConfigId: 320010));
            AddMustItemInfoText(isHave: MustItem.IsHaveShengMingGem = IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), 3, out int linghuncount), isEnough: linghuncount >= 3, str: $"生命宝石\t\tx3({linghuncount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 3, out int linghuncount1), isEnough: linghuncount1 >= 3, str: $"玛雅之石\t\tx3({linghuncount1})");
            //AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            AddSubItemInfoText("中级魔晶石(+5%) 可选 xn", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            //可选材料
            //AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            //  AddSubItemInfoText("幸运符咒(+1~10%) 可选 x1~10",IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
           // AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            IsCanMerger = MustItem == (true, true, true);
            return CheckItemCount();
        }
    }
}
