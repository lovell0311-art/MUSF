using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 大天使旗帜合成
    /// </summary>
    [MergerSystem(605)]
    public class ArchangelSynthesisFlag : MergerMethod
    {
        /// <summary>
        ///套装指向合成符咒
        /// </summary>
        readonly Dictionary<long, string> HeChengFuChou3Dic = new Dictionary<long, string>
        {
            { 320462,"迪亚的合成符咒"},
            { 320463,"天启的合成符咒"},
            { 320464,"朵拉的合成符咒"},
            { 320465,"布加迪的合成符咒"},
            { 320466,"魅影的合成符咒"},
            { 320467,"迷茫的合成符咒"},
            { 320428,"破灭的合成符咒"},
           // { 320429,"帝王披风"},
          //  { 320430,"斗皇披风"},
          //  { 320431,"超越披风"}

        };
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 10_000_0;
            SuccessRate = 40;
            MaxSuccessRate = 45;
            FailedDelete = true;
            mergerMethodId = 10099;
            mergerMethod = "ArchangelSynthesisFlag";
            (bool IsHaveNormalWeaponItem, bool IsHaveShengMingGem, bool IsHaveMayaStone, bool NewIsHaveMayaStone) MustItem;

            //标题
            AddTextTitle("大天使旗帜合成");
            ///必须材料
           // AddMustItemInfoText("普通品质防具装备\t\tx1", MustItem.IsHaveNormalWeaponItem = IsHaveFangJuZhuangBei());
            AddMustItemInfoText("大天使合成符咒\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: 320449));
            AddMustItemInfoText(isHave: MustItem.IsHaveShengMingGem = IsHaveItem(itemConfigId: 320461, 20, out int linghuncount), isEnough: linghuncount >= 20, str: $"大天使旗帜碎片\t\tx20({linghuncount})");

            AddMustItemInfoText(isHave: MustItem.NewIsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 50, out int linghuncount1), isEnough: linghuncount1 >= 50, str: $"灵魂宝石\t\tx50({linghuncount1})");
            AddMustItemInfoText(isHave: MustItem.IsHaveNormalWeaponItem = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 80, out int linghuncount2), isEnough: linghuncount2 >= 80, str: $"祝福宝石\t\tx80({linghuncount2})");
            //AddMustItemInfoText("玛雅之石\t\tx1", MustItem.NewIsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            //可选材料
            //AddSubItemInfoText("普通品质防具装备 可选 x1", IsHaveFangJuZhuangBei());
            //AddSubItemInfoText("低级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.LOW_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
             AddSubItemInfoText("中级魔晶石(+5%) 可选 xn", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            //  AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64()));
            //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            //AddSubItemInfoText("高级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.High_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            //AddSubItemInfoText("魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
             AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            IsHaveHeChengFuChou();
            void IsHaveHeChengFuChou()
            {
                for (int i = 0, length = HeChengFuChou3Dic.Count; i < length; i++)
                {
                    AddSubItemInfoText($"{HeChengFuChou3Dic.ElementAt(i).Value} 可选 x1", IsHaveItem(itemConfigId: HeChengFuChou3Dic.ElementAt(i).Key, addSuccessrateValue: 0, IsMust: false));
                    if (IsHaveItem(itemConfigId: HeChengFuChou3Dic.ElementAt(i).Key, addSuccessrateValue: 0, IsMust: false))
                    {
                        AddTextTitle($"指向套装({HeChengFuChou3Dic.ElementAt(i).Value})合成");
                        break;
                    }
                }
            }
            IsCanMerger = MustItem == (true, true, true, true);
            //if (IsCanMerger==true)
            //{

            //    AddSuccessRate(50);
            //}
            return CheckItemCount();
        }
    }
}