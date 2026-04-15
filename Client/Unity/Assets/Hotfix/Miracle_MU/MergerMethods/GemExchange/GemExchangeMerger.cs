using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 卓越兑换宝石
    /// </summary>
    [MergerSystem(108)]
    public class GemExchangeMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 500_0;
            SuccessRate = 30;
            MaxSuccessRate = 30;
            FailedDelete = true;
            mergerMethodId = 10051;
            mergerMethod = "EquipmentGemstones";
            bool IsHave = false;
            //标题
            AddTextTitle("卓越兑换宝石");
            ///必须材料
            AddMustItemInfoText("需要卓越品质装备\t\tx1", IsHave = IsHaveFangZhuZhuangBei());

            IsCanMerger = IsHave;


            ///普通品质武器装备

            bool IsHaveFangZhuZhuangBei()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if (CheckItems[i].IsHaveExecllentEntry)
                    {
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
            //bool IsHaveFangZhuZhuangBei()
            //{
            //    Log.DebugBrown("卓越兑换宝石");
            //    for (int i = CheckItems.Count - 1; i >= 0; i--)
            //    {
            //        if ((CheckItems[i].ItemType <= (int)E_ItemType.Boots && CheckItems[i].ItemType >= (int)E_ItemType.Shields) || (CheckItems[i].ItemType >= (int)E_ItemType.Swords && CheckItems[i].ItemType <= (int)E_ItemType.MagicGun))
            //        {
            //            if (CheckItems[i].ExecllentEntryDic.Count >= 1)
            //            {
            //                CheckItems.RemoveAt(i);
            //                return true;
            //            }

            //        }
            //    }
            //    return false;
            //}
            return CheckItemCount();

        }
    }
}
