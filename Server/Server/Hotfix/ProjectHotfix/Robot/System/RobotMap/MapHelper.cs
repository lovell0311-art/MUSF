using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using CustomFrameWork;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix.Robot
{
    public static class MapHelper
    {
        /// <summary>
        /// 取随机点
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Vector2Int GetRandPos(this Map self)
        {
            return new Vector2Int()
            {
                x = Help_RandomHelper.Range(0, self.Astar.Wight),
                y = Help_RandomHelper.Range(0, self.Astar.Height)
            };
        }

        public static void UnitEnter(this Map self,Unit enteredUnit,Vector2Int pos)
        {
            enteredUnit.Position = pos;
            if(self.AddUnit(enteredUnit))
            {
                if(!enteredUnit.IgnoreCollision)
                {
                    self.Astar.OtherCollision[pos.x, pos.y] += 1;
                }
            }
        }

        public static void UnitMove(this Map self, Unit movementUnit, Vector2Int targetPos)
        {
            if(movementUnit.CurrentMap.InstanceId == self.InstanceId)
            {
                if(!movementUnit.IgnoreCollision)
                {
                    self.Astar.OtherCollision[movementUnit.Position.x, movementUnit.Position.y] -= 1;
                    self.Astar.OtherCollision[targetPos.x, targetPos.y] += 1;
                }
                movementUnit.Position = targetPos;
            }
        }

        public static void UnitLeave(this Map self, Unit leavedUnit)
        {
            if (self.RemoveUnit(leavedUnit.Id))
            {
                if(!leavedUnit.IgnoreCollision)
                {
                    self.Astar.OtherCollision[leavedUnit.Position.x, leavedUnit.Position.y] -= 1;
                }
            }
        }

        public static void LocalUnitMove(this Map self, Unit movementUnit, Vector2Int targetPos)
        {
            int curCellX = targetPos.x / 12;
            int curCellY = targetPos.y / 12;

//             foreach(Unit unit in self.UnitDict.Values)
//             {
//                 if (unit.Id == movementUnit.Id) continue;
//                 int unitCellX = unit.Position.x / 12;
//                 int unitCellY = unit.Position.y / 12;
//                 if(Math.Abs(unitCellX - curCellX) > 1 ||
//                     Math.Abs(unitCellY - curCellY) > 1)
//                 {
//                     // 单位离开视野
//                     self.LeavedUnitQueue.Enqueue(unit);
//                 }
//             }

            self.UnitMove(movementUnit, targetPos);

//             while (self.LeavedUnitQueue.Count > 0)
//             {
//                 self.LeavedUnitQueue.Dequeue().Dispose();
//             }

        }


        /// <summary>
        /// 传送到指定地图
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public static async Task<bool> TransferToMap(Unit localUnit, int mapId, ETCancellationToken cancellationToken)
        {
            if (!TransferPointManager.Instance.MapId2TransferPoint.ContainsKey(mapId)) return false;
            Map_TransferPointConfig config =  TransferPointManager.Instance.MapId2TransferPoint.GetOne(mapId);
            return await RequestTransferTo(localUnit,config.Id,cancellationToken);
        }

        /// <summary>
        /// 请求传送
        /// </summary>
        /// <param name="transferPointId"></param>
        /// <returns></returns>
        public static async Task<bool> RequestTransferTo(Unit localUnit, int transferPointId, ETCancellationToken cancellationToken)
        {
            Scene clientScene = localUnit.ClientScene();
            Session session = clientScene.GetComponent<SessionComponent>().session;

            ETCancellationToken cancelWaitToken = new ETCancellationToken();
            async Task<bool> SendC2G_MapDeliveryRequestCoroutine()
            {
                C2G_MapDeliveryRequest msg = new C2G_MapDeliveryRequest();
                msg.MapId = transferPointId;
                IResponse res = await session.Call(msg, cancellationToken);
                if(cancellationToken.IsCancel())
                {
                    cancelWaitToken.Cancel();
                    return false;
                }
                if (res.Error != ErrorCode.ERR_Success)
                {
                    Log.Warning($"[{clientScene.Name}] C2G_MapDeliveryRequest:{res.Error}");
                    cancelWaitToken.Cancel();
                    return false;
                }
                return true;
            }
            
            async Task<bool> Wait_MapChangeFinishCoroutine()
            {
                await clientScene.GetComponent<ObjectWait>().Wait<Wait_MapChangeFinish>(cancelWaitToken);
                if (cancelWaitToken.IsCancel()) return false;
                return true;
            }

            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();

            tasks.Add(SendC2G_MapDeliveryRequestCoroutine());
            tasks.Add(Wait_MapChangeFinishCoroutine());

            List<bool> rets = await TaskHelper.WaitAll(tasks);
            if (cancellationToken.IsCancel()) return false;
            foreach(bool ret in rets)
            {
                if (ret == false) return false;
            }

            return true;
        }

    }
}
