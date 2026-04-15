using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;


namespace ETHotfix
{
    /// <summary>
    /// 随机添加卓越属性 用宠物的方式 最低 3条
    /// </summary>
    [ItemCustomAttrMethod]
    public class ItemRandAddExcAttrWithPet_3 : IItemCustomAttrMethodHandler
    {
        public void Run(Item item)
        {
            if (!item.CanHaveExcellentOption())
            {
                throw new ItemNotSupportAttrException($"物品不支持添加卓越属性，item.ConfigId={item.ConfigID},ItemCustomAttrMethod='ItemRandAddExcAttrWithPet_3'");
            }
            var excAttrEntryManager = Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
            if (excAttrEntryManager.TryGetPetsAttrEntry(out var selector))
            {
                if (excAttrEntryManager.PetsExcAttrEntryCount.TryGetValue(out int count))
                {
                    if(count < 3) count = 3;
                    var newSelector = new RandomSelector<int>(selector);
                    while (count > 0)
                    {
                        if (newSelector.TryGetValueAndRemove(out var entryId))
                        {
                            if (item.data.ExcellentEntry.Add(entryId))
                            {
                                --count;
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
                        RandomHelper.RandomNumber(0, 100) < ConstItem.ExcAddLuckyPct)
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
}
