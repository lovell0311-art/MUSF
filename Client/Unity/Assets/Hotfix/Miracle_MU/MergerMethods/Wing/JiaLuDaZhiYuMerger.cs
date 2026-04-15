using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 加鲁达之羽
    /// </summary>
    [MergerSystem(311)]
    public class JiaLuDaZhiYuMerger : MergerMethod
    {
        /// <summary>
        /// 三代翅膀
        /// </summary>
        readonly Dictionary<long, string> Wing_3_Dic = new Dictionary<long, string>()
        {
          { 220035,"暴风之翼"},
          { 220033,"幻影之翼"},
          { 220034,"时空之翼"},
          { 220032,"破灭之翼"},
          { 220018,"幻灭之翼"},
          { 220030,"次元之翼"},
          { 220016,"沉默之翼"},
          { 220031,"帝王披风"},
          { 220029,"斗皇披风"},
          { 220027,"超越披风"},
        };
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 2_000;
            SuccessRate = 0;
            MaxSuccessRate = 60;
            FailedDelete = true;
            mergerMethodId = 10013;
            mergerMethod = "JiaLuDaZhiYuSynthesis";
            (bool IsHaveWing_3,bool IsHaveZhuFuGem, bool IsHaveLingHunGem, bool IsHaveCreatGem, bool IsHaveMayaStone) MustItem;
            Log.DebugGreen($"加鲁达之羽合成：{CheckItems.Count}");
            //标题
            AddTextTitle("加鲁达之羽合成");
            ///必须材料
            AddMustItemInfoText("三代翅膀/披风(+13以上追4以上)\t\tx1", MustItem.IsHaveWing_3 = IsHaveWing_3());
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuGem = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 30, out int count), isEnough: count >= 30, str: $"祝福宝石\t\tx30({count})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunGem = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 30, out int linghuncount), isEnough: linghuncount >= 30, str: $"灵魂宝石\t\tx30({linghuncount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveCreatGem = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(), 30, out int creatcount), isEnough: creatcount >= 30, str: $"创造宝石\t\tx30({creatcount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 30, out int mayacount), isEnough: mayacount >= 30, str: $"玛雅宝石\t\tx30({mayacount})");
            //可选材料
            //AddSubItemInfoText("高级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("高级魔晶石(+5%) 可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.High_LEVEL_MOJING_STONE.ToInt64(), (long)100000, 5, false));
            //AddSubItemInfoText("灵魂宝石(+3%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), (long)100000, 3, false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            // AddSubItemInfoText("幸运符咒(+1~10%) 可选 x1~10", IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            // AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            IsCanMerger = MustItem == (true, true, true, true,true);
            return CheckItemCount();

            ///是否有三代翅膀 (+13 追4)
            bool IsHaveWing_3()
            {
                bool isHave = false;
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    var item = CheckItems[i];
                    if (Wing_3_Dic.ContainsKey(item.ConfigId) && item.GetProperValue(E_ItemValue.Level) >=13 && item.OptLev >= 1)
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