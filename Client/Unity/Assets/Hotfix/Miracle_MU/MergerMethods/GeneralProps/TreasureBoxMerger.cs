using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 黄金宝箱合成
    /// </summary>
    [MergerSystem(107)]
    public class TreasureBoxMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 200_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10072;
            mergerMethod = "GoldChestSynthesis";
            bool IsHavePuTong;
            //标题


            AddTextTitle("白银兑换黄金宝箱");
            ///必须材料
            AddMustItemInfoText(isHave: IsHavePuTong = IsHaveItem(itemConfigId: 320407, 5, out int curCount), isEnough: curCount >= 5, str: $"白银宝箱\t\tx5({curCount})");
            IsCanMerger = IsHavePuTong;

            return CheckItemCount();
        }
    }
    /// <summary>
    /// 钻石宝箱合成
    /// </summary>
    [MergerSystem(108)]
    public class DiamondBoxMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 500_0;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10085;
            mergerMethod = "DiamondChestSynthesis";
            bool IsHavePuTong;
            //标题


            AddTextTitle("黄金兑换钻石宝箱");
            ///必须材料
            AddMustItemInfoText(isHave: IsHavePuTong = IsHaveItem(itemConfigId: 320408, 5, out int curCount), isEnough: curCount >= 5, str: $"黄金宝箱\t\tx5({curCount})");
            IsCanMerger = IsHavePuTong;

            return CheckItemCount();
        }
    }
}
