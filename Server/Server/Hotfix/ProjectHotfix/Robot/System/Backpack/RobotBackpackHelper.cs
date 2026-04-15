using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using CustomFrameWork;

namespace ETHotfix.Robot
{
    public static class RobotBackpackHelper
    {
        /// <summary>
        /// 拾取物品
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="targetItem"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<bool> PickUpItem(Unit unit,Unit targetItem, ETCancellationToken cancellationToken)
        {
            Scene clientScene = unit.ClientScene();
            Session session = clientScene.GetComponent<SessionComponent>().session;
            RobotItemComponent itemComponent = targetItem.GetComponent<RobotItemComponent>();
            RobotBackpackComponent backpack = unit.GetComponent<RobotBackpackComponent>();
            int x = 0;
            int y = 0;
            if (!backpack.ItemBox.CheckStatus(itemComponent.Config.X, itemComponent.Config.Y, ref x, ref y))
            {
                // 背包满了
                Log.Console($"[{clientScene.Name}] 背包满了");
                backpack.IsFull = true;
                return false;
            }
            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();

            ETCancellationToken cancelWaitToken = new ETCancellationToken();
            async Task<bool> Send_C2G_BattlePickUpDropItemRequest()
            {
                IResponse res = await session.Call(new C2G_BattlePickUpDropItemRequest()
                {
                    PosX = targetItem.Position.x,
                    PosY = targetItem.Position.y,
                    InstanceId = targetItem.Id,
                    PosInBackpackX = x,
                    PosInBackpackY = y,
                }, cancellationToken);
                if(cancellationToken.IsCancel())
                {
                    cancelWaitToken.Cancel();
                    return false;
                }
                if (res.Error != ErrorCode.ERR_Success)
                {
                    Log.Warning($"[{clientScene.Name}] C2G_BattlePickUpDropItemRequest:{res.Error}");
                    cancelWaitToken.Cancel();
                    return false;
                }
                return true;
            }
            async Task<bool> Wait_ItemLeaveMap()
            {
                await targetItem.GetComponent<ObjectWait>().Wait<Wait_Unit_LeaveMap>(cancelWaitToken);
                if (cancelWaitToken.IsCancel()) return false;
                return true;
            }

            tasks.Add(Send_C2G_BattlePickUpDropItemRequest());
            tasks.Add(Wait_ItemLeaveMap());

            List<bool> rets = await TaskHelper.WaitAll(tasks);
            if (cancellationToken.IsCancel()) return false;
            foreach (bool ret in rets) if (ret == false) return false;

            return true;
        }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="usedItem"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<bool> UseItem(Unit unit,RobotItem usedItem, ETCancellationToken cancellationToken = null)
        {
            Scene clientScene = unit.ClientScene();
            Session session = clientScene.GetComponent<SessionComponent>().session;

            C2G_PlayerUseItemInTheBackpack msg = new C2G_PlayerUseItemInTheBackpack();
            msg.ItemUUID = usedItem.Id;
            IResponse res = await session.Call(msg, cancellationToken);
            if (cancellationToken != null && cancellationToken.IsCancel()) return false;
            if (res.Error != ErrorCode.ERR_Success) return false;
            return true;
        }
    }
}
