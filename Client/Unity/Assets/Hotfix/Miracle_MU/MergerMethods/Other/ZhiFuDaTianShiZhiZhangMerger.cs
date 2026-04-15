using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 祝福大天使之杖合成
    /// </summary>
    [MergerSystem(704)]
    public class ZhiFuDaTianShiZhiZhangMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 100_000_0;
            SuccessRate = 40;
            MaxSuccessRate = 50;
            FailedDelete = true;
            mergerMethodId = 10030;
            mergerMethod = "BlessingStaffSynthesis";
            (bool IsDaTianShiZhiJueDuiQuanRen,bool ISData, bool IsDaTianShiDeTieChui, bool IsHaveZhuFuBaoShi, bool IsHaveLingHunBaoShi, bool IsHaveShengMingBaoShi, bool IsHaveMayaStone) MustItem;
           
            //标题
            AddTextTitle("祝福大天使之杖合成");
            ///必须材料
            AddMustItemInfoText(isHave: MustItem.IsDaTianShiZhiJueDuiQuanRen = IsHaveYouLingZhanMa(out int lev, out int optLevel), str: $"大天使之杖(+{lev})追{optLevel}\t\tx1");
            AddMustItemInfoText("大天使之杖\t\tx1", MustItem.ISData = IsHaveItem(itemConfigId: 80010));
            AddMustItemInfoText("大天使的铁锤\t\tx1", MustItem.IsDaTianShiDeTieChui = IsHaveItem(itemConfigId: 320314));
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 30, out int zhufucount), isEnough: zhufucount >= 30, str: $"祝福宝石\t\tx30({zhufucount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 30, out int linghuncount), isEnough: linghuncount >= 30, str: $"灵魂宝石\t\tx30({linghuncount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveShengMingBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), 30, out int shengmingcount), isEnough: shengmingcount >= 30, str: $"生命宝石\t\tx30({shengmingcount})");
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            //可选材料
            //AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
           // AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            IsCanMerger = MustItem == (true, true, true, true, true, true,true);
            return CheckItemCount();

            ///大天使绝对权杖?(+15 追16)
            bool IsHaveYouLingZhanMa(out int lev, out int optLevel)
            {
                bool isHave = false;
                lev = 1;
                optLevel = 1;
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if (CheckItems[i].ConfigId == 80010)
                    {
                        if (CheckItems[i].GetProperValue(E_ItemValue.Level) >= 15 && CheckItems[i].GetProperValue(E_ItemValue.OptLevel) >= 4)
                        {
                            lev = CheckItems[i].GetProperValue(E_ItemValue.Level);
                            optLevel = CheckItems[i].GetProperValue(E_ItemValue.OptLevel);
                            isHave = true;
                            CheckItems.RemoveAt(i);
                            break;
                        }
                    }
                }
                return isHave;
            }
        }
    }
}
