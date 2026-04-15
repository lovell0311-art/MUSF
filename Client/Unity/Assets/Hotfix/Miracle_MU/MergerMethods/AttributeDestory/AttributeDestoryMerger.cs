using ETModel;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 去除再生宝石属性
    /// </summary>
    [MergerSystem(1201)]
    public class AttributeDestoryMerger : MergerMethod
    {
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 5_000_000;
            SuccessRate = 100;
            MaxSuccessRate = 100;
            FailedDelete = true;
            mergerMethodId = 10073;
            mergerMethod = "RemoveRegeneration";
            bool IsHavePuTong;
            //标题


            AddTextTitle("去除再生宝石属性");
            ///必须材料
            AddMustItemInfoText("有再生宝石属性的装备\t\tx1", IsHavePuTong = IsHaveRegeneration());

            IsCanMerger = IsHavePuTong;

            return CheckItemCount();
            ///普通品质武器装备
            bool IsHaveRegeneration()
            {
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    if ((CheckItems[i].ItemType <= (int)E_ItemType.Boots &&(CheckItems[i].ItemType >= (int)E_ItemType.Swords) || (CheckItems[i].ItemType == (int)E_ItemType.Wing)
                        || (CheckItems[i].ItemType == (int)E_ItemType.Necklace)|| (CheckItems[i].ItemType == (int)E_ItemType.Rings)|| (CheckItems[i].ItemType == (int)E_ItemType.Guard)
                        || (CheckItems[i].ItemType == (int)E_ItemType.WristBand) || (CheckItems[i].ItemType == (int)E_ItemType.Pet) || (CheckItems[i].ItemType == (int)E_ItemType.QiZhi)))
                    {
                        if (CheckItems[i].IsHaveReginAtr)
                        {
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
