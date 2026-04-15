using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 四代翅膀合成
    /// </summary>
    [MergerSystem(304)]
    public class MergerWing_4 : MergerMethod
    {

        /// <summary>
        ///四代指向合成符咒
        /// </summary>
        readonly Dictionary<long, string> HeChengFuChou3Dic = new Dictionary<long, string>
        {
            { 320441,"火神之翼合成符咒"},
            { 320442,"天体之翼合成符咒"},
            { 320443,"苍穹之翼合成符咒"},
            { 320444,"灭亡之翼合成符咒"},
            { 320445,"异界之翼合成符咒"},
            { 320446,"主宰披风合成符咒"},
            { 320447,"审判披风合成符咒"},
            { 320448,"超然披风合成符咒"},
          //  { 320430,"斗皇披风"},
          //  { 320431,"超越披风"}

        };
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 10_000_0;
            SuccessRate = 5;
            MaxSuccessRate = 40;
            FailedDelete = true;
            mergerMethodId = 10014;
            mergerMethod = "FourthWingSynthesis";
            (bool IsHaveJiaLuDaZhiYu, bool IsHaveJiaLuDaZhiHuoZhong, bool IsHaveHuangJinWenZhang, bool IsHaveZhuFuGem,  bool IsHaveLingHunGem, bool IsHaveCreatGem, bool IsHaveMayaStone) MustItem;
            Log.DebugGreen($"四代翅膀/披风合成：{CheckItems.Count}");
            //标题
            AddTextTitle("四代翅膀/披风合成");
            ///必须材料
            AddMustItemInfoText("加鲁达之羽\t\tx1", MustItem.IsHaveJiaLuDaZhiYu = IsHaveItem(320300)) ;
            AddMustItemInfoText("加鲁达之火种\t\tx1", MustItem.IsHaveJiaLuDaZhiHuoZhong = IsHaveItem(320301)) ;
            AddMustItemInfoText(isHave: MustItem.IsHaveHuangJinWenZhang = IsHaveItem(itemConfigId: 320299, 50, out int huangjinwenzhangcount), isEnough: huangjinwenzhangcount >= 50, str: $"黄金文章\t\tx50({huangjinwenzhangcount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuGem = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 30, out int count), isEnough: count >= 30, str: $"祝福宝石\t\tx30({count})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunGem = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 30, out int linghuncount), isEnough: linghuncount >= 30, str: $"灵魂宝石\t\tx30({linghuncount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveCreatGem = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(), 30, out int creatcount), isEnough: creatcount >= 30, str: $"创造宝石\t\tx30({creatcount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 30, out int mayacount), isEnough: mayacount >= 30, str: $"玛雅宝石\t\tx30({mayacount})");
            //可选材料
            // AddSubItemInfoText("高级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.High_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            IsHaveHeChengFuChou();
            void IsHaveHeChengFuChou()
            {
                for (int i = 0, length = HeChengFuChou3Dic.Count; i < length; i++)
                {
                    AddSubItemInfoText($"{HeChengFuChou3Dic.ElementAt(i).Value} 可选 x1", IsHaveItem(itemConfigId: HeChengFuChou3Dic.ElementAt(i).Key, addSuccessrateValue: 0, IsMust: false));
                    if (IsHaveItem(itemConfigId: HeChengFuChou3Dic.ElementAt(i).Key, addSuccessrateValue: 0, IsMust: false))
                    {
                        AddTextTitle($"指向翅膀({HeChengFuChou3Dic.ElementAt(i).Value})合成");
                        break;
                    }
                }
            }
            AddSubItemInfoText("高级魔晶石(+5%) 可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.High_LEVEL_MOJING_STONE.ToInt64(), (long)100000, 5, false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
          //  AddSubItemInfoText("幸运符咒(+1~10%) 可选 x1~10", IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
          //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            IsCanMerger = MustItem == (true, true, true, true, true,true,true);
            return CheckItemCount();
        }
    }
}
