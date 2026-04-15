using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 二代披风
    /// </summary>
    [MergerSystem(305)]
    public class MergerPiFeng_2 : MergerMethod
    {
        /// <summary>
        /// 二代披风合成
        /// </summary>
        Dictionary<long, string> Wing_1_Dic = new Dictionary<long, string>()
        {
          { 220057,"灾难之翼"},
          { 220058,"恶魔之翼"},
          { 220059,"天使之翼"},
          { 220060,"精灵之翼"},
        };
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 5_000_0;
            SuccessRate = 0;
            MaxSuccessRate = 90;
            FailedDelete = true;
            mergerMethodId = 10006;
            mergerMethod = "SecondPiFengSynthesis";
            (bool IsHaveWing_1, bool IsHaveGuoWangJuanZhou, bool IsHaveMayaStone,bool IsHaveZhuoYue) MustItem;
            Log.DebugGreen($"二代披风合成：{CheckItems.Count}");
            //标题
            AddTextTitle("二代披风合成");
            ///必须材料
            AddMustItemInfoText("一代翅膀\t\tx1", MustItem.IsHaveWing_1 = IsHaveWing_1());
            AddMustItemInfoText("国王卷轴\t\tx1", MustItem.IsHaveGuoWangJuanZhou = IsHaveItem(itemConfigId: 320298));
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            AddMustItemInfoText("卓越品质武器/防具装备 (+4以上/追4以上) x1", MustItem.IsHaveZhuoYue = IsHaveExcellenceItem());
            ///辅助材料
            AddSubItemInfoText("祝福宝石(+5%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), (long)100000, 5, false));
            AddSubItemInfoText("灵魂宝石(+3%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), (long)100000, 3, false));
            //AddSubItemInfoText("玛雅之石(+2%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), (long)100000, 2, false));
            AddSubItemInfoText("中级魔晶石(+5%) 可选 xN", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            // AddSubItemInfoText("幸运符咒(+1~10%) 可选 x1~10", IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            //AddSubItemInfoText("王者披风合成符咒(+100%) 可选 x1", IsHaveItem(itemConfigId: 320041, addSuccessrateValue: 0, IsMust: false));
            IsCanMerger = MustItem == (true, true, true,true);
            return CheckItemCount();
            ///是否有一代翅膀
            bool IsHaveWing_1()
            {
                bool isHave = false;
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    var item = CheckItems[i];
                    if (Wing_1_Dic.ContainsKey(item.ConfigId))
                    {
                        SuccessRate = CheckItems[i].GetProperValue(E_ItemValue.Level) * 2 + CheckItems[i].OptLev * 3;
                        CheckItems.RemoveAt(i);
                        isHave = true;
                        break;
                    }
                }
                IsCanMerger = isHave;
                return isHave;
            }
            //是否有卓越品质武器/防具装备 (+4以上/追4以上)
            bool IsHaveExcellenceItem()
            {
                bool isHave = false;
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    var item = CheckItems[i];
                    if (item.IsHaveExecllentEntry && item.GetProperValue(E_ItemValue.Level) >= 4 && item.OptLev >= 1)
                    {
                        SuccessRate += CheckItems[i].GetProperValue(E_ItemValue.Level) * 2 + CheckItems[i].OptLev * 3 + CheckItems[i].ExecllentEntryDic.Count * 7;//卓越属性;
                        CheckItems.RemoveAt(i);
                        isHave = true;
                        break;
                    }
                }
                return isHave;
            }
        }
    }
}
