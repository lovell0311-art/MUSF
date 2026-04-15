using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;
using ILRuntime.Runtime;
using System.Linq;
using NPOI.SS.UserModel;

namespace ETHotfix
{
    /// <summary>
    /// 三代翅膀、披风 合成
    /// </summary>
    [MergerSystem(303)]
    public class MergerWing_3 : MergerMethod
    {
        /// <summary>
        /// 3代翅膀合成保护符咒
        /// </summary>
        readonly Dictionary<long, string> HeChengFuChou3Dic = new Dictionary<long, string>
        {
            { 320422,"暴风之翼"},
            { 320423,"幻影之翼"},
            { 320424,"时空之翼"},
            { 320425,"破灭之翼"},
            { 320426,"幻灭之翼"}, 
            { 320427,"次元之翼"},
            { 320428,"沉默之翼"},
            { 320429,"帝王披风"},
            { 320430,"斗皇披风"},
            { 320431,"超越披风"}

        };
        /// <summary>
        /// 2.5代翅膀
        /// </summary>
        readonly Dictionary<long, string> Wing2_5Dai3Dic = new Dictionary<long, string>
        {
            { 220039 ,"混沌之翼"},
            { 220037 ,"生命之翼"},
            { 220038 ,"魔力之翼"},
            { 220040 ,"死亡披风"}
        };
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 2_000;
            SuccessRate = 5;
            MaxSuccessRate = 40;
            FailedDelete = true;
            mergerMethodId = 10012;
            mergerMethod = "ThirdWingSynthesis";
            (bool ShenYingZhiYu, bool IsHaveShenYingHuoZhong, bool IsHaveZhuFuGem, bool IsHaveLingHunGem, bool IsHaveCreatGem, bool IsHaveMayaStone) MustItem;
            Log.DebugGreen($"死亡合成：{CheckItems.Count}");
            //标题
            AddTextTitle("三代翅膀/披风合成");
            ///必须材料
            AddMustItemInfoText("神鹰之羽或者2.5代翅膀\t\tx1", MustItem.ShenYingZhiYu = IsHaveItem(itemConfigId: 320020) || IsHave2_5Dai());
            AddMustItemInfoText("神鹰火种\t\tx1", MustItem.IsHaveShenYingHuoZhong = IsHaveItem(itemConfigId: 320019));
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuGem = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(),30,out int count),isEnough:count>=30,str: $"祝福宝石\t\tx30({count})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunGem = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(),30,out int linghuncount), isEnough: linghuncount >= 30, str: $"灵魂宝石\t\tx30({linghuncount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveCreatGem = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(),30,out int creatcount),isEnough: creatcount >= 30, str: $"创造宝石\t\tx30({creatcount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(),30,out int mayacount),isEnough: mayacount >= 30, str: $"玛雅宝石\t\tx30({mayacount})");
            //可选材料
            //AddSubItemInfoText("中级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            AddSubItemInfoText("高级魔晶石(+5%) 可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.High_LEVEL_MOJING_STONE.ToInt64(), (long)100000, 5, false));
            //  AddSubItemInfoText("幸运符咒(+1~10%) 可选 x1~10", IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            IsHaveHeChengFuChou();
            IsCanMerger = MustItem == (true, true, true, true,true,true);
            return CheckItemCount();

            void IsHaveHeChengFuChou()
            {
                for (int i = 0, length = HeChengFuChou3Dic.Count; i < length; i++)
                {
                    AddSubItemInfoText($"{HeChengFuChou3Dic.ElementAt(i).Value}合成符咒 可选 x1", IsHaveItem(itemConfigId: HeChengFuChou3Dic.ElementAt(i).Key, addSuccessrateValue: 0, IsMust: false));
                    if (IsHaveItem(itemConfigId: HeChengFuChou3Dic.ElementAt(i).Key, addSuccessrateValue: 0, IsMust: false))
                    {
                        AddTextTitle($"三代翅膀({HeChengFuChou3Dic.ElementAt(i).Value})合成");
                        break;
                    }
                }
            }
            bool IsHave2_5Dai()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    var item = CheckItems[i];
                    if (Wing2_5Dai3Dic.ContainsKey(item.ConfigId))
                    {
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
