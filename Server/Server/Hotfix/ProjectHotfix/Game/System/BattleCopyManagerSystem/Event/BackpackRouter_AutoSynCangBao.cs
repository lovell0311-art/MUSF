using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [BackpackRouter(320401)]    // 藏宝图碎片
    public class BackpackRouter_AutoSynCangBao : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            //判断是否合成藏宝图
            int level = item.GetProp(EItemValue.Level);
            var items = backpack.GetLevelItemsFromConfigID(320401, level);
            if (items.Count >= (5 - 1))
            {
                item.Delete($"藏宝图合成({log})");
                for (int i = 0; i < (5 - 1); i++)
                {
                    backpack.DeleteItem(items[i], "藏宝图合成");
                }

                //转化为藏宝图
                Item targetItem = ItemFactory.Create(310073, backpack.Parent.GameAreaId);
                targetItem.SetProp(EItemValue.Level, level);
                backpack.AddItem(targetItem, "藏宝图合成");
            }
        }
    }
    [BackpackRouter(320373)]    // 武魂精粹碎片
    public class BackpackRouter_TheSoulShard : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            //判断是否合成武魂精粹
            int level = item.GetProp(EItemValue.Level);
            var items = backpack.GetLevelItemsFromConfigID(320373, level);
            if (items.Count >= (10 - 1))
            {
                item.Delete($"武魂精粹合成({log})");
                for (int i = 0; i < (10 - 1); i++)
                {
                    backpack.DeleteItem(items[i], "武魂精粹合成");
                }

                //转化为武魂精粹
                Item targetItem = ItemFactory.Create(310155, backpack.Parent.GameAreaId);
                targetItem.SetProp(EItemValue.Level, level);
                backpack.AddItem(targetItem, "武魂精粹合成");
            }
        }
    }
}