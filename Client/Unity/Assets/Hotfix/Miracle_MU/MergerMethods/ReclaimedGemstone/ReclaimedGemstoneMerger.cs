using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 再生宝石
    /// </summary>
    [MergerSystem(1001)]
    public class ReclaimedGemstoneMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 50_000;
            SuccessRate = 80;
            MaxSuccessRate = 80;
            FailedDelete = true;
            mergerMethodId = 10045;
            mergerMethod = "RegenGemstones";
            bool IsHavePuTong;
            //标题


            AddTextTitle("再生宝石");
            ///必须材料
            AddMustItemInfoText("再生原石\t\tx1", IsHavePuTong = IsHaveItem(itemConfigId : GemItemConfigId.ORECYCLED_GEMS.ToInt64()));

            IsCanMerger = IsHavePuTong;

            return CheckItemCount();
        }
    }

    /// <summary>
    /// 初级进化宝石生成
    /// </summary>
    [MergerSystem(903)]
    public class DReclaimedGemstoneMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 150_000;
            SuccessRate = 20;
            MaxSuccessRate = 20;
            FailedDelete = true;
            mergerMethodId = 10046;
            mergerMethod = "PEvolutionGem";
            bool IsHavePuTong;
            //标题
            AddTextTitle("初级进化宝石");
            ///必须材料
            AddMustItemInfoText("普通品质武器/防具装备\t\tx1", IsHavePuTong = IsHaveItemAddSeven());
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));

            IsCanMerger = IsHavePuTong;

            ///是否是普通品质武器/防具装备
            bool IsHaveItemAddSeven()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if ((CheckItems[i].ItemType <= (int)E_ItemType.Boots && CheckItems[i].ItemType >= (int)E_ItemType.Shields) || (CheckItems[i].ItemType >= (int)E_ItemType.Swords && CheckItems[i].ItemType <= (int)E_ItemType.MagicGun))
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

    [MergerSystem(902)]
    public class MReclaimedGemstoneMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 150_000;
            SuccessRate = 50;
            MaxSuccessRate = 50;
            FailedDelete = true;
            mergerMethodId = 10047;
            mergerMethod = "AEvolutionGem";
            bool IsHavePuTong;
            //标题
            AddTextTitle("高级进化宝石");
            ///必须材料
            AddMustItemInfoText("卓越品质装备\t\tx1", IsHavePuTong = IsExcellence());
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));
            IsCanMerger = IsHavePuTong;

            ///是否是卓越品质武器/防具装备
            bool IsExcellence()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if ((CheckItems[i].ItemType <= (int)E_ItemType.Boots && CheckItems[i].ItemType >= (int)E_ItemType.Shields) || (CheckItems[i].ItemType >= (int)E_ItemType.Swords && CheckItems[i].ItemType <= (int)E_ItemType.MagicGun))
                    {
                        if (CheckItems[i].IsHaveExecllentEntry)
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
