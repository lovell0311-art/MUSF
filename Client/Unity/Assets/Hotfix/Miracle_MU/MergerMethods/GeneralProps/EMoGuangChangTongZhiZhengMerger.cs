using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 恶魔广场通行证
    /// </summary>
    [MergerSystem(101)]
    public class EMoGuangChangTongZhiZhengMerger : MergerMethod
    {
        int lev = 1;//默认是合成一级的恶魔广场通行证
        readonly long EMoZhiYanId = 320097;//恶魔之眼 ID
        readonly long EMoZhiYaoId = 320098;//恶魔之钥 ID
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = GetMoney();
            SuccessRate = 80;
            MaxSuccessRate = 80;
            FailedDelete = true;
            mergerMethodId = 10003;
            mergerMethod = "EmoGuangChangSynthesis";
          
            //是否有恶魔之眼、恶魔之钥、玛雅之石
            (bool IsHaveEMoZhiYan, bool IsHaveEMoZhiYao, bool IsHaveMayaStone) MustItem = (false, false, false);
            //标题
            AddTextTitle($"恶魔广场通行证 +{lev}");
            //必要材料
            //必要材料
            if (CheckItems.Count == 1)
            {
                MustItem.IsHaveEMoZhiYan = IsHaveItem(EMoZhiYanId, ref lev);
                MustItem.IsHaveEMoZhiYao = IsHaveItem(EMoZhiYaoId, ref lev);
                AddMustItemInfoText(isHave: MustItem.IsHaveEMoZhiYan, str: $"恶魔之眼+{lev}\t\tx1");
                AddMustItemInfoText(isHave: MustItem.IsHaveEMoZhiYao, str: $"恶魔之钥+{lev}\t\tx1");
            }
            else if (CheckItems.Count >= 2)
            {
                if (CheckItems[0].ConfigId == EMoZhiYanId|| CheckItems[1].ConfigId == EMoZhiYanId)
                {
                    AddMustItemInfoText(isHave: MustItem.IsHaveEMoZhiYan = IsHaveItem(EMoZhiYanId, ref lev), str: $"恶魔之眼+{lev}\t\tx1");
                    AddMustItemInfoText($"恶魔之钥+{lev}\t\tx1", MustItem.IsHaveEMoZhiYao = IsHaveItem(EMoZhiYaoId, lev: lev));
                }
                else if (CheckItems[0].ConfigId == EMoZhiYaoId || CheckItems[1].ConfigId == EMoZhiYaoId)
                {
                    AddMustItemInfoText(isHave: MustItem.IsHaveEMoZhiYao = IsHaveItem(EMoZhiYaoId, ref lev), str: $"恶魔之钥+{lev}\t\tx1");
                    AddMustItemInfoText($"恶魔之眼+{lev}\t\tx1", MustItem.IsHaveEMoZhiYan = IsHaveItem(EMoZhiYanId, lev: lev));
                }
                
            }
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(GemItemConfigId.MAYA_GEMS.ToInt64()));


            Money = GetMoney();
            AddTextTitle($"恶魔广场通行证 +{lev}");
            IsCanMerger = MustItem == (true, true, true);
            return CheckItemCount();

            int GetMoney() => lev switch
            {
                1 => 100_000,
                2 => 200_000,
                3 => 400_000,
                4 => 700_000,
                5 => 11000_000,
                6 => 1600_000,
                7 => 2000_000,
                _ => 100_000
            };
        }
    }
}