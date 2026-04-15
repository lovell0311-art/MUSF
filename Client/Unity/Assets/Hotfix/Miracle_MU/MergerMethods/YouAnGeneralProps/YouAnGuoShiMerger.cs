using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 果实合成
    /// </summary>
    [MergerSystem(802)]
    public class YouAnGuoShiMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 5_000_000;
            SuccessRate = 40;
            MaxSuccessRate = 40;
            FailedDelete = true;
            mergerMethodId = 10052;
            mergerMethod = "Attributefruit";

            //是否用玛雅之石 创造宝石
            (bool IsHaveMayaStone, bool IsHaveCreatStone) MustItem;
            //标题
            AddTextTitle("果实合成");
            //必要材料
            AddMustItemInfoText("创造宝石\t\tx1", MustItem.IsHaveCreatStone = IsHaveItem(GemItemConfigId.CREATE_GEMS.ToInt64()));
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(GemItemConfigId.MAYA_GEMS.ToInt64()));
            IsCanMerger = MustItem == (true, true);
            return CheckItemCount();
        }
    }
}