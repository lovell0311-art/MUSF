using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod(typeof(ItemAttrEntryManager), EventSystemType.INIT)]
    public class ItemAttrEntryManagerStartSystem : ITEventMethodOnInit<ItemAttrEntryManager>
    {
        public void OnInit(ItemAttrEntryManager self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(ItemAttrEntryManager), EventSystemType.LOAD)]
    public class ItemAttrEntryManagerLoadSystem : ITEventMethodOnLoad<ItemAttrEntryManager>
    {
        public override void OnLoad(ItemAttrEntryManager self)
        {
            self.OnInit();
        }
    }

    public static class ItemAttrEntryManagerSystem
    {
        public static void OnInit(this ItemAttrEntryManager self)
        {
            self.AllEntry.Clear();
        }

        public static ItemAttrEntry GetOrCreate(this ItemAttrEntryManager self,int configId,int level)
        {
            if(self.AllEntry.TryGetValue((configId,level),out var value))
            {
                return value;
            }
            ItemAttrEntry entry = ItemAttrEntryFactory.Create(configId,level);
            if(entry != null)
            {
                self.AllEntry.Add((configId, level), entry);
            }
            return entry;
        }

    }
}
