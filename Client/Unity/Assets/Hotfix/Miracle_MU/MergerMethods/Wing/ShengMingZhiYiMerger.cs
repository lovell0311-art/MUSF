using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 生命之翼合成
    /// </summary>
    [MergerSystem(307)]
    public class ShengMingZhiYiMerger : MergerMethod
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
            Money = 2_000;
            SuccessRate = 0;
            MaxSuccessRate = 70;
            FailedDelete = true;
            mergerMethodId = 10008;
            mergerMethod = "ShengYingZhiYuSynthesis";
            (bool IsHaveWing_2, bool IsHaveChiYanMoDeLinHun, bool IsHaveCreatStone, bool IsHaveMayaStone) MustItem;
            Log.DebugGreen($"生命之翼合成：{CheckItems.Count}");
            //标题
            AddTextTitle("生命之翼合成");
            ///必须材料
            AddMustItemInfoText("二代翅膀/披风\t\tx1", MustItem.IsHaveWing_2 = IsHaveWing_2());
            AddMustItemInfoText("炽炎魔的灵魂\t\tx1", MustItem.IsHaveChiYanMoDeLinHun = IsHaveItem(itemConfigId: 320285));
            AddMustItemInfoText("创造宝石\t\tx1", MustItem.IsHaveCreatStone = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64()));
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            //可选材料
         //   AddSubItemInfoText("幸运符咒(+1~10%) 可选 x1~10", IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            AddSubItemInfoText("灵魂宝石(+3%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), (long)100000, 3, false));
            AddSubItemInfoText("中级魔晶石(+5%) 可选 xN", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            //AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            // AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            IsCanMerger = MustItem == (true, true, true, true);
            return CheckItemCount();

            ///是否有二代翅膀
            bool IsHaveWing_2()
            {
                bool isHave = false;
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    var item = CheckItems[i];
                    if (Wing_2_Dic.ContainsKey(item.ConfigId))
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
