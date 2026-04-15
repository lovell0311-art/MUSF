using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 幽灵战马
    /// </summary>
    [MergerSystem(208)]
    public class YouLingZhanMaMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_000_0;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10074;
            mergerMethod = "GhostWarhorseSynthesis";
            (bool IsHaveYouLingZhanMaYingZhang,bool IsHaveHeiWangMa, bool IsHaveCreatStone, bool IsHaveFuHuoSuiPian, bool IsHaveZhuFuBaoShi) MustItem;
            
            //标题
            AddTextTitle("幽灵战马合成");
            ///必须材料
            AddMustItemInfoText("飞行火麒麟\t\tx1", MustItem.IsHaveHeiWangMa = IsHaveItem(itemConfigId: 260022));
            AddMustItemInfoText("战马印章\t\tx1", MustItem.IsHaveYouLingZhanMaYingZhang = IsHaveItem(itemConfigId: 320312));
            AddMustItemInfoText(isHave: MustItem.IsHaveFuHuoSuiPian = IsHaveItem(itemConfigId: 320315, 10, out int zhufucount), isEnough: zhufucount >= 10, str: $"复活碎片\t\tx10({zhufucount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 30, out int linghuncount), isEnough: linghuncount >= 30, str: $"祝福宝石\t\tx30({linghuncount})");

            AddMustItemInfoText(isHave: MustItem.IsHaveCreatStone = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(), 5, out int linghuncount1), isEnough: linghuncount1 >= 5, str: $"创造宝石\t\tx5({linghuncount1})");
            //AddMustItemInfoText("创造宝石\t\tx1", MustItem.IsHaveCreatStone = IsHaveItem(GemItemConfigId.CREATE_GEMS.ToInt64()));

            IsCanMerger = MustItem == (true,true, true, true, true);
            return CheckItemCount();
        }
    }
}
