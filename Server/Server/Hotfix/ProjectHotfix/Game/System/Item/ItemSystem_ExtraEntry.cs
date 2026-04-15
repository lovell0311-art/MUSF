using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    /// <summary>
    /// 额外属性，用于套装
    /// </summary>
    public static partial class ItemSystem
    {
        public static void AddExtraEntry(this Item self)
        {
            ItemSetManager itemSetManager = Root.MainFactory.GetCustomComponent<ItemSetManager>();

            foreach(int extraId in self.ConfigData.ExtraAttrId)
            {
                if(itemSetManager.ExtraEntryLevelSelector.TryGetValue(extraId,out var selector))
                {
                    if(selector.TryGetValue(out int level))
                    {
                        self.data.ExtraEntry[extraId] = level;
                    }
                }
                else
                {
                    Log.Error($"没有找到 ExtraEntry.Id={extraId}");
                }
            }
        }

    }
}
