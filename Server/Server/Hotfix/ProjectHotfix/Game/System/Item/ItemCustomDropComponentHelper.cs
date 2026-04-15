using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    public static class ItemCustomDropComponentHelper
    {
        public static MapItem DropItem(Enemy deadEnemy, GamePlayer attacker, C_FindTheWay2D b_FindTheWay)
        {
            var customItemDrop = Root.MainFactory.GetCustomComponent<ItemCustomDropComponent>();
            if (!customItemDrop.ItemDict.TryGetValue(deadEnemy.Config.Id, out var itemDict)) return null;
            if (!customItemDrop.DropTypeSelector.TryGetValue(out var dropType)) return null;
            if (!itemDict.TryGetValue(dropType, out var selector)) return null;
            if (!selector.TryGetValue(out var itemDropId)) return null;

            var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if(!readConfig.GetJson<ItemCustomDrop_InfoConfigJson>().JsonDic.TryGetValue(itemDropId,out var dropItem))
            {
                Log.Warning($"击杀怪物后，没有找到掉落的物品配置. deadEnemy.ConfigId={deadEnemy.Config.Id},itemDropId={itemDropId}");
                return null;
            }
            var item = ItemFactory.TryCreate(dropItem.ItemId, attacker.Player.GameAreaId, dropItem.ToItemCreateAttr());
            if (item == null)
            {
                Log.Warning($"无法创建物品.dropItem.ItemId={dropItem.ItemId}");
                return null;
            }

            Log.Info($"自定义物品掉落 dropItem.Id={dropItem.Id},dropItem.ItemId={dropItem.ItemId}");
            MapItem mDropItem = MapItemFactory.Create(item,EMapItemCreateType.MonsterDrop);
            mDropItem.MonsterConfigId = deadEnemy.Config.Id;
            return mDropItem;
        }
    }
}
