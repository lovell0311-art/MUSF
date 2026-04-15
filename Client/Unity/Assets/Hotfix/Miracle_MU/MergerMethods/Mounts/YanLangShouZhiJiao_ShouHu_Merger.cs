using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 炎狼兽 之角 守护座机
    /// </summary>
    [MergerSystem(205)]
    public class YanLangShouZhiJiao_ShouHu_Merger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 30_000_0;
            SuccessRate = 30;
            MaxSuccessRate = 79;
            FailedDelete = true;
            mergerMethodId = 10020;
            mergerMethod = "YanLangShouShouHuSynthesis";
            (bool IsHaveNormalWeaponItem, bool IsHaveShengMingGem, bool IsHaveMayaStone, bool NewIsHaveMayaStone) MustItem;

            //标题
            AddTextTitle("炎狼兽之角+守护合成");
            ///必须材料
            AddMustItemInfoText("普通品质防具装备\t\tx1", MustItem.IsHaveNormalWeaponItem = IsHaveFangJuZhuangBei());
            AddMustItemInfoText("炎狼兽之角\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: 260008));
            AddMustItemInfoText(isHave: MustItem.IsHaveShengMingGem = IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), 5, out int linghuncount), isEnough: linghuncount >= 5, str: $"生命宝石\t\tx5({linghuncount})");

            AddMustItemInfoText(isHave: MustItem.NewIsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 8, out int linghuncount1), isEnough: linghuncount >= 8, str: $"玛雅之石\t\tx8({linghuncount1})");
            AddSubItemInfoText("中级魔晶石(+5%) 可选 xn", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            //AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
          //  AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64()));
           // AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            //AddSubItemInfoText("高级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.High_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            //AddSubItemInfoText("魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            IsCanMerger = MustItem == (true, true, true,true);
            //if (IsCanMerger==true)
            //{

            //    AddSuccessRate(50);
            //}
            return CheckItemCount();

            ///普通品质防具装备
            bool IsHaveFangJuZhuangBei()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    Log.DebugBrown($"CheckItems[i].Slot:{CheckItems[i].ItemType}");
                    if (CheckItems[i].ItemType <= (int)E_ItemType.Boots && CheckItems[i].ItemType >= (int)E_ItemType.Shields)
                    {
                        SuccessRate += CheckItems[i].GetProperValue(E_ItemValue.Level) * 2 + CheckItems[i].OptLev * 3 + CheckItems[i].ExecllentEntryDic.Count * 7;//卓越属性;
                        Log.DebugBrown($"CheckItems[i].Slot:{CheckItems[i].ItemType}");
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
        }
    }
}