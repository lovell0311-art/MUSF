using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;
using System.Threading.Tasks;
using CustomFrameWork;
using UnityEngine;


namespace ETHotfix.Robot
{
    public static class MoveHelper
    {
        // 可以多次调用，多次调用的话会取消上一次的协程
        public static Task<bool> MoveToAsync(this Unit unit, Vector2Int targetPos, ETCancellationToken cancellationToken = null)
        {
            return MoveToAsync(unit, targetPos, null,cancellationToken);
        }

        // 可以多次调用，多次调用的话会取消上一次的协程
        public static async Task<bool> MoveToAsync(this Unit unit,Vector2Int targetPos,Func<bool> stop = null,ETCancellationToken cancellationToken = null)
        {
            MoveComponent move = unit.GetComponent<MoveComponent>();
            Scene clientScene = unit.ClientScene();
            move.MovePointList.Clear();
            for (int i = 0; i < 100; ++i)
            {
                unit.CurrentMap.Astar.FindPath(unit.Position, targetPos, ref move.MovePointList, 1000);
                long NextMoveTime = Help_TimeHelper.GetNow();
                if (move.MovePointList.Count > 0)
                {
                    // 第一个点是自己所在的位置
                    move.MovePointList.RemoveFirst();
                }
                else
                {
                    return false;
                }
                while (move.MovePointList.Count > 0)
                {
                    Vector2Int point = move.MovePointList.First.Value;
                    move.MovePointList.RemoveFirst();
                    if (!unit.CurrentMap.Astar.IsPass(point.x, point.y))
                    {
                        break;
                    }

                    C2G_MovePosRequest msg = new C2G_MovePosRequest()
                    {
                        X = point.x,
                        Y = point.y,
                        Angle = 0,
                    };
                    IResponse res = await clientScene.GetComponent<SessionComponent>().session.Call(msg, cancellationToken);
                    if (cancellationToken.IsCancel()) return false;
                    if (res.Error != ErrorCode.ERR_Success) return false;
                    unit.CurrentMap?.LocalUnitMove(unit, point);
                    NextMoveTime += move.MoveTimeInterval;
                    Log.Debug($"[{clientScene.Name}] 移动=>({point.x},{point.y}) count={move.MovePointList.Count} IsRun:{move.IsRun}");
                    bool ret = await ETModel.ET.TimerComponent.Instance.WaitTillAsync(NextMoveTime, cancellationToken);
                    if (ret == false) return false;

                    if (stop != null)
                    {
                        if (stop.Invoke()) return true;
                    }
                }
                if (move.MovePointList.Count == 0) break;
            }
            return true;
        }
        // 可以多次调用，多次调用的话会取消上一次的协程
        public static Task<bool> MoveToUnitAsync(this Unit unit, Unit targetUnit, ETCancellationToken cancellationToken = null)
        {
            return MoveToUnitAsync(unit, targetUnit, null, cancellationToken);
        }

        public static async Task<bool> MoveToUnitAsync(this Unit self, Unit targetUnit, Func<bool> stop = null, ETCancellationToken cancellationToken = null)
        {
            long instanceId = targetUnit.InstanceId;
            for (int i =0;i<1000;++i)
            {
                if (instanceId != targetUnit.InstanceId)
                {
                    break;
                }
                bool ret = await self.MoveToAsync(targetUnit.Position, stop, cancellationToken);
                if (ret == false) return false;
                if (stop != null)
                {
                    if (stop.Invoke()) return true;
                }
                if ((targetUnit.Position - self.Position).sqrMagnitude == 0)
                {
                    // 到达目标点
                    return true;
                }
            }
            return false;
        }

        // 可以多次调用，多次调用的话会取消上一次的协程
        public static Task<bool> MoveToPosAsync(this Unit unit, Vector2Int target, ETCancellationToken cancellationToken = null)
        {
            return MoveToPosAsync(unit, target, null, cancellationToken);
        }

        public static async Task<bool> MoveToPosAsync(this Unit self, Vector2Int target, Func<bool> stop = null, ETCancellationToken cancellationToken = null)
        {
            for (int i = 0; i < 1000; ++i)
            {
                bool ret = await self.MoveToAsync(target, stop, cancellationToken);
                if (ret == false) return false;
                if (stop != null)
                {
                    if (stop.Invoke()) return true;
                }
                if ((target - self.Position).sqrMagnitude == 0)
                {
                    // 到达目标点
                    return true;
                }
            }
            return false;
        }

    }
}
