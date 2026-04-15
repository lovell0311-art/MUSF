using ETModel;
using ILRuntime.Runtime;
using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// 炎狼兽之角 破坏
    /// </summary>
    [MergerSystem(204)]
    public class YanLangShouZhiJiao_PoHuai_Merger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 2_000_00;
            SuccessRate = 30;
            MaxSuccessRate = 79;
            FailedDelete = true;
            mergerMethodId = 10019;
            mergerMethod = "YanLangShouPoHuaiSynthesis";
            (bool IsHaveNormalWeaponItem, bool IsHaveShengMingGem, bool IsHaveMayaStone) MustItem;
           
            //标题
            AddTextTitle("炎狼兽之角+破坏合成");
            ///必须材料
            AddMustItemInfoText("普通品质武器装备\t\tx1", MustItem.IsHaveNormalWeaponItem = IsHaveFangZhuZhuangBei());
            AddMustItemInfoText("炎狼兽之角\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: 260008));
            AddMustItemInfoText(isHave: MustItem.IsHaveShengMingGem = IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), 5, out int linghuncount), isEnough: linghuncount >= 5, str: $"生命宝石\t\tx5({linghuncount})");

            AddMustItemInfoText(isHave: MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 5, out int linghuncount1), isEnough: linghuncount1 >= 5, str: $"玛雅之石\t\tx5({linghuncount1})");
            //AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            //可选材料320400
            //AddSubItemInfoText("魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            //AddSubItemInfoText("普通品质武器装备 可选 x1", IsHaveFangZhuZhuangBei());
            //AddSubItemInfoText("低级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.LOW_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("中级魔晶石(+5%) 可选 xn", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
          //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            //AddSubItemInfoText("高级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.High_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            //AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            IsCanMerger = MustItem == (true, true, true);
            return CheckItemCount();

            ///普通品质武器装备
            bool IsHaveFangZhuZhuangBei() 
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if (CheckItems[i].ItemType >= (int)E_ItemType.Swords&& CheckItems[i].ItemType<= (int)E_ItemType.MagicGun)
                    {
                        SuccessRate += CheckItems[i].GetProperValue(E_ItemValue.Level) * 2 + CheckItems[i].OptLev * 3 + CheckItems[i].ExecllentEntryDic.Count * 7;//卓越属性;
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
