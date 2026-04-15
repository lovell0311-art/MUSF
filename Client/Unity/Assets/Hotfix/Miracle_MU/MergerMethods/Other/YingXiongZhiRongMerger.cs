using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 英雄之戒合成
    /// </summary>
    [MergerSystem(701)]
    public class YingXiongZhiRongMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 10_000_0;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10026;
            mergerMethod = "ArchangelHammerSynthesis";
            (bool IsMoLianDeXinWu, bool IsHaveZhuFuBaoShi, bool IsHaveLingHunBaoShi, bool IsHaveShengMingBaoShi, bool IsHaveMayaStone) MustItem;
           
            //标题
            AddTextTitle("英雄之戒合成");
            ///必须材料
            AddMustItemInfoText("磨练的信物\t\tx1", MustItem.IsMoLianDeXinWu = IsHaveItem(itemConfigId: 320278));
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 10, out int zhufucount), isEnough: zhufucount >= 10, str: $"祝福宝石\t\tx10({zhufucount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 10, out int linghuncount), isEnough: linghuncount >= 10, str: $"灵魂宝石\t\tx10({linghuncount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveShengMingBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), 10, out int lshengmingcount), isEnough: lshengmingcount >= 10, str: $"生命宝石\t\tx10({lshengmingcount})");
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));

            IsCanMerger = MustItem == (true, true, true, true, true);
            return CheckItemCount();
        }
    }
}
