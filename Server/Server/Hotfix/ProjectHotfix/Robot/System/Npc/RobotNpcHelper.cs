using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using CustomFrameWork;
using UnityEngine;


namespace ETHotfix.Robot
{
    public static class RobotNpcHelper
    {
        /// <summary>
        /// 前往npc
        /// </summary>
        /// <param name="localUnit"></param>
        /// <param name="npcIds"></param>
        /// <returns></returns>
        public static async Task<Unit> GoToNpc(Unit localUnit,List<int> npcIds, ETCancellationToken cancellationToken)
        {
            Scene clientScene = localUnit.ClientScene();
            RobotMapComponent robotMap = clientScene.GetComponent<RobotMapComponent>();
            MapNpcInfoComponent mapNpcInfo = robotMap.CurrentMap.GetComponent<MapNpcInfoComponent>();
            bool ret;
            List<NpcInfo> npcInfos = new List<NpcInfo>();
            if (mapNpcInfo.AllNpcSpawnPoint.Count != 0)
            {
                npcInfos = mapNpcInfo.AllNpcSpawnPoint.Where(p => npcIds.IndexOf(p.Index) >= 0).ToList();
            }

            if (npcInfos.Count == 0)
            {
                // 没有找到需要的npc
                RobotRoleComponent role = localUnit.GetComponent<RobotRoleComponent>();
                ret = await MapHelper.TransferToMap(localUnit, role.Config.InitMap, cancellationToken);
                if (cancellationToken.IsCancel()) return null;
                if (ret == false) return null;
                // TODO 在新地图中重新查找npc
                mapNpcInfo = robotMap.CurrentMap.GetComponent<MapNpcInfoComponent>();
                npcInfos = mapNpcInfo.AllNpcSpawnPoint.Where(p => npcIds.IndexOf(p.Index) >= 0).ToList();
            }

            if (npcInfos.Count == 0) return null;


            // 找当前地图离自己最近的npc
            NpcInfo targetNpcInfo = npcInfos.OrderBy(info => (localUnit.Position - new Vector2Int(info.PositionX, info.PositionY)).sqrMagnitude).First();
            Vector2Int targetNpcPos = new Vector2Int(targetNpcInfo.PositionX, targetNpcInfo.PositionY);

            // 进入npc操作距离
            bool EnterOperationDistance()
            {
                if ((targetNpcPos - localUnit.Position).sqrMagnitude <= 5 * 5) return true;
                return false;
            }
            ret = await localUnit.MoveToPosAsync(targetNpcPos, EnterOperationDistance, cancellationToken);
            if (cancellationToken.IsCancel()) return null;
            if (ret == false) return null;

            // 查找离玩家最近的npc
            Unit targetNpc = null;
            int sqrMagnitude = int.MaxValue;
            foreach (Unit npcUnit in localUnit.CurrentMap.NpcDict.Values)
            {
                RobotNpcComponent npc = npcUnit.GetComponent<RobotNpcComponent>();
                if (npcIds.IndexOf(npc.ConfigId) >= 0)
                {
                    int sqrMagnitude2 = (npcUnit.Position - localUnit.Position).sqrMagnitude;
                    if(sqrMagnitude > sqrMagnitude2)
                    {
                        sqrMagnitude = sqrMagnitude2;
                        targetNpc = npcUnit;
                    }
                }
            }
            if (targetNpc == null) return null;

            bool EnterNpcRange()
            {
                if ((targetNpc.Position - localUnit.Position).sqrMagnitude <= 5 * 5) return true;
                return false;
            }
            if(EnterNpcRange())
            {
                return targetNpc;
            }

            ret = await localUnit.MoveToUnitAsync(targetNpc, EnterNpcRange, cancellationToken);
            if (cancellationToken.IsCancel()) return null;
            if (ret == false) return null;

            return targetNpc;
        }

        public static async Task<bool> SellItem(Unit localUnit, Unit targetNpc,RobotItem item, ETCancellationToken cancellationToken)
        {
            Scene clientScene = localUnit.ClientScene();
            Session session = clientScene.GetComponent<SessionComponent>().session;

            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();

            ETCancellationToken cancelWaitToken = new ETCancellationToken();
            async Task<bool> Send_C2G_BattlePickUpDropItemRequestAsync()
            {
                IResponse res = await session.Call(new C2G_SellingItemToNPCShop()
                {
                    NPCShopID = targetNpc.Id,
                    ItemUUID = item.Id,
                }, cancellationToken);
                if(cancellationToken.IsCancel())
                {
                    cancelWaitToken.Cancel();
                    return false;
                }
                if (res.Error != ErrorCode.ERR_Success)
                {
                    Log.Warning($"[{clientScene.Name}] C2G_SellingItemToNPCShop:{res.Error}");
                    cancelWaitToken.Cancel();
                    return false;
                }
                return true;
            }
            async Task<bool> Wait_ItemLeaveBackpackAsync()
            {
                await localUnit.GetComponent<ObjectWait>().Wait<Wait_Unit_ItemLeaveBackpack>(cancelWaitToken);
                if (cancelWaitToken.IsCancel()) return false;
                return true;
            }

            tasks.Add(Send_C2G_BattlePickUpDropItemRequestAsync());
            tasks.Add(Wait_ItemLeaveBackpackAsync());

            List<bool> rets = await TaskHelper.WaitAll(tasks);
            if (cancellationToken.IsCancel()) return false;
            foreach (bool ret in rets)
            {
                if (ret == false) return false;
            }
            return true;
        }
    }
}
