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

    [BackpackRouter(320107)]
    public class BackpackRouter_AutoSynShiLuo : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            //判断是否合成失落的地图
            int level = item.GetProp(EItemValue.Level);
            var items = backpack.GetLevelItemsFromConfigID(320107, level);
            if (items.Count >= (5 - 1))
            {
                item.Delete($"失落的地图合成({log})");
                for (int i = 0; i < (5 - 1); i++)
                {
                    backpack.DeleteItem(items[i], "失落的地图合成");
                }

                //转化为失落的地图
                Item targetItem = ItemFactory.Create(320106, backpack.Parent.GameAreaId);
                targetItem.SetProp(EItemValue.Level, level);
                backpack.AddItem(targetItem, "失落的地图合成");
            }
        }
    }
}