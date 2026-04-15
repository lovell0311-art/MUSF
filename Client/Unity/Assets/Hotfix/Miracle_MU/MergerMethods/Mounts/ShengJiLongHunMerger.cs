using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 圣迹龙魂（赤耀）合成
    /// </summary>
    [MergerSystem(214)]
    public class ShengJiLongHunMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 2_000_000;
            SuccessRate = 60;
            MaxSuccessRate = 80;
            FailedDelete = true;
            mergerMethodId = 10076;
            mergerMethod = "MarkDragonSoulSynthesis";
            (bool IsHaveYouLingZhanMa, bool IsLongHun, bool IsHaveLongHunZhiYing, bool IsHaveZhuFuBaoShi, bool IsHaveLingHunBaoShi, bool IsHaveChuangZaoBaoShi, bool IsHaveMayaStone) MustItem;
           
            //标题
            AddTextTitle("圣迹龙魂（赤耀）合成");
            ///必须材料
            AddMustItemInfoText("烈火战马\t\tx1", MustItem.IsHaveYouLingZhanMa = IsHaveItem(itemConfigId: 260021));
            AddMustItemInfoText("龙魂之印\t\tx1", MustItem.IsHaveLongHunZhiYing = IsHaveItem(itemConfigId: 320313));
            AddMustItemInfoText(isHave: MustItem.IsLongHun = IsHaveItem(itemConfigId: 320315, 10, out int fhsp), isEnough: fhsp >= 10, str: $"复活碎片\t\tx10({fhsp})");
            AddMustItemInfoText(isHave: MustItem.IsHaveZhuFuBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), 20, out int zhufucount), isEnough: zhufucount >= 20, str: $"祝福宝石\t\tx20({zhufucount})");
            AddMustItemInfoText(isHave: MustItem.IsHaveLingHunBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), 20, out int linghuncount), isEnough: linghuncount >= 20, str: $"灵魂宝石\t\tx20({linghuncount})");
            AddMustItemInfoText("创造宝石\t\tx1", MustItem.IsHaveChuangZaoBaoShi = IsHaveItem(itemConfigId: GemItemConfigId.CREATE_GEMS.ToInt64()));
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            ;
            AddSubItemInfoText($"生命宝石(+6%)  可选 x1", IsHaveGuDing(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), 1, 6, false));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
          //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            IsCanMerger = MustItem == (true, true, true, true, true, true,true);
            return CheckItemCount();
        }
    }
}
