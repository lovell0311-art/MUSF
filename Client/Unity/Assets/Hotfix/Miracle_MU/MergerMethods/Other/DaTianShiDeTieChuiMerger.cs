using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 大天使的铁锤合成
    /// </summary>
    [MergerSystem(702)]
    public class DaTianShiDeTieChuiMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 100_000_0;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10026;
            mergerMethod = "BlessingSwordSynthesis";
            (bool IsTianJieZhiGang, bool IsHaveZhuFuBaoShi, bool IsHaveLingHunBaoShi, bool IsHaveChuangZhaoBaoShi, bool IsHaveShouHuBaoShi, bool IsHaveMayaStone) MustItem;
            
            //标题
            AddTextTitle("大天使的铁锤合成");
            ///必须材料
            //AddMustItemInfoText("天界之钢\t\tx10", MustItem.IsTianJieZhiGang = IsHaveItem(itemConfigId: 320262));
            AddMustItemInfoText(isHave: MustItem.IsTianJieZhiGang = IsHaveItem(itemConfigId: 320262, 10, out int tianjiezhigang), isEnough: tianjiezhigang >= 10, str: $"天界之钢\t\tx10({tianjiezhigang})");
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 10, out int zhufucount), isEnough: zhufucount >= 10, str: $"祝福宝石\t\tx10({zhufucount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 10, out int linghuncount), isEnough: linghuncount >= 10, str: $"灵魂宝石\t\tx10({linghuncount})");
            AddMustItemInfoText("创造宝石\t\tx1", MustItem.IsHaveChuangZhaoBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64()));
            AddMustItemInfoText("守护宝石\t\tx1", MustItem.IsHaveShouHuBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.GUARDIAN_GEMS.ToInt64()));
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
          //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            IsCanMerger = MustItem == (true, true, true, true, true,true);
            return CheckItemCount();
        }
    }
}
