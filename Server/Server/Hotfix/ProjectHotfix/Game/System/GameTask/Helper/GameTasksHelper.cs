using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    public static class GameTasksHelper
    {
        /// <summary>
        /// 掉落任务物品
        /// </summary>
        /// <param name="deadEnamy"></param>
        /// <param name="attacker"></param>
        /// <param name="b_FindTheWay"></param>
        /// <returns>掉落的物品</returns>
        public static MapItem DropItem(Enemy deadEnamy, GamePlayer attacker, C_FindTheWay2D b_FindTheWay)
        {
            var taskConfigManager = Root.MainFactory.GetCustomComponent<GameTaskConfigManager>();
            if (!taskConfigManager.DropItemConfigDict.TryGetValue(deadEnamy.Config.Id, out var tasksList))
            {
                return null;
            }
            var gameTasksComponent = attacker.Player.GetCustomComponent<GameTasksComponent>();
            using (ListComponent<GameTask> tasks = ListComponent<GameTask>.Create())
            {
                List<GameTask> list = tasks;
                gameTasksComponent.GetTasksWith(ref list, p =>
                {
                    if (p.TaskState != EGameTaskState.Doing) return false;
                    if (tasksList.ContainsKey(p.ConfigId) != true) return false;
                    return true;
                });
                if (list.Count == 0) return null;

                int randNum = Help_RandomHelper.Range(0, 10000);
                foreach (var task in tasks)
                {
                    foreach (var dropItem in tasksList[task.ConfigId])
                    {
                        // 每个任务，有多个任务目标
                        // TODO 检查对应的目标有没有完成
                        if (dropItem.TaskTargetId >= task.TaskProgress.Count)
                        {
                            Log.Warning($"GameTask_DropItemConfig.TaskTargetId 字段错误!dropItem.Id={dropItem.Id},dropItem.TaskTargetId={dropItem.TaskTargetId}");
                            continue;
                        }

                        if (task.TaskProgress[dropItem.TaskTargetId] >= task.Config.TaskTargetCount[dropItem.TaskTargetId])
                        {
                            // 任务完成
                            continue;
                        }

                        if (randNum >= dropItem.DropRate) continue;

                        var item = ItemFactory.TryCreate(dropItem.ItemId, attacker.Player.GameAreaId, dropItem.ToItemCreateAttr());
                        if (item == null)
                        {
                            Log.Warning($"无法创建物品.dropItem.ItemId={dropItem.ItemId}");
                            continue;
                        }

                        Log.Info($"掉落任务物品 dropItem.Id={dropItem.Id},dropItem.ItemId={dropItem.ItemId}");
                        MapItem mDropItem = MapItemFactory.Create(item,EMapItemCreateType.MonsterDrop);
                        mDropItem.MonsterConfigId = deadEnamy.Config.Id;
                        mDropItem.ProtectTick = Help_TimeHelper.GetNowSecond(10000 * 1000 * 60 * 60L);  // 任务物品，其他人无法拾取
                        return mDropItem;
                    }

                }
            }

            return null;
        }

        /// <summary>
        /// 将当前协议广播给周围的玩家和自己 穿戴和卸下装备时使用
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_ActorMessage"></param>
        public static void NotifyAroundPlayer(Player player, IActorMessage b_ActorMessage)
        {
            //获得角色当前所在地图和坐标
            var mGamePlayer = player.GetCustomComponent<GamePlayer>();
            if (mGamePlayer == null) return;

            int mapIndex = mGamePlayer.UnitData.Index;
            int x = mGamePlayer.UnitData.X;
            int y = mGamePlayer.UnitData.Y;

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(player.SourceGameAreaId);
            if (mServerArea == null)
            {
                Log.Error($"找不到区服");
                return;
            }
            //获得当前地图组件
            if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mapIndex, out var mapComponent))
            {
                C_FindTheWay2D b_Source = mapComponent.GetFindTheWay2D(x, y);
                mapComponent.SendNotice(b_Source, b_ActorMessage);
            }
        }


        /// <summary>
        /// 可以拾取任务物品
        /// </summary>
        /// <param name="self"></param>
        /// <param name="mapItem"></param>
        /// <returns></returns>
        public static bool CanPickUpTaskItem(Player player, Item taskItem)
        {
            GameTasksComponent gameTasksCmpt = player.GetCustomComponent<GameTasksComponent>();
            using ListComponent<GameTask> taskList = ListComponent<GameTask>.Create();
            List<GameTask> list = taskList;
            gameTasksCmpt.GetTasksInProgressByActionType(ref list, EGameTaskActionType.CollectAndSubmitItem);

            if (list.Count == 0) return false;

            int needItemCount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                GameTask task = list[i];
                for (int targetId = 0; targetId < task.Config.TaskTargetId.Count; targetId++)
                {
                    if (task.Config.TaskTargetId[targetId] == taskItem.ConfigID)
                    {
                        needItemCount += task.Config.TaskTargetCount[targetId];
                    }
                }
            }

            if (needItemCount == 0) return true;

            int itemCount = 0;

            BackpackComponent backpack = player.GetCustomComponent<BackpackComponent>();
            var allItem = backpack.GetAllItemByConfigID(taskItem.ConfigID);
            if (allItem == null || allItem.Count == 0) return true;

            foreach (Item item in allItem.Values)
            {
                itemCount += item.GetProp(EItemValue.Quantity);
                if (itemCount >= needItemCount) return false;
            }
            return true;
        }
    }
}
