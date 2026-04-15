using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 神鹰之羽
    /// </summary>
    [MergerSystem(306)]
    public class ShenYingZhiYuMerger : MergerMethod
    {
        /// <summary>
        /// 二代翅膀、披风
        /// </summary>
        Dictionary<long, string> Wing_2_Dic = new Dictionary<long, string>()
        {
          { 220046,"绝望之翼"},
          { 220047,"暗黑之翼"},
          { 220048,"飞龙之翼"},
          { 220049,"魔魂之翼"},
          { 220050,"圣灵之翼"},
          { 220054,"极限披风"},
          { 220055,"武者披风"},
          { 220056,"王者披风"},
        };
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 5_000_000;
            SuccessRate = 0;
            MaxSuccessRate = 70;
            FailedDelete = true;
            mergerMethodId = 10011;
            mergerMethod = "ShengMingWingSynthesis";
            (bool IsHaveWing_2, bool IsHaveCreatStone, bool IsHavelinHunGem, bool IsHaveMayaStone) MustItem;
            Log.DebugGreen($"神鹰之羽合成：{CheckItems.Count}");
            //标题
            AddTextTitle("神鹰之羽合成");
            ///必须材料
            AddMustItemInfoText("二代翅膀/披风(+9以上追4以上)\t\tx1", MustItem.IsHaveWing_2 = IsHaveWing_2());
            AddMustItemInfoText("创造宝石\t\tx1", MustItem.IsHaveCreatStone = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64()));
            //AddMustItemInfoText("灵魂宝石\t\tx1", MustItem.IsHavelinHunGem = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64()));
            AddMustItemInfoText(isHave: MustItem.IsHavelinHunGem = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 10, out int linghuncount), isEnough: linghuncount >= 10, str: $"灵魂宝石\t\tx10({linghuncount})");
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            //可选材料
            //AddSubItemInfoText("高级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.High_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("高级魔晶石(+5%) 可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.High_LEVEL_MOJING_STONE.ToInt64(), (long)100000, 5, false));
            AddSubItemInfoText("灵魂宝石(+3%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), (long)100000, 3, false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));
            // AddSubItemInfoText("幸运符咒(+1~10%) 可选 x1~10", IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            // AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            IsCanMerger = MustItem == (true, true, true, true);
            return CheckItemCount();

            ///是否有二代翅膀 (+9 追4)
            bool IsHaveWing_2()
            {
                bool isHave = false;
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    var item = CheckItems[i];
                    if (Wing_2_Dic.ContainsKey(item.ConfigId)&&item.GetProperValue(E_ItemValue.Level)>=9&&item.OptLev>=1)
                    {
                        SuccessRate += CheckItems[i].GetProperValue(E_ItemValue.Level) * 2 + CheckItems[i].OptLev * 3 + CheckItems[i].ExecllentEntryDic.Count * 7;//卓越属性;
                        CheckItems.RemoveAt(i);
                        isHave = true;
                        break;
                    }
                }
                IsCanMerger = isHave;
                return isHave;
            }
        }
    }
}