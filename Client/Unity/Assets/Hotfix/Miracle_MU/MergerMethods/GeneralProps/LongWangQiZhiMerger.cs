using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;
using UnityEditorInternal.VersionControl;

namespace ETHotfix
{
    /// <summary>
    /// 龙王旗帜
    /// </summary>
    //[MergerSystem(104)]
    public class LongWangQiZhiMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_000_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10039;
            mergerMethod = "LongwangqizhiSynthesis";

            //是否用龙王旗杆
            (bool IsHaveLongWangQiGan,bool IsHaveMoJingShi, bool IsHaveZhuFu, bool IsHaveLinHun) MustItem;
            //标题
            AddTextTitle("龙王旗帜合成");
            //必要材料
            AddMustItemInfoText(isHave: MustItem.IsHaveLongWangQiGan = IsHaveItem(itemConfigId: 320412, 3, out int curCount), isEnough: curCount >= 3, str: $"龙王旗杆\t\tx3({curCount})");
            AddMustItemInfoText("祝福宝石\t\tx1", MustItem.IsHaveZhuFu = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64()));
            AddMustItemInfoText("灵魂宝石\t\tx1", MustItem.IsHaveLinHun = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64()));
            AddMustItemInfoText("魔晶石\t\tx1", MustItem.IsHaveMoJingShi = IsHaveItem(GemItemConfigId.LEVEL_MOJING_STONE.ToInt64()));
            IsCanMerger = MustItem == (true,true,true,true);
            return CheckItemCount();
        }
    }
}