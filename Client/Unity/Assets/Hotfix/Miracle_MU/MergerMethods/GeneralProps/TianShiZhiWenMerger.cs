using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;
using UnityEditorInternal.VersionControl;

namespace ETHotfix
{
    /// <summary>
    /// 天使之吻
    /// </summary>
    //[MergerSystem(105)]
    public class TianShiZhiWenMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_000_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10053;
            mergerMethod = "AngelKissSynthesis";

            (bool IsHaveTianShiNaJie,bool IsHaveMoJingShi, bool IsHaveZhuFu, bool IsHaveLinHun, bool IsHaveMaYa, bool IsHaveGouWei) MustItem;
            //标题
            AddTextTitle("天使之吻合成");
            //必要材料
            AddMustItemInfoText(isHave: MustItem.IsHaveTianShiNaJie = IsHaveItem(itemConfigId: 320420, 3, out int curCount), isEnough: curCount >= 3, str: $"天使衲戒\t\tx3({curCount})");
            AddMustItemInfoText("魔晶石\t\tx1", MustItem.IsHaveMoJingShi = IsHaveItem(GemItemConfigId.LEVEL_MOJING_STONE.ToInt64()));
            AddMustItemInfoText("祝福宝石\t\tx1", MustItem.IsHaveZhuFu = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64()));
            AddMustItemInfoText("灵魂宝石\t\tx1", MustItem.IsHaveLinHun = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64()));
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMaYa = IsHaveItem(GemItemConfigId.MAYA_GEMS.ToInt64()));
            AddMustItemInfoText("终极勾尾\t\tx1", MustItem.IsHaveGouWei = IsHaveItem(240001));
            IsCanMerger = MustItem == (true,true,true,true,true,true);
            return CheckItemCount();
        }
    }
}