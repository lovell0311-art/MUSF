using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;


namespace ETHotfix
{
    /// <summary>
    /// 物品尝试添加幸运属性
    /// </summary>
    [ItemCustomAttrMethod]
    public class ItemTryAddLucky : IItemCustomAttrMethodHandler
    {
        public void Run(Item item)
        {
            if (item.CanHaveLuckyAttr())
            {
                item.SetProp(EItemValue.LuckyEquip, 1);
            }
        }
    }
}
