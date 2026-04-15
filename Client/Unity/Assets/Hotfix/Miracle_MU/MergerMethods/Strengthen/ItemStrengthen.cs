using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 装备强化
    /// </summary>
    [MergerSystem(401)]
    public class ItemStrengthen : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            KnapsackDataItem knapsackDataItem = null;
            int count = 1;
            IsCanMerger = true;
            Money = 2_000_000;
            SuccessRate = 60;
            MaxSuccessRate = 85;
            FailedDelete = true;
            mergerMethodId = 10032;
            mergerMethod = "EquipmentStrengthenSynthesis";
            (bool IsHaveZheDuanDeJiao, bool IsHaveMayaStone, bool IsHaveZhuFu, bool IsHaveLingHun) MustItemQHAddNine;
            (bool IsHaveZheDuanDeJiao, bool IsHaveCan) MustItemQH;
            (bool IsHaveZheDuanDeJiao, bool IsHaveLingHun) MustItemQHAddSix;
            (bool QH, bool QHAddSix, bool QHAddNine) MustItemType;
            //标题
            AddTextTitle("装备强化");
            if (MustItemType.QHAddNine = IsHaveFangJuZhuangBeiLevelAddNine())
            {
                AddMustItemInfoText("普通品质武器/防具装备\t\tx1", MustItemQHAddNine.IsHaveZheDuanDeJiao = MustItemType.QHAddNine);
                AddMustItemInfoText("玛雅之石\t\tx1", MustItemQHAddNine.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
                //AddSubItemInfoText("祝福宝石\t\tx1", IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64()));
                AddMustItemInfoText(isHave: MustItemQHAddNine.IsHaveZhuFu = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), count, out int zhufucount), isEnough: zhufucount >= count, str: $"祝福宝石\t\tx{count}({zhufucount})");
                AddMustItemInfoText(isHave: MustItemQHAddNine.IsHaveLingHun = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), count, out int linghuncount), isEnough: linghuncount >= count, str: $"灵魂宝石\t\tx{count}({linghuncount})");
                //可选材料
                AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
               // AddSubItemInfoText("物品保护符咒x1或者保护符咒x1", IsHaveItem(itemConfigId: 320141, addSuccessrateValue: 1, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
              //  AddSubItemInfoText("物品保护符咒\t\tx1或者保护符咒\t\tx1", IsHaveItem(itemConfigId: 320141)|| IsHaveItem(itemConfigId: 320318));
                AddSubItemInfoText("强化装备保护符咒\t\tx1", IsHaveItem(itemConfigId: 320141));
                IsCanMerger = MustItemQHAddNine == (true, true, true, true);
            }
            else if (MustItemType.QHAddSix = IsHaveFangJuZhuangBeiLevelAddSix())
            {
                AddMustItemInfoText("普通品质武器/防具装备\t\tx1", MustItemQHAddSix.IsHaveZheDuanDeJiao = MustItemType.QHAddSix);
                AddMustItemInfoText("灵魂宝石\t\tx1", MustItemQHAddSix.IsHaveLingHun = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64()));
                // AddSubItemInfoText("物品保护符咒\t\tx1", IsHaveItem(itemConfigId: 320141));
                //   AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
                //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
                AddSubItemInfoText("强化装备保护符咒\t\tx1", IsHaveItem(itemConfigId: 320141));
                IsCanMerger = MustItemQHAddSix == (true, true);
            }
            else if (MustItemType.QH = IsHaveFangJuZhuangBeiLevel())
            {
                bool IsHaveZhuFu = false,IsHaveLinHun = false;
                AddMustItemInfoText("普通品质武器/防具装备\t\tx1", MustItemQH.IsHaveZheDuanDeJiao = MustItemType.QH);
                AddSubItemInfoText("强化装备保护符咒\t\tx1", IsHaveItem(itemConfigId: 320141));
                if (!IsHaveGEMS(GemItemConfigId.SOUL_GEMS))
                    AddMustItemInfoText("祝福宝石\t\tx1", IsHaveZhuFu = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64()));
                if (!IsHaveGEMS(GemItemConfigId.BLESSING_GEMS))
                    AddMustItemInfoText("灵魂宝石\t\tx1", IsHaveLinHun = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64()));
                MaxSuccessRate = 100;
                SuccessRate = 0;
                if (IsHaveZhuFu)
                {
                    int index = knapsackDataItem.GetProperValue(E_ItemValue.Level);
                   
                    mergerMethodId = 10064 + index;
                    Money = 0;
                    mergerMethod = $"EquipmentStrengthenSynthesis{index + 1}_{index + 1}";
                    SuccessRate = 100;
                    RemoveText(2,GemItemConfigId.SOUL_GEMS.ToInt64());
                }
                else if (IsHaveLinHun)
                {
                    int index = knapsackDataItem.GetProperValue(E_ItemValue.Level);
                    mergerMethodId = 10055 + index;
                    Money = 0;
                    mergerMethod = $"EquipmentStrengthenSynthesis{index + 1}";
                    SuccessRate = 50;
                }
                MustItemQH.IsHaveCan = IsHaveZhuFu || IsHaveLinHun;
                IsCanMerger = MustItemQH == (true,true);
            }
            return CheckItemCount();

            ///普通品质防具装备+9以上
            bool IsHaveFangJuZhuangBeiLevelAddNine()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    //Log.DebugBrown($"CheckItems[i].Slot:{CheckItems[i].ItemType}");
                    if ((CheckItems[i].ItemType <= (int)E_ItemType.Wing && CheckItems[i].ItemType >= (int)E_ItemType.Shields) || (CheckItems[i].ItemType >= (int)E_ItemType.Swords && CheckItems[i].ItemType <= (int)E_ItemType.MagicGun) || CheckItems[i].ItemType == (int)E_ItemType.WristBand
                       || CheckItems[i].ItemType == (int)E_ItemType.QiZhi || CheckItems[i].ItemType == (int)E_ItemType.Guard || CheckItems[i].ItemType == (int)E_ItemType.Rings || CheckItems[i].ItemType == (int)E_ItemType.Pet
                       || CheckItems[i].ItemType == (int)E_ItemType.Necklace|| CheckItems[i].ItemType == (int)E_ItemType.Rings || CheckItems[i].ItemType == (int)E_ItemType.WristBand)
                    {
                        if (CheckItems[i].GetProperValue(E_ItemValue.Level) >= 9 && CheckItems[i].GetProperValue(E_ItemValue.Level) < 15)
                        {
                            //标题
                            AddTextTitle("装备强化(+9~+15)");
                            count = CheckItems[i].GetProperValue(E_ItemValue.Level) - 8;
                            mergerMethodId = 10032 + CheckItems[i].GetProperValue(E_ItemValue.Level) - 9;
                            Money = (CheckItems[i].GetProperValue(E_ItemValue.Level) - 8) * 2000000;
                            SuccessRate = GetSuccessRate(CheckItems[i].GetProperValue(E_ItemValue.Level),true);
                            MaxSuccessRate = GetMaxSuccessRate(CheckItems[i].GetProperValue(E_ItemValue.Level));
                            mergerMethod = $"EquipmentStrengthenSynthesis{CheckItems[i].GetProperValue(E_ItemValue.Level) + 1}";
                            //Log.DebugBrown($"CheckItems[i].Slot:{CheckItems[i].ItemType}");
                            if (CheckItems[i].GetProperValue(E_ItemValue.LuckyEquip) is int value && value != 0)
                            {
                                AddSuccessRate(5);
                            }
                            CheckItems.RemoveAt(i);
                            return true;
                        }
                    }
                }
                return false;
            }
            int GetSuccessRate(int level,bool s) => level switch
            {
                9 => s ? 55 : 40,
                10 => s ? 50 : 40,
                11 => s ? 45 : 35,
                12 => s ? 40 : 35,
                13 => s ? 35 : 30,
                14 => s ? 30 : 30,
                _ => 0
            };
            int GetMaxSuccessRate(int level) => level switch
            {
                9 => 70,
                10 => 65,
                11 => 60,
                12 => 55,
                13 => 50,
                14 => 45,
                _ => 0
            };

            ///普通品质防具装备+6以上
            bool IsHaveFangJuZhuangBeiLevelAddSix()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if ((CheckItems[i].ItemType <= (int)E_ItemType.Wing && CheckItems[i].ItemType >= (int)E_ItemType.Shields) || (CheckItems[i].ItemType >= (int)E_ItemType.Swords && CheckItems[i].ItemType <= (int)E_ItemType.MagicGun) || CheckItems[i].ItemType == (int)E_ItemType.WristBand
                        || CheckItems[i].ItemType == (int)E_ItemType.QiZhi || CheckItems[i].ItemType == (int)E_ItemType.Guard || CheckItems[i].ItemType == (int)E_ItemType.Rings || CheckItems[i].ItemType == (int)E_ItemType.Pet 
                        || CheckItems[i].ItemType == (int)E_ItemType.Necklace || CheckItems[i].ItemType == (int)E_ItemType.Rings || CheckItems[i].ItemType == (int)E_ItemType.WristBand)
                    {
                        if (CheckItems[i].GetProperValue(E_ItemValue.Level) >= 6 && CheckItems[i].GetProperValue(E_ItemValue.Level) < 9)
                        {
                            //标题
                            AddTextTitle("装备强化(+6~+9)");
                            mergerMethodId = 10061 + CheckItems[i].GetProperValue(E_ItemValue.Level) - 6;
                            Money = 0;
                            MaxSuccessRate = SuccessRate = CheckItems[i].GetProperValue(E_ItemValue.LuckyEquip) != 0 ? 75 : 50;
                            mergerMethod = $"EquipmentStrengthenSynthesis{CheckItems[i].GetProperValue(E_ItemValue.Level) + 1}";
                            if (CheckItems[i].GetProperValue(E_ItemValue.LuckyEquip) is int value && value != 0)
                            {
                                AddSuccessRate(5);
                            }
                            CheckItems.RemoveAt(i);
                            return true;
                        }
                    }
                }
                return false;
            }
            ///普通品质防具装备
            bool IsHaveFangJuZhuangBeiLevel()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if ((CheckItems[i].ItemType <= (int)E_ItemType.Wing && CheckItems[i].ItemType >= (int)E_ItemType.Shields) || (CheckItems[i].ItemType >= (int)E_ItemType.Swords && CheckItems[i].ItemType <= (int)E_ItemType.MagicGun) || CheckItems[i].ItemType == (int)E_ItemType.WristBand
                        || CheckItems[i].ItemType == (int)E_ItemType.QiZhi || CheckItems[i].ItemType == (int)E_ItemType.Guard || CheckItems[i].ItemType == (int)E_ItemType.Rings || CheckItems[i].ItemType == (int)E_ItemType.Pet 
                        || CheckItems[i].ItemType == (int)E_ItemType.Necklace || CheckItems[i].ItemType == (int)E_ItemType.Rings|| CheckItems[i].ItemType == (int)E_ItemType.WristBand)
                    {
                        //标题
                        AddTextTitle("装备强化(+1~+6)");
                        knapsackDataItem = CheckItems[i];
                        if (CheckItems[i].GetProperValue(E_ItemValue.LuckyEquip) is int value && value != 0)
                        {
                            AddSuccessRate(5);
                        }
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }

            bool IsHaveGEMS(long configId)
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if (CheckItems[i].ConfigId == configId)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
