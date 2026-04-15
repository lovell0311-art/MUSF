using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static class MapItemFactory
    {
        public static MapItem Create(int configId,EMapItemCreateType createType,int count = 1)
        {
            MapItem mapItem = Root.CreateBuilder.GetInstance<MapItem>();
            mapItem.ConfigId = configId;
            mapItem.Count = count;
            mapItem.ProtectTick = Help_TimeHelper.GetNowSecond(10000000 * 15);
            mapItem.ItemUid = 0;
            mapItem.CreateType = createType;

            mapItem.AddCustomComponent<TimingDisposeComponent>();   // 定时销毁
            return mapItem;
        }

        public static MapItem Create(Item item, EMapItemCreateType createType)
        {
            MapItem mapItem = Root.CreateBuilder.GetInstance<MapItem>();
            mapItem.ConfigId = item.ConfigID;
            mapItem.Quality = item.GetQuality();
            mapItem.Count = item.GetProp(EItemValue.Quantity);
            mapItem.SetItem(item);
            mapItem.ProtectTick = Help_TimeHelper.GetNowSecond(10000000 * 15);
            mapItem.ItemUid = item.ItemUID;
            mapItem.CreateType = createType;

            mapItem.AddCustomComponent<TimingDisposeComponent>();   // 定时销毁
            return mapItem;
        }
    }
}
