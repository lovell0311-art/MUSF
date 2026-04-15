using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 果实合成
    /// </summary>
    [MergerSystem(902)]
    public class PetMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            long index = 0;
            IsCanMerger = true;
            Money = 1_000_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10080;
            mergerMethod = "AdvancedPet1";
            (bool ISHaveChengGuang1, bool IsHaveChengGuang2,bool IsHaveMaYa,bool IsHaveCreatGem,bool IsHaveZaiSheng) MustItem;
            //标题
            AddTextTitle("宠物进阶");
            ///必须材料
            MustItem.IsHaveMaYa = false;
            MustItem.IsHaveCreatGem = false;
            MustItem.IsHaveZaiSheng = false;
            AddMustItemInfoText("宠物1\t\tx1", MustItem.ISHaveChengGuang1 = IsHaveFangZhuZhuangBei());
            AddMustItemInfoText("宠物2\t\tx1", MustItem.IsHaveChengGuang2 = IsHaveFangZhuZhuangBei2());

            IsCanMerger = MustItem == (true, true, true, true, true);
            ///普通品质武器装备
            bool IsHaveFangZhuZhuangBei()
            {
                for (int i = 0; i < CheckItems.Count; i++)
                {
                  //  Log.DebugBrown("进阶" + "index" + i + CheckItems[i].item_Info.Name + ":类型" + CheckItems[i].ItemType + ":id" + CheckItems[i].ConfigId);
                    if (CheckItems[i].ItemType == (int)E_ItemType.Pet)
                    {
                        Log.DebugGreen($"{ConfigComponent.Instance.GetItem<Item_PetConfig>(CheckItems[i].ConfigId.ToInt32()).Type}");
                        if (ConfigComponent.Instance.GetItem<Item_PetConfig>(CheckItems[i].ConfigId.ToInt32()).Type == 0)
                        {
                            AddMustItemInfoText(isHave: MustItem.IsHaveMaYa = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 10, out int mayacount), isEnough: mayacount >= 10, str: $"玛雅宝石\t\tx10({mayacount})");
                            AddTextTitle("宠物进阶1");
                            Money = 1_000_000;
                            mergerMethodId = 10080;
                            mergerMethod = "AdvancedPet1";
                            MustItem.IsHaveCreatGem = true;
                            MustItem.IsHaveZaiSheng = true;
                        }
                        else
                        if (ConfigComponent.Instance.GetItem<Item_PetConfig>(CheckItems[i].ConfigId.ToInt32()).Type == 1)
                        {
                            AddMustItemInfoText(isHave: MustItem.IsHaveMaYa = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 20, out int mayacount), isEnough: mayacount >= 20, str: $"玛雅宝石\t\tx20({mayacount})");
                            AddMustItemInfoText(isHave: MustItem.IsHaveCreatGem = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(), 10, out int creatcount), isEnough: creatcount >= 10, str: $"创造宝石\t\tx10({creatcount})");
                            AddTextTitle("宠物进阶2");
                            Money = 5_000_000;
                            mergerMethodId = 10081;
                            mergerMethod = "AdvancedPet2";
                            MustItem.IsHaveZaiSheng = true;
                        }
                        else if (ConfigComponent.Instance.GetItem<Item_PetConfig>(CheckItems[i].ConfigId.ToInt32()).Type == 2)
                        {
                            AddMustItemInfoText(isHave: MustItem.IsHaveMaYa = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 30, out int mayacount), isEnough: mayacount >= 30, str: $"玛雅宝石\t\tx30({mayacount})");
                            AddMustItemInfoText(isHave: MustItem.IsHaveCreatGem = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(), 20, out int creatcount), isEnough: creatcount >= 20, str: $"创造宝石\t\tx20({creatcount})");
                            AddMustItemInfoText(isHave: MustItem.IsHaveZaiSheng = IsHaveItem(itemConfigId: GemItemConfigId.RECYCLED_GEMS.ToInt64(), 10, out int rECYCLED_GEMS), isEnough: rECYCLED_GEMS >= 10, str: $"再生宝石\t\tx10({rECYCLED_GEMS})");
                            AddTextTitle("宠物进阶3");

                            mergerMethodId = 10082;
                            Money = 10_000_000;
                            mergerMethod = "AdvancedPet3";
                        }
                        else
                        {
                            return false;
                        }
                        index = CheckItems[i].ConfigId;

                        CheckItems.RemoveAt(i);
                        return true;
                    }
                }
                //for (int i = CheckItems.Count - 1; i >= 0; i--)
                //{
                //    Log.DebugBrown("进阶"+"index"+i + CheckItems[i].item_Info.Name+":类型"+ CheckItems[i].ItemType+":id"+ CheckItems[i].ConfigId);
                //    if (CheckItems[i].ItemType == (int)E_ItemType.Pet)
                //    {
                //        Log.DebugBrown("kk" + i);
                //        Log.DebugGreen($"{ConfigComponent.Instance.GetItem<Item_PetConfig>(CheckItems[i].ConfigId.ToInt32()).Type}");
                //        if (ConfigComponent.Instance.GetItem<Item_PetConfig>(CheckItems[i].ConfigId.ToInt32()).Type == 0)
                //        {
                //            AddMustItemInfoText(isHave: MustItem.IsHaveMaYa = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 10, out int mayacount), isEnough: mayacount >= 10, str: $"玛雅宝石\t\tx10({mayacount})");
                //            AddTextTitle("宠物进阶1");
                //            Money = 1_000_000;
                //            mergerMethodId = 10080;
                //            mergerMethod = "AdvancedPet1";
                //            MustItem.IsHaveCreatGem = true;
                //            MustItem.IsHaveZaiSheng = true;
                //        }else
                //        if (ConfigComponent.Instance.GetItem<Item_PetConfig>(CheckItems[i].ConfigId.ToInt32()).Type == 1)
                //        {
                //            AddMustItemInfoText(isHave: MustItem.IsHaveMaYa = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 20, out int mayacount), isEnough: mayacount >= 20, str: $"玛雅宝石\t\tx20({mayacount})");
                //            AddMustItemInfoText(isHave: MustItem.IsHaveCreatGem = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(), 10, out int creatcount), isEnough: creatcount >= 10, str: $"创造宝石\t\tx10({creatcount})");
                //            AddTextTitle("宠物进阶2");
                //            Money = 5_000_000;
                //            mergerMethodId = 10081;
                //            mergerMethod = "AdvancedPet2";
                //            MustItem.IsHaveZaiSheng = true;
                //        }
                //        else if(ConfigComponent.Instance.GetItem<Item_PetConfig>(CheckItems[i].ConfigId.ToInt32()).Type == 2)
                //        {
                //            AddMustItemInfoText(isHave: MustItem.IsHaveMaYa = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 30, out int mayacount), isEnough: mayacount >= 30, str: $"玛雅宝石\t\tx30({mayacount})");
                //            AddMustItemInfoText(isHave: MustItem.IsHaveCreatGem = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(), 20, out int creatcount), isEnough: creatcount >= 20, str: $"创造宝石\t\tx20({creatcount})");
                //            AddMustItemInfoText(isHave: MustItem.IsHaveZaiSheng = IsHaveItem(itemConfigId: GemItemConfigId.RECYCLED_GEMS.ToInt64(), 10, out int rECYCLED_GEMS), isEnough: rECYCLED_GEMS >= 10, str: $"再生宝石\t\tx10({rECYCLED_GEMS})");
                //            AddTextTitle("宠物进阶3");

                //            mergerMethodId = 10082;
                //            Money = 10_000_000;
                //            mergerMethod = "AdvancedPet3";
                //        }
                //        else
                //        {
                //            return false;
                //        }
                //        index = CheckItems[i].ConfigId;
                       
                //        CheckItems.RemoveAt(i);
                //        return true;
                //    }
                //}
                return false;
            }
            ///普通品质武器装备
            bool IsHaveFangZhuZhuangBei2()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if (CheckItems[i].ItemType == (int)E_ItemType.Pet)
                    {
                        if (index == CheckItems[i].ConfigId)
                        {
                            CheckItems.RemoveAt(i);
                            return true;
                        }

                    }
                }
                return false;
            }
            return CheckItemCount();

        }
    }
}