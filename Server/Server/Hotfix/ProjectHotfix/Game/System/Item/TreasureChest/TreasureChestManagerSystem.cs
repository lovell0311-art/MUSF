using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod(typeof(TreasureChestManager), EventSystemType.INIT)]
    public class TreasureChestManagerInitSystem : ITEventMethodOnInit<TreasureChestManager>
    {
        public void OnInit(TreasureChestManager self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(TreasureChestManager), EventSystemType.LOAD)]
    public class TreasureChestManagerLoadSystem : ITEventMethodOnLoad<TreasureChestManager>
    {
        public override void OnLoad(TreasureChestManager self)
        {
            self.OnInit();
        }
    }



    public static class TreasureChestManagerSystem
    {
        public static void OnInit(this TreasureChestManager self)
        {
            var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();

            {//宝箱
                self.ItemSelector.Clear();

                var allConfig = readConfig.GetJson<TreasureChest_ItemInfoConfigJson>().JsonDic;
                foreach (var config in allConfig.Values)
                {
                    if (!self.ItemSelector.TryGetValue(config.TreasureChestId, out var selector))
                    {
                        selector = new RandomSelector<int>();
                        self.ItemSelector.Add(config.TreasureChestId, selector);
                    }
                    selector.Add(config.Id, config.DropRate);
                }
            }
            
            {//特殊掉落
                self.SpecialDrop.Clear();

                var allConfig = readConfig.GetJson<DropItem_SpecialConfigJson>().JsonDic;
                foreach (var config in allConfig.Values)
                {
                    if (!self.SpecialDrop.TryGetValue(config.TreasureChestId, out var selector))
                    {
                        selector = new RandomSelector<int>();
                        self.SpecialDrop.Add(config.TreasureChestId, selector);
                    }
                    selector.Add(config.Id, config.DropRate);
                }
            }
        }


    }
}
