using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;


namespace ETHotfix
{
    /// <summary>
    /// 物品随机套装Id
    /// </summary>
    [ItemCustomAttrMethod]
    public class ItemTryRandSetId : IItemCustomAttrMethodHandler
    {
        public void Run(Item item)
        {
            if (!item.CanHaveSetOption())
            {
                return;
            }
            var itemSetManager = Root.MainFactory.GetCustomComponent<ItemSetManager>();
            if(!itemSetManager.ItemSetSelector.TryGetValue(item.ConfigID,out var selector))
            {
                return;
            }
            if(selector.TryGetValue(out int setId))
            {
                item.SetProp(EItemValue.SetId, setId);
                item.AddExtraEntry();   // 套装必出属性

                // 自动添加幸运，技能
                if (item.CanHaveLuckyAttr() &&
                    RandomHelper.RandomNumber(0, 100) < ConstItem.SetAddLuckyPct)
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
