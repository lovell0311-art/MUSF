using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 天鹰合成
    /// </summary>
    [MergerSystem(207)]
    public class TianYingMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_000_000;
            SuccessRate = 60;
            MaxSuccessRate = 60;
            FailedDelete = true;
            mergerMethodId = 10022;
            mergerMethod = "TianYingSynthesis";
            (bool IsHaveHeiWangMaZhiHun, bool IsHaveZhuFuBaoShi, bool IsHaveLingHunBaoShi, bool IsHaveChuangZaoBaoShi, bool IsHaveMayaStone) MustItem;
           
            //标题
            AddTextTitle("天鹰合成");
            ///必须材料
            AddMustItemInfoText("天鹰之魂\t\tx1", MustItem.IsHaveHeiWangMaZhiHun = IsHaveItem(itemConfigId: 320311));
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 2, out int zhufucount), isEnough: zhufucount >=2, str: $"祝福宝石\t\tx2({zhufucount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 2, out int linghuncount), isEnough: linghuncount >= 2, str: $"灵魂宝石\t\tx2({linghuncount})");
            AddMustItemInfoText("创造宝石\t\tx1", MustItem.IsHaveChuangZaoBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64()));
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
          //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            IsCanMerger = MustItem == (true, true, true, true, true);
            return CheckItemCount();
        }
    }
}
