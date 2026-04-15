using ETModel;
using ILRuntime.Runtime;
using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// Ñ×ÀÇÊÞ+»ÃÓ°
    /// </summary>
    [MergerSystem(212)]
    public class YanLangShouZhiJiao_HuanYing_Merger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_000_0;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10077;
            mergerMethod = "PhantomFWolfBSynthesis";
            (bool IsHaveNormalWeaponItem, bool IsHaveShengMingGem) MustItem;
           
            //±êÌâ
            AddTextTitle("Ñ×ÀÇÊÞÖ®½Ç+»ÃÓ°ºÏ³É");
            ///±ØÐë²ÄÁÏ
            AddMustItemInfoText("Ñ×ÀÇÊÞÖ®½Ç+ÆÆ»µ\t\tx1", MustItem.IsHaveNormalWeaponItem = IsHaveItem(itemConfigId: 260009));
            AddMustItemInfoText("Ñ×ÀÇÊÞÖ®½Ç+ÊØ»¤\t\tx1", MustItem.IsHaveShengMingGem = IsHaveItem(itemConfigId: 260010));
           
            IsCanMerger = MustItem == (true, true);
            return CheckItemCount();

        }
    }
}
