using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;


namespace ETHotfix
{
    /// <summary>
    /// 物品添加幸运属性
    /// </summary>
    [ItemCustomAttrMethod]
    public class ItemAddLucky : IItemCustomAttrMethodHandler
    {
        public void Run(Item item)
        {
            if (!item.CanHaveLuckyAttr())
            {
                throw new ItemNotSupportAttrException($"物品不支持添加幸运，item.ConfigId={item.ConfigID},ItemCustomAttrMethod='ItemAddLucky'");
            }
            item.SetProp(EItemValue.LuckyEquip, 1);
        }
    }
}
