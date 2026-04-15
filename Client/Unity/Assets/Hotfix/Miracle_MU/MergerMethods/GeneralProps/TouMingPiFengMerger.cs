using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 透明披风合成
    /// </summary>
    [MergerSystem(103)]
    public class TouMingPiFengMerger : MergerMethod
    {
        int lev = 1;//默认是合成一级的透明披风
        readonly long XueLingZhiShuId = 320004;//血灵之书 ID
        readonly long XueLingZhiKuId = 320005;//血灵之骷 Id
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = GetMoney();
            SuccessRate = 80;
            MaxSuccessRate = 80;
            FailedDelete = true;
            mergerMethodId = 10002;
            mergerMethod = "TouMingPiFengSynthesis";
           
            //是否有血灵·之书、血灵之骷、玛雅之石
            (bool IsHaveXueLingZhiShu, bool IsHaveXueLingZhiKu, bool IsHaveMayaStone) MustItem = (false, false, false);
            //标题
            AddTextTitle($"透明披风 +{lev}");
            //必要材料
            if (CheckItems.Count == 1)
            {
                MustItem.IsHaveXueLingZhiShu = IsHaveItem(XueLingZhiShuId, ref lev);
                MustItem.IsHaveXueLingZhiKu = IsHaveItem(XueLingZhiKuId, ref lev);
                AddMustItemInfoText(isHave: MustItem.IsHaveXueLingZhiShu, str: $"血灵之书+{lev}\t\tx1");
                AddMustItemInfoText(isHave: MustItem.IsHaveXueLingZhiKu, str: $"血灵之骷+{lev}\t\tx1");
            }
            else if (CheckItems.Count >= 2)
            {
                if (CheckItems[0].ConfigId == XueLingZhiShuId|| CheckItems[1].ConfigId == XueLingZhiShuId)//已经加入 血灵之书
                {
                    AddMustItemInfoText(isHave: MustItem.IsHaveXueLingZhiShu = IsHaveItem(XueLingZhiShuId, ref lev), str: $"血灵之书+{lev}\t\tx1");
                    AddMustItemInfoText($"血灵之骷+{lev}\t\tx1", MustItem.IsHaveXueLingZhiKu = IsHaveItem(XueLingZhiKuId, lev: lev));
                }
                else if (CheckItems[0].ConfigId == XueLingZhiKuId || CheckItems[1].ConfigId == XueLingZhiKuId)
                {
                    AddMustItemInfoText(isHave: MustItem.IsHaveXueLingZhiKu = IsHaveItem(XueLingZhiKuId, ref lev), str: $"血灵之骷+{lev}\t\tx1");
                    AddMustItemInfoText($"血灵之书+{lev}\t\tx1", MustItem.IsHaveXueLingZhiShu = IsHaveItem(XueLingZhiShuId, lev: lev));
                }
            }
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(GemItemConfigId.MAYA_GEMS.ToInt64()));
            Money = GetMoney();
            AddTextTitle($"透明披风 +{lev}");

            IsCanMerger = MustItem == (true, true, true);
            return CheckItemCount();

            int GetMoney() => lev switch
            {
                1 => 50_000,
                2 => 80_000,
                3 => 150_000,
                4 => 200_000,
                5 => 400_000,
                6 => 600_000,
                7 => 800_000,
                _ => 50_000
            };
        }
    }
}
