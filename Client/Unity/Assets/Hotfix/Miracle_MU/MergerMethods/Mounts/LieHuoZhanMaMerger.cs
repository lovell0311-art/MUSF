using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 烈火战马
    /// </summary>
    [MergerSystem(211)]
    public class LieHuoZhanMaMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_000_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10078;
            mergerMethod = "FlamingHorseSynthesis";
            (bool IsHaveYouLingZhanMaYingZhang,bool IsHaveYanLangShou, bool IsHaveFuHuoSuiPian, bool IsHaveZhuFuBaoShi, bool IsHaveMaYa) MustItem;

            //标题
            AddTextTitle("烈火战马合成");
            ///必须材料
            AddMustItemInfoText("飞行火麒麟\t\tx1", MustItem.IsHaveYanLangShou = IsHaveItem(itemConfigId: 260022));
            AddMustItemInfoText("战马印章\t\tx1", MustItem.IsHaveYouLingZhanMaYingZhang = IsHaveItem(itemConfigId: 320312));
            AddMustItemInfoText(isHave: MustItem.IsHaveFuHuoSuiPian = IsHaveItem(itemConfigId: 320315, 10, out int zhufucount), isEnough: zhufucount >= 10, str: $"复活碎片\t\tx10({zhufucount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 30, out int linghuncount), isEnough: linghuncount >= 30, str: $"祝福宝石\t\tx30({linghuncount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveMaYa = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 30, out int linghuncount1), isEnough: linghuncount1 >= 30, str: $"玛雅之石\t\tx30({linghuncount1})");
           // AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMaYa = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));

            IsCanMerger = MustItem == (true,true, true, true, true);
            return CheckItemCount();
        }
    }
}
