using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    public static class BackpackComponentHelper
    {
        /// <summary>
        /// 将背包中的物品丢弃到地面
        /// </summary>
        /// <param name="self"></param>
        /// <param name="itemUid"></param>
        /// <param name="map"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool DiscardItemToGround(this BackpackComponent self,long itemUid,MapComponent map,int posX,int posY,EMapItemCreateType createType,string log)
        {
            Item item = self.GetItemByUID(itemUid);
            if (item == null) return false;
            self.RemoveItem(item, log);
            // 设置物品在地面
            item.data.InComponent = EItemInComponent.Map;
            item.data.posX = posX;
            item.data.posY = posY;
            // 物品已经移出player.DataCacheManageComponent，需要将物品状态立即更新到数据库。
            // 防止重新上线，存档错误
            item.SaveDBNow().Coroutine();
            // 这个物品已经不属于这个玩家，将物品数据从缓存中删除
            self.mPlayer.GetCustomComponent<DataCacheManageComponent>().Get<DBItemData>().DataRemove(item.data.Id);

            MapItem mDropItem = MapItemFactory.Create(item, createType);
            // 添加拾取保护
            mDropItem.KillerId.Add(self.mPlayer.GameUserId);
            mDropItem.OwnerUserId = self.mPlayer.UserId;
            mDropItem.OwnerGameUserId = self.mPlayer.GameUserId;

            map.MapEntityEnter(mDropItem, posX, posY);

            // 发布 DiscardItemByBackpack 事件
            ETModel.EventType.DiscardItemFromBackpack.Instance.player = self.mPlayer;
            ETModel.EventType.DiscardItemFromBackpack.Instance.mapItem = mDropItem;
            ETModel.EventType.DiscardItemFromBackpack.Instance.map = map;
            Root.EventSystem.OnRun("DiscardItemFromBackpack", ETModel.EventType.DiscardItemFromBackpack.Instance);
            return true;
        }
    }
}
