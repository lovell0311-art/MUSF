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

    [BackpackRouter(320404)]
    public class BackpackRouter_TreasureChest1 : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            var items = backpack.GetAllItemByConfigID(320404);
            if (items != null && items.Count >= (5 - 1))
            {
                item.Delete($"合成白银宝箱({log})");
                foreach (var Itme in items)
                {
                    backpack.DeleteItem(Itme.Value, "白银宝箱合成");
                }

                //转化为白银宝箱
                Item targetItem = ItemFactory.Create(320407, backpack.Parent.GameAreaId);
                backpack.AddItem(targetItem, "白银宝箱合成");
            }
        }
    }
}