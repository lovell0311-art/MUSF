using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 征服者的翅膀
    /// </summary>
    [MergerSystem(312)]
    public class ZhengFuZheDeChiBangMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 10_000_0;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10015;
            mergerMethod = "ZhengFuZheWingSynthesis";
            (bool IsHaveZhengFuZheDeXinWu,bool IsHaveZhuFuGem, bool IsHaveLingHunGem, bool IsHaveShengMingGem, bool IsHaveMayaStone) MustItem;
            Log.DebugGreen($"征服者的翅膀合成：{CheckItems.Count}");
            //标题
            AddTextTitle("征服者的翅膀合成");
            ///必须材料
            AddMustItemInfoText("征服者的信物\t\tx1", MustItem.IsHaveZhengFuZheDeXinWu = IsHaveItem(320284));
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuGem = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 10, out int count), isEnough: count >= 10, str: $"祝福宝石\t\tx10({count})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunGem = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 10, out int linghuncount), isEnough: linghuncount >= 10, str: $"灵魂宝石\t\tx10({linghuncount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveShengMingGem = IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), 10, out int creatcount), isEnough: creatcount >= 10, str: $"生命宝石\t\tx10({creatcount})");
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            IsCanMerger = MustItem == (true, true, true, true, true);
            return CheckItemCount();
        }
    }
}
