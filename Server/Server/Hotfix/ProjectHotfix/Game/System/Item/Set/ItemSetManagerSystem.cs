using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod(typeof(ItemSetManager), EventSystemType.INIT)]
    public class ItemSetManagerInitSystem : ITEventMethodOnInit<ItemSetManager>
    {
        public void OnInit(ItemSetManager self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(ItemSetManager), EventSystemType.LOAD)]
    public class ItemSetManagerLoadSystem : ITEventMethodOnLoad<ItemSetManager>
    {
        public override void OnLoad(ItemSetManager self)
        {
            self.OnInit();
        }
    }

    public static class ItemSetManagerSystem
    {
        public static void OnInit(this ItemSetManager self)
        {
            self.ItemSetSelector.Clear();

            var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDict = readConfig.GetJson<SetItem_TypeConfigJson>().JsonDic;
            foreach(var config in jsonDict.Values)
            {
                foreach (var itemId in config.ItemsId)
                {
                    RandomSelector<int> selector;
                    if (!self.ItemSetSelector.TryGetValue(itemId, out selector))
                    {
                        selector = new RandomSelector<int>();
                        self.ItemSetSelector.Add(itemId, selector);
                    }
                    selector.Add(config.Id, config.Rate);
                }
            }


            self.ExtraEntryLevelSelector.Clear();
            var extraConfigJsonDict = readConfig.GetJson<ItemAttrEntry_ExtraConfigJson>().JsonDic;
            foreach (var config in extraConfigJsonDict.Values)
            {
                RandomSelector<int> selector;
                if (!self.ExtraEntryLevelSelector.TryGetValue(config.Id, out selector))
                {
                    selector = new RandomSelector<int>();
                    self.ExtraEntryLevelSelector.Add(config.Id, selector);
                }
                selector.Add(0, config.Rate0);
                selector.Add(1, config.Rate1);
                selector.Add(2, config.Rate2);
            }
        }

        public static bool TryGetSetId(this ItemSetManager self,int itemConfigId,out int value)
        {
            if(self.ItemSetSelector.TryGetValue(itemConfigId,out var selector))
            {
                if(selector.TryGetValue(out value))
                {
                    return true;
                }
            }
            value = 0;
            return false;
        }
    }
}
