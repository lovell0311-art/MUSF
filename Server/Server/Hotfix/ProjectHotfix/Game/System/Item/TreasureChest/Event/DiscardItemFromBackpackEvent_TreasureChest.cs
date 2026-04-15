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

    [EventMethod("DiscardItemFromBackpack")]
    public class DiscardItemFromBackpackEvent_TreasureChest : ITEventMethodOnRun<ETModel.EventType.DiscardItemFromBackpack>
    {
        public void OnRun(ETModel.EventType.DiscardItemFromBackpack args)
        {
            var treasureChest = Root.MainFactory.GetCustomComponent<TreasureChestManager>();
            var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            //需要要是的宝箱不处理
            //if (args.mapItem.ConfigId == 320407 || args.mapItem.ConfigId == 320408 || args.mapItem.ConfigId == 320409) return;

            if (!readConfig.GetJson<TreasureChest_TypeConfigJson>().JsonDic.TryGetValue(args.mapItem.ConfigId, out var dropConfig))
            {
                // 没有宝箱配置
                return;
            }
            if (!treasureChest.ItemSelector.TryGetValue(args.mapItem.ConfigId, out var selector))
            {
                Log.Error($"宝箱没有配置掉落的物品. args.mapItem.InstanceId={args.mapItem.ConfigId}");
                return;
            }
            var itemInfoJson = readConfig.GetJson<TreasureChest_ItemInfoConfigJson>().JsonDic;


            // TODO 随机宝箱能掉落的物品
            if (dropConfig.NotRepeat == 1)
            {
                selector = new RandomSelector<int>(selector);
            }
            //var rand = new Random();
            //int count = rand.Next(dropConfig.MinCount, dropConfig.MaxCount);
            int count = dropConfig.GetCountDrop();
            using ListComponent<Item> itemList = ListComponent<Item>.Create();
            if (dropConfig.MustFall.Count >= 1)
            {
                foreach (var Info in dropConfig.MustFall)
                {
                    itemInfoJson.TryGetValue(Info.Key, out var itemInfoConfig);
                    for (int j = 0; j < Info.Value; j++)
                    {
                        var item = ItemFactory.TryCreate(itemInfoConfig.ItemId, args.player.GameAreaId, itemInfoConfig.ToItemCreateAttr());
                        if (item == null)
                        {
                            Log.Error($"配置的物品id不存在{args.mapItem.ConfigId},itemInfoConfig.ItemId={itemInfoConfig.ItemId}");
                            continue;
                        }
                        itemList.Add(item);
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                int itemInfoId = 0;
                if (dropConfig.NotRepeat == 1)
                {
                    if (!selector.TryGetValueAndRemove(out itemInfoId))
                    {
                        Log.Error($"宝箱配置的物品数量不够. args.mapItem.InstanceId={args.mapItem.ConfigId}");
                        return;
                    }
                }
                else
                {
                    if (!selector.TryGetValue(out itemInfoId))
                    {
                        Log.Error($"宝箱配置的物品数量不够. args.mapItem.InstanceId={args.mapItem.ConfigId}");
                        return;
                    }
                }

                itemInfoJson.TryGetValue(itemInfoId, out var itemInfoConfig);
                try
                {
                    var item = ItemFactory.Create(itemInfoConfig.ItemId, args.player.GameAreaId, itemInfoConfig.ToItemCreateAttr());
                    itemList.Add(item);
                }
                catch (ItemNotSupportAttrException e)
                {
                    Log.Warning($"宝箱配置的物品无法添加指定属性. args.mapItem.InstanceId={args.mapItem.ConfigId},itemInfoConfig.ItemId={itemInfoConfig.ItemId}({e.ToString()})");
                }
                catch (ItemConfigNotExistException e)
                {
                    Log.Error($"宝箱配置的物品id不存在. args.mapItem.InstanceId={args.mapItem.ConfigId},itemInfoConfig.ItemId={itemInfoConfig.ItemId}({e.ToString()})");
                    return;
                }
            }

            // TODO 随机掉落的位置
            if (itemList.Count == 0)
            {
                Log.Error($"宝箱没有配置可以掉落的物品. args.mapItem.InstanceId={args.mapItem.ConfigId}");
                return;
            }
            using ListComponent<C_FindTheWay2D> allPos = ListComponent<C_FindTheWay2D>.Create();
            List<C_FindTheWay2D> allPosList = allPos;

            if (!GetRandomDropPos(args.map, args.mapItem.Position.x, args.mapItem.Position.y, itemList.Count, ref allPosList))
            {
                Log.Error($"丢弃的位置空间太小，无法展开宝箱. args.mapItem.InstanceId={args.mapItem.ConfigId}");
                return;
            }

            // TODO 展开宝箱
            Log.PLog("Item", $"a:{args.player.UserId} r:{args.player.GameUserId} 开启宝箱 {args.mapItem.Item.ToLogString()}");
            for (int i = 0; i < itemList.Count; ++i)
            {
                var item = itemList[i];
                var pos = allPos[i];


                Log.PLog("Item", $"a:{args.player.UserId} r:{args.player.GameUserId} 宝箱爆出的物品 {args.mapItem.Item.ToLogString()} => {item.ToLogString()}");

                MapItem mDropItem = MapItemFactory.Create(item,EMapItemCreateType.TChestDrop);
                // 添加拾取保护
                mDropItem.KillerId.Add(args.player.GameUserId);
                // 设置保护时间 1小时。在物品被清除前，禁止其他玩家拾取
                mDropItem.ProtectTick = Help_TimeHelper.GetNowSecond(10000000 * 60 * 60L);
                args.map.MapEntityEnter(mDropItem, pos.X, pos.Y);
            }

            // TODO 清除宝箱的拥有者，防止在一帧时间内，被自己拾取
            args.mapItem.KillerId.Clear();

            var map = args.map;
            var mapItem = args.mapItem;
            var player = args.player;

            async Task DeleteItemAsync()
            {
                await ETModel.ET.TimerComponent.Instance.WaitFrameAsync();
                if (map.IsDisposeable) return;
                if (mapItem.IsDisposeable) return;
                // TODO 删除宝箱
                mapItem.Dispose();
            }

            DeleteItemAsync().Coroutine();
        }

        /// <summary>
        /// 获取随机掉落点
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="x">坐标X</param>
        /// <param name="y">坐标Y</param>
        /// <param name="count">取出多少点</param>
        /// <param name="allPos">取出的FindTheWay2D列表</param>
        /// <returns>true.取出成功 false.取出失败</returns>
        public bool GetRandomDropPos(MapComponent map, int x, int y, int count, ref List<C_FindTheWay2D> allPos)
        {
            if (count <= 0) return false;
            if(count == 1)
            {
                // 返回丢弃的位置
                C_FindTheWay2D pos = map.GetFindTheWay2D(x, y);
                if (pos == null)
                {
                    return false;
                }
                allPos.Add(pos);
                return true;
            }

            int width = 3;
            int minPosX = x - width;
            int minPosY = y - width;
            int maxPosX = x + width;
            int maxPosY = y + width;


            using ListComponent<C_FindTheWay2D> tempPosList = ListComponent<C_FindTheWay2D>.Create();

            for (int posX = minPosX; posX < maxPosX; ++posX)
            {
                if (map.TryGetPosX(posX))
                {
                    for (int posY = minPosY; posY < maxPosY; ++posY)
                    {
                        var pos = map.GetFindTheWay2D(posX, posY);
                        if (pos != null)
                        {
                            if (pos.IsObstacle == false)
                            {
                                tempPosList.Add(pos);
                            }
                        }
                    }
                }
            }
            if (tempPosList.Count == 0) return false;
            var rand = new Random();

            // 不能出现重复的位置
            bool notRepeat = true;
            if (tempPosList.Count < count)
            {
                // 可以出现重复的位置
                notRepeat = false;
            }

            for (int i = 0; i < count; ++i)
            {
                int randId = rand.Next(0, tempPosList.Count);
                allPos.Add(tempPosList[randId]);
                if (notRepeat)
                {
                    tempPosList.RemoveAt(randId);
                }
            }
            return true;
        }

    }
}
