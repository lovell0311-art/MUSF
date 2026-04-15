using ETModel;
using ILRuntime.Runtime;
namespace ETHotfix
{

    [MergerSystem(504)]
    public class GemDuiHuanMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 500_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10083;
            mergerMethod = "GemPrivilegeCard1";
            bool IsHaveGem;
            //标题
            AddTextTitle("宝石兑换月卡");
            ///必须材料
            AddMustItemInfoText("玛雅,创造,再生,祝福,灵魂,生命\t\tx1", IsHaveGem = 
                IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()) ||
                IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64())||
                IsHaveItem(itemConfigId: GemItemConfigId.RECYCLED_GEMS.ToInt64()) ||
                IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64()) ||
                IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64()) ||
                IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64())
                );

            IsCanMerger = IsHaveGem;

            return CheckItemCount();
        }
    } 
    [MergerSystem(505)]
    public class GemDuiHuanCangBaoTuMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_500_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10084;
            mergerMethod = "GemExchangeTreasureMap";
            bool IsHaveGem;
            //标题
            AddTextTitle("宝石兑换藏宝图");
            ///必须材料
            AddMustItemInfoText("玛雅*60/创造*60/再生*60/祝福*10/灵魂*10/生命*3", IsHaveGem =
                (IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), 60, out int curCount, 0, false) && curCount <= 60) ||
                (IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64(), 60, out int curCount1, 0, false) && curCount1 <= 60) ||
                (IsHaveItem(itemConfigId: GemItemConfigId.RECYCLED_GEMS.ToInt64(), 60, out int curCount2, 0, false) && curCount2 <= 60) ||
                (IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 10, out int curCount3, 0, false) && curCount3 <= 10) ||
                (IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 10, out int curCount4, 0, false) && curCount4 <= 10) ||
                (IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), 3, out int curCount5, 0, false) && curCount5 <= 3)
                );

            IsCanMerger = IsHaveGem;

            return CheckItemCount();
        }
    }
}
