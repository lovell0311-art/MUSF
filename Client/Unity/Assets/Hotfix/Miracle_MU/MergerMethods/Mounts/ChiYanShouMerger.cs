using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;
using UnityEditorInternal.VersionControl;

namespace ETHotfix
{
    /// <summary>
    /// ≥‡—Ê ﬁ ∫œ≥…
    /// </summary>

    //[MergerSystem(210)]

    public class ChiYanShouMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_000_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10038;
            mergerMethod = "ChiyanshouSynthesis";
            (bool IsHaveHeiWangMaZhiHun, bool IsHaveZhuFu, bool IsHaveLinHun,bool IsHaveMoJingShi) MustItem;
            //±ÍÃ‚
            AddTextTitle("≥‡—Ê ﬁ∫œ≥…");
            ///±ÿ–Î≤ƒ¡œ
            AddMustItemInfoText(isHave: MustItem.IsHaveHeiWangMaZhiHun = IsHaveItem(itemConfigId: 320413, 3, out int chiyanshoucount), isEnough: chiyanshoucount >= 3, str: $"≥‡—Ê ﬁÀÈ∆¨\t\tx3({chiyanshoucount})");
            AddMustItemInfoText("◊£∏£±¶ Ø\t\tx1", MustItem.IsHaveZhuFu = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64()));
            AddMustItemInfoText("¡ÈªÍ±¶ Ø\t\tx1", MustItem.IsHaveLinHun = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64()));
            AddMustItemInfoText("ƒßæß Ø\t\tx1", MustItem.IsHaveMoJingShi = IsHaveItem(itemConfigId: GemItemConfigId.LEVEL_MOJING_STONE.ToInt64()));
            IsCanMerger = MustItem == (true,true,true,true);
            return CheckItemCount();

        }
    }
}