using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
   
    [MergerSystem(501)]
    public class HDemonMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 50_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10050;
            mergerMethod = "AMagicStone";
            bool IsHavePuTong;
            //标题


            AddTextTitle("高级魔晶石");
            ///必须材料
            AddMustItemInfoText("套装品质装备(+7以上/追4以上)\t\tx1", IsHavePuTong = IsSuitItem());

            IsCanMerger = IsHavePuTong;

            ///是否套装品质装备 (+7以上/追4以上)
            bool IsSuitItem()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    Log.DebugGreen($"装备等级{CheckItems[i].GetProperValue(E_ItemValue.Level)} 装备追加{CheckItems[i].OptLev} 套装ID{CheckItems[i].GetProperValue(E_ItemValue.SetId)}");
                    if (CheckItems[i].GetProperValue(E_ItemValue.Level) >= 7 && CheckItems[i].OptLev >= 1 && CheckItems[i].GetProperValue(E_ItemValue.SetId) != 0)
                    {
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
            return CheckItemCount();
        }
    }
    
    /// <summary>
    /// 魔晶石
    /// </summary>
    [MergerSystem(503)]
    public class DemonMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 50_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10048;
            mergerMethod = "LMagicStone";
            bool IsHavePuTong;
            //标题
            AddTextTitle("低级魔晶石");
            ///必须材料
            AddMustItemInfoText("普通品质装备(+6以上/追4以上)\t\tx1", IsHavePuTong = IsHaveItemAddSeven());

            IsCanMerger = IsHavePuTong;

            ///是否是 (+7以上/追4以上)
            bool IsHaveItemAddSeven()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if (CheckItems[i].GetProperValue(E_ItemValue.Level) >= 6 && CheckItems[i].OptLev >= 1)
                    {
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }
            return CheckItemCount();
        }
    }

    [MergerSystem(502)]
    public class MDemonMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 50_0;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10049;
            mergerMethod = "IMagicStone";
            bool IsHavePuTong;
            //标题
            AddTextTitle("中级魔晶石");
            ///必须材料
            AddMustItemInfoText("卓越品质装备(+7以上/追4以上)\t\tx1", IsHavePuTong = IsExcellence());

            IsCanMerger = IsHavePuTong;

            ///是否是卓越装备 (+7以上/追4以上)
            bool IsExcellence()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if (CheckItems[i].GetProperValue(E_ItemValue.Level) >= 7 && CheckItems[i].OptLev >= 1 && CheckItems[i].IsHaveExecllentEntry)
                    {
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                }
                return false;
            }

            return CheckItemCount();
        }
    }
    
}
