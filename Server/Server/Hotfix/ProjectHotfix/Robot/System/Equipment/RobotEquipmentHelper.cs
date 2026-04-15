using System.Threading.Tasks;
using System.Collections.Generic;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using CustomFrameWork;

namespace ETHotfix.Robot
{
    public static class RobotEquipmentHelper
    {
        /// <summary>
        /// 穿装备
        /// </summary>
        /// <param name="localUnit"></param>
        /// <param name="item"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static async Task<bool> EquipItem(Unit localUnit,RobotItem item,EquipPosition position, ETCancellationToken cancellationToken)
        {
            Scene clientScene = localUnit.ClientScene();
            // 本地检查是否能穿戴
            RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
            if (equipment.GetItem(position) != null) return false;
            if (equipment.CanEquip(item) == false) return false;
            // 本地检查完成

            Session session = clientScene.GetComponent<SessionComponent>().session;

            ETCancellationToken cancelWaitToken = new ETCancellationToken();
            async Task<bool> C2G_EquipItemRequestCoroutine()
            {
                C2G_EquipItemRequest msg = new C2G_EquipItemRequest();
                msg.ItemUUID = item.Id;
                msg.EquipPosition = (int)position;

                IResponse res = await session.Call(msg, cancellationToken);
                if(cancellationToken.IsCancel())
                {
                    cancelWaitToken.Cancel();
                    return false;
                }
                if(res.Error != ErrorCode.ERR_Success)
                {
                    Log.Warning($"[{clientScene.Name}] C2G_EquipItemRequest.Error={res.Error}");
                    cancelWaitToken.Cancel();
                    return false;
                }
                return true;
            }

            async Task<bool> Wait_ItemLeaveBackpackCoroutine()
            {
                await localUnit.GetComponent<ObjectWait>().Wait<Wait_Unit_ItemLeaveBackpack>(cancelWaitToken);
                if (cancelWaitToken.IsCancel()) return false;
                return true;
            }

            async Task<bool> Wait_UnitEquipItemCoroutine()
            {
                await localUnit.GetComponent<ObjectWait>().Wait<Wait_Unit_EquipItem>(cancelWaitToken);
                if (cancelWaitToken.IsCancel()) return false;
                return true;
            }

            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();
            tasks.Add(C2G_EquipItemRequestCoroutine());
            tasks.Add(Wait_ItemLeaveBackpackCoroutine());
            tasks.Add(Wait_UnitEquipItemCoroutine());

            List<bool> rets = await TaskHelper.WaitAll(tasks);
            if (cancelWaitToken.IsCancel()) return false;
            foreach (bool ret in rets) if (ret == false) return false;

            return true;
        }

        /// <summary>
        /// 卸下
        /// </summary>
        /// <param name="localUnit"></param>
        /// <param name="item"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static async Task<bool> UnloadEquipItem(Unit localUnit, EquipPosition position, ETCancellationToken cancellationToken)
        {
            Scene clientScene = localUnit.ClientScene();
            // 本地检查是否能卸下
            RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
            RobotBackpackComponent backpack = localUnit.GetComponent<RobotBackpackComponent>();
            RobotItem item = equipment.GetItem(position);
            if (item == null) return false;
            int posX = 0;
            int posY = 0;
            if (backpack.ItemBox.CheckStatus(item.Config.X, item.Config.Y, ref posX, ref posY) == false)
            {
                // 背包满了
                Log.Console($"[{clientScene.Name}] 背包满了");
                backpack.IsFull = true;
                return false;
            }
            // 本地检查完成


            Session session = clientScene.GetComponent<SessionComponent>().session;

            ETCancellationToken cancelWaitToken = new ETCancellationToken();
            async Task<bool> C2G_UnloadEquipItemRequestCoroutine()
            {
                C2G_UnloadEquipItemRequest msg = new C2G_UnloadEquipItemRequest();
                msg.EquipPosition = (int)position;
                msg.PosInBackpackX = posX;
                msg.PosInBackpackY = posY;

                IResponse res = await session.Call(msg, cancellationToken);
                if (cancellationToken.IsCancel())
                {
                    cancelWaitToken.Cancel();
                    return false;
                }
                if (res.Error != ErrorCode.ERR_Success)
                {
                    Log.Warning($"[{clientScene.Name}] C2G_UnloadEquipItemRequest.Error={res.Error}");
                    cancelWaitToken.Cancel();
                    return false;
                }
                return true;
            }

            async Task<bool> Wait_UnitUnloadEquipItemCoroutine()
            {
                await localUnit.GetComponent<ObjectWait>().Wait<Wait_Unit_UnloadEquipItem>(cancelWaitToken);
                if (cancelWaitToken.IsCancel()) return false;
                return true;
            }

            async Task<bool> Wait_ItemIntoBackpackCoroutine()
            {
                await localUnit.GetComponent<ObjectWait>().Wait<Wait_Unit_ItemIntoBackpack>(cancelWaitToken);
                if (cancelWaitToken.IsCancel()) return false;
                return true;
            }


            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();
            tasks.Add(C2G_UnloadEquipItemRequestCoroutine());
            tasks.Add(Wait_UnitUnloadEquipItemCoroutine());
            tasks.Add(Wait_ItemIntoBackpackCoroutine());

            List<bool> rets = await TaskHelper.WaitAll(tasks);
            if (cancelWaitToken.IsCancel()) return false;
            foreach (bool ret in rets) if (ret == false) return false;

            return true;
        }

        /// <summary>
        /// 更换装备
        /// </summary>
        /// <param name="localUnit"></param>
        /// <param name="item"></param>
        /// <param name="position"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<bool> ReplaceEquipItem(Unit localUnit, RobotItem item, EquipPosition position, ETCancellationToken cancellationToken)
        {
            Scene clientScene = localUnit.ClientScene();
            RobotEquipmentComponent equipment = localUnit.GetComponent<RobotEquipmentComponent>();
            if (equipment.GetItem(position) != null)
            {
                bool ret = await UnloadEquipItem(localUnit, position, cancellationToken);
                if (cancellationToken.IsCancel()) return false;
                if(ret == false) return false;
            }
            return await EquipItem(localUnit, item, position, cancellationToken);
        }


        public static async Task<bool> RepairEquipItem(Unit localUnit,Unit targetNpc,List<EquipPosition> positionList, ETCancellationToken cancellationToken)
        {
            Scene clientScene = localUnit.ClientScene();
            Session session = clientScene.GetComponent<SessionComponent>().session;

            C2G_RepairEquipItemRequest msg = new C2G_RepairEquipItemRequest();
            foreach (EquipPosition position in positionList) msg.EquipPosition.Add((int)position);
            if (targetNpc != null)
            {
                msg.NpcUID = targetNpc.Id;
                msg.NpcPosX = targetNpc.Position.x;
                msg.NpcPosY = targetNpc.Position.y;
            }
            IResponse res = await session.Call(msg, cancellationToken);
            if (cancellationToken.IsCancel()) return false;
            if(res.Error != ErrorCode.ERR_Success)
            {
                Log.Warning($"[{clientScene.Name}] C2G_RepairEquipItemRequest:{res.Error}");
                return false;
            }
            return true;
        }
    }
}
