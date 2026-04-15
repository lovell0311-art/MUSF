
using ETModel;
using ILRuntime.Runtime;
using NPOI.SS.Formula.Functions;

namespace ETHotfix
{
    /// <summary>
    /// 卓越装备属性随机
    /// </summary>
    [MergerSystem(1101)]
    public class ZhuoYueAttributesRandommergerMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            int count = 1;
            IsCanMerger = true;
            Money = 500_000;
            SuccessRate = 0;
            MaxSuccessRate = 0;
            FailedDelete = true;
            mergerMethodId = 10038;
            mergerMethod = "ChiyanshouSynthesis";
            (bool IsHaveZhuoYue, bool IsHaveLingHun, bool IsHaveZhuFu, bool IsHaveMoJingShi) MustItem;
            //标题


            AddTextTitle("增加卓越装备属性条数");
            ///必须材料
            AddMustItemInfoText("卓越装备\t\tx1", MustItem.IsHaveZhuoYue = IsHaveFangJuZhuangBeiLevel());
            AddMustItemInfoText("祝福宝石\t\tx1", MustItem.IsHaveZhuFu = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64()));
            AddMustItemInfoText("灵魂宝石\t\tx1", MustItem.IsHaveLingHun = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64()));
            AddMustItemInfoText("魔晶石\t\tx1", MustItem.IsHaveMoJingShi = IsHaveItem(itemConfigId: GemItemConfigId.LEVEL_MOJING_STONE.ToInt64()));
            //辅助材料
          //  AddSubItemInfoText("物品保护符咒\t\tx1", IsHaveItem(itemConfigId: 320141));

            IsCanMerger = MustItem == (true,true,true,true);
            ///卓越装备
            bool IsHaveFangJuZhuangBeiLevel()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if (CheckItems[i].ExecllentEntryDic.Count >= 1)
                    {
                        count = CheckItems[i].ExecllentEntryDic.Count;
                        mergerMethodId = 10039 + count;
                        Money *= count;
                        SuccessRate = MaxSuccessRate = GetMaxSuccessRate(count);
                        mergerMethod = $"InEquipmentNumber{count}";
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                    //if ((CheckItems[i].ItemType <= (int)E_ItemType.Staffs && CheckItems[i].ItemType >= (int)E_ItemType.Swords) || 
                    //    (CheckItems[i].ItemType >= (int)E_ItemType.Scepter && CheckItems[i].ItemType <= (int)E_ItemType.Dangler) ||
                    //    CheckItems[i].ItemType == (int)E_ItemType.Pet
                    //    )
                    //{
                    //    if (CheckItems[i].ExecllentEntryDic.Count >= 1)
                    //    {
                    //        count = CheckItems[i].ExecllentEntryDic.Count;
                    //        mergerMethodId = 10039 + count;
                    //        Money *= count;
                    //        SuccessRate = MaxSuccessRate = GetMaxSuccessRate(count);
                    //        mergerMethod = $"InEquipmentNumber{count}";
                    //        CheckItems.RemoveAt(i);
                    //        return true;
                    //    }
                    //}
                }
                return false;
            }
            int GetMaxSuccessRate(int count) => count switch
            {
                1 => 80,
                2 => 60,
                3 => 40,
                4 => 20,
                5 => 5,
                _ => 0
            };
            return CheckItemCount();
        }
    }
}
