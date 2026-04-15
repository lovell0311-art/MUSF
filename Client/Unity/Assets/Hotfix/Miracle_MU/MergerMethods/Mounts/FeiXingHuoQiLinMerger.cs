using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 飞行火麒麟 合成
    /// </summary>
    [MergerSystem(210)]
    public class FeiXingHuoQiLinMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_000_00;
            SuccessRate = 50;
            MaxSuccessRate = 80;
            FailedDelete = true;
            mergerMethodId = 10087;
            mergerMethod = "KirinSynthesis";
            (bool IsHaveHeiWangMaZhiHun, bool IsHaveZhuFuBaoShi, bool IsHaveLingHunBaoShi, bool IsHaveChuangZaoBaoShi) MustItem;

            //标题
            AddTextTitle("飞行火麒麟");
            ///必须材料
            AddMustItemInfoText("折断的角\t\tx1", MustItem.IsHaveHeiWangMaZhiHun = IsHaveItem(itemConfigId: 320010));
            AddMustItemInfoText("炎狼兽之角\t\tx1", MustItem.IsHaveChuangZaoBaoShi = IsHaveItem(itemConfigId: 260008));
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(), 2, out int zhufucount), isEnough: zhufucount >= 2, str: $"创造宝石\t\tx2({zhufucount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 3, out int linghuncount), isEnough: linghuncount >= 3, str: $"灵魂宝石\t\tx3({linghuncount})");
            //AddMustItemInfoText("创造宝石\t\tx1", MustItem.IsHaveChuangZaoBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64()));
            //AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            AddSubItemInfoText("中级魔晶石(+5%) 可选 xn", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
          //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            IsCanMerger = MustItem == (true, true, true, true);
            return CheckItemCount();

        }
    }
}