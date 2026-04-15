
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 玛雅武器合成
    /// </summary>
    [MergerSystem(601)]
    public class MaYaWuQiMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 0;
            SuccessRate = 2;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10025;
            mergerMethod = "MayaWeaponSynthesis";
            (bool IsHaveZheDuanDeJiao, bool IsHaveMayaStone) MustItem;

            //标题
            AddTextTitle("玛雅武器合成");
            ///必须材料
            AddMustItemInfoText("普通品质武器/防具装备(+4以上/追4以上)\t\tx1", MustItem.IsHaveZheDuanDeJiao = IsHaveFangJuZhuangBeiLevel());
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            //可选材料
            //AddSubItemInfoText("普通品质武器/防具装备(+4以上/追4以上)\t\tx1", IsHaveFangJuZhuangBeiLevel());
            AddSubItemInfoText("祝福宝石\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), (long)100000,5, false));
            AddSubItemInfoText("灵魂宝石\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), (long)100000, 5, false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));
            IsCanMerger = MustItem == (true, true);
            return CheckItemCount();

            ///普通品质防具装备+4以上/追4以上
            bool IsHaveFangJuZhuangBeiLevel()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    Log.DebugBrown($"CheckItems[i].Slot:{CheckItems[i].ItemType}");
                    if ((CheckItems[i].ItemType <= (int)E_ItemType.Boots && CheckItems[i].ItemType >= (int)E_ItemType.Shields) || (CheckItems[i].ItemType >= (int)E_ItemType.Swords && CheckItems[i].ItemType <= (int)E_ItemType.MagicGun))
                    {
                        if (CheckItems[i].GetProperValue(E_ItemValue.Level) >= 4 && CheckItems[i].OptLev >= 1)
                        {

                            SuccessRate += CheckItems[i].GetProperValue(E_ItemValue.Level) * 2 + CheckItems[i].OptLev * 3 + CheckItems[i].ExecllentEntryDic.Count * 7;//卓越属性;
                            //Log.DebugBrown($"CheckItems[i].Slot:{CheckItems[i].ItemType}");
                            CheckItems.RemoveAt(i);
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
