using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;


namespace ETHotfix
{
    /// <summary>
    /// 添加镶嵌孔洞 用 ItemSocket_DropCount 配置
    /// </summary>
    [ItemCustomAttrMethod]
    public class ItemTryAddSocketHoleCount : IItemCustomAttrMethodHandler
    {
        public void Run(Item item)
        {
            if(!item.CanHaveEnableSocket())
            {
                return;
            }
            ItemSocketManager itemSocketManager = Root.MainFactory.GetCustomComponent<ItemSocketManager>();
            if (itemSocketManager.DropHoleCountSelector.TryGetValue(out int count))
            {
                item.SetProp(EItemValue.FluoreSlotCount, count);
                item.SetProp(EItemValue.FluoreSlot1, 0);
                item.SetProp(EItemValue.FluoreSlot2, 0);
                item.SetProp(EItemValue.FluoreSlot3, 0);
                item.SetProp(EItemValue.FluoreSlot4, 0);
                item.SetProp(EItemValue.FluoreSlot5, 0);

                // 自动添加幸运，技能
                if (item.CanHaveLuckyAttr() &&
                    RandomHelper.RandomNumber(0, 100) < ConstItem.SocketAddLuckyPct)
                {
                    item.SetProp(EItemValue.LuckyEquip, 1);
                }
                if (item.CanHaveSkill())
                {
                    item.SetProp(EItemValue.SkillId, item.ConfigData.Skill);
                }
            }
        }
    }
}
