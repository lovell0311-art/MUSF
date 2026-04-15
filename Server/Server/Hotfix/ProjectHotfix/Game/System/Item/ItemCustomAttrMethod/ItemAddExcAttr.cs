using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;


namespace ETHotfix
{
    /// <summary>
    /// 添加卓越属性 用 ItemAttrEntryRate_ExcConfig
    /// </summary>
    [ItemCustomAttrMethod]
    public class ItemAddExcAttr : IItemCustomAttrMethodHandler
    {
        public void Run(Item item)
        {
            if (!item.CanHaveExcellentOption())
            {
                throw new ItemNotSupportAttrException($"物品不支持添加卓越属性，item.ConfigId={item.ConfigID},ItemCustomAttrMethod='ItemAddExcAttr'");
            }
            var excAttrEntryManager = Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
            if (excAttrEntryManager.ExcAttrEntryCount.TryGetValue(out int count))
            {
                if (excAttrEntryManager.TryGetSelectorByItem(item, out var selector))
                {
                    var newSelector = new RandomSelector<int>(selector);
                    int addCount = 0;
                    while (addCount < count)
                    {
                        if (newSelector.TryGetValueAndRemove(out var entryId))
                        {
                            if (item.data.ExcellentEntry.Add(entryId))
                            {
                                ++addCount;
                            }
                        }
                        else
                        {
                            // 词条取空了
                            break;
                        }
                    }
                    // 自动添加幸运，技能
                    if (item.CanHaveLuckyAttr() &&
                        RandomHelper.RandomNumber(0,100) < ConstItem.ExcAddLuckyPct)
                    {
                        item.SetProp(EItemValue.LuckyEquip, 1);
                    }
                    if(item.CanHaveSkill())
                    {
                        item.SetProp(EItemValue.SkillId, item.ConfigData.Skill);
                    }
                }
                else
                {
                    throw new ItemNotSupportAttrException($"物品不支持添加卓越属性，item.ConfigId={item.ConfigID},ItemCustomAttrMethod='ItemAddExcAttr'");
                }
            }
        }
    }
}
