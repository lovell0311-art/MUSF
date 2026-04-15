using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod("NSMapItem.Destory")]
    public class NSMapItemDestory_DisposeDBItem : ITEventMethodOnRun<ETModel.EventType.NSMapItem.Destory>
    {
        public void OnRun(ETModel.EventType.NSMapItem.Destory args)
        {
            // MapItem 销毁时，需要将存着的物品，从数据库中销毁
            if (args.self.Item == null) return;
            args.self.Item.Delete("Map 自动清理");
            args.self.SetItem(null);
        }
    }

    [EventMethod("NSMapEntity.EnterMap.MapItem")]
    public class MapItemEnterMap : ITEventMethodOnRun<ETModel.EventType.NSMapEntity.EnterMap>
    {
        public void OnRun(ETModel.EventType.NSMapEntity.EnterMap args)
        {
            MapItem mapItem = (MapItem)args.self;
            // 通知附加玩家，MapItem 进入地图
            MapItem.g2C_ItemDrop_notice.Info.Clear();
            MapItem.g2C_ItemDrop_notice.Info.Add(mapItem.ToMessage());
            C_FindTheWay2D mFindTheWay = args.map.GetFindTheWay2D(mapItem.Position.x, mapItem.Position.y);
            args.map.SendNotice(mFindTheWay, MapItem.g2C_ItemDrop_notice);

            // 将DB数据，移到Map中
            if (mapItem.Item == null) return;
            if (mapItem.Item.data.GameUserId == -1) return; // 新物品，没被任何人拾取过
            DataCacheManageComponent dataCacheManage = args.self.Map.GetCustomComponent<DataCacheManageComponent>();
            if (dataCacheManage == null)
            {
                dataCacheManage = args.self.Map.AddCustomComponent<DataCacheManageComponent>();
            }
            dataCacheManage.GetOrCreate<DBItemData>().DataAdd(mapItem.Item.data);
        }
    }

    [EventMethod("NSMapEntity.LeaveMap.MapItem")]
    public class MapItemLeaveMap : ITEventMethodOnRun<ETModel.EventType.NSMapEntity.LeaveMap>
    {
        public void OnRun(ETModel.EventType.NSMapEntity.LeaveMap args)
        {
            MapItem mapItem = (MapItem)args.self;
            // 通知附加玩家，MapItem 离开地图
            MapItem.g2C_BattlePickUpDropItem_notice.InstanceId.Clear();
            MapItem.g2C_BattlePickUpDropItem_notice.InstanceId.Add(mapItem.InstanceId);
            C_FindTheWay2D mFindTheWay = args.map.GetFindTheWay2D(mapItem.Position.x, mapItem.Position.y);
            args.map.SendNotice(mFindTheWay, MapItem.g2C_BattlePickUpDropItem_notice);

            // 将DB数据，移出Map
            if (mapItem.ItemUid == 0) return;
            DataCacheManageComponent dataCacheManage = args.self.Map.GetCustomComponent<DataCacheManageComponent>();
            if (dataCacheManage == null) return;
            C_DataCache<DBItemData> dataCache = dataCacheManage.Get<DBItemData>();
            if (dataCache == null) return;
            if (dataCache.DataQueryById(mapItem.ItemUid) == null)
            {
                // 这个Item没有存数据库
                return;
            }
            dataCache.DataRemove(mapItem.ItemUid);
        }
    }


    public static class MapItemSystem
    {
    }
}
