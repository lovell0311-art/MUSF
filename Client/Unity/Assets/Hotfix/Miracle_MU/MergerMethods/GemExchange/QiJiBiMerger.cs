using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 卓越兑换奇迹币
    /// </summary>
    [MergerSystem(801)]
    public class QiJiBiMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 500_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10079;
            mergerMethod = "ExcellentExchangeCoin";
            bool IsHave = false;
            //标题
            AddTextTitle("卓越兑换U币");
            ///必须材料
            AddMustItemInfoText("卓越品质装备/白板套装属性装备/玛雅/创造/祝福/灵魂/再生/生命宝石\t\tx1", IsHave = IsHaveFangZhuZhuangBei());

            IsCanMerger = IsHave;


            ///普通品质武器装备
            bool IsHaveFangZhuZhuangBei()
            {
                Log.DebugBrown("来了1");
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    Log.DebugBrown("来了");
                    if (CheckItems[i].ExecllentEntryDic.Count >= 1)
                    {
                        if (CheckItems[i].ConfigId == 340001)//开服 赠送的勇者旗帜 无法用于奇迹币兑换
                        {
                           
                            continue;
                        }
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                    if (CheckItems[i].ConfigId == 280001 || CheckItems[i].ConfigId == 280006 || CheckItems[i].ConfigId == 280003 ||
                        CheckItems[i].ConfigId == 280004 || CheckItems[i].ConfigId == 280005 /*|| CheckItems[i].ConfigId == 280011*/)//宝石
                    {
                        CheckItems.RemoveAt(i);
                        return true;
                    }
                    if (CheckItems[i].GetProperValue(E_ItemValue.SetId) != 0)//套装
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
