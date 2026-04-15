using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    /// <summary>
    /// 拾取物品
    /// </summary>
    [AIHandler]
    public class AI_PickUpItem : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            // 检查视野范围有没有可以拾取的物品
            long nowTime = Help_TimeHelper.GetNowSecond();
            foreach (Unit item in localUnit.CurrentMap.ItemDict.Values)
            {
                RobotItemComponent itemComponent = item.GetComponent<RobotItemComponent>();
                if (itemComponent.KillerId.IndexOf(localUnit.Id) >= 0 ||
                    itemComponent.ProtectTick < nowTime)
                {
                    if(localUnit.CurrentMap.Astar.IsPass(item.Position.x,item.Position.y))
                    {
                        // 是可以到达的位置
                        return 0;
                    }
                }
            }
            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return;
            Log.Console($"[{clientScene.Name}] 拾取物品");
            RobotItemComponent itemComponent;
            while (true)
            {
                // TODO 查找最近的物品
                int sqrMagnitude = int.MaxValue;
                long nowTime = Help_TimeHelper.GetNowSecond();
                Unit targetItem = null;
                foreach (Unit item in localUnit.CurrentMap.ItemDict.Values)
                {
                    itemComponent = item.GetComponent<RobotItemComponent>();
                    if (itemComponent.KillerId.IndexOf(localUnit.Id) >= 0 || itemComponent.ProtectTick < nowTime)
                    {
                        if(localUnit.CurrentMap.Astar.IsPass(item.Position.x, item.Position.y))
                        {
                            // 且这个位置是可到达的
                            int sqrMagnitude2 = (item.Position - localUnit.Position).sqrMagnitude;
                            if (sqrMagnitude > sqrMagnitude2)
                            {
                                sqrMagnitude = sqrMagnitude2;
                                targetItem = item;
                            }
                        }
                    }
                }
                if (targetItem == null) return; // 周围没有能拾取的物品了

                Log.Console($"[{clientScene.Name}] 去拾取物品:{targetItem.Id},{targetItem.Position}");
                // 移动到物品所在的位置
                bool ret = await localUnit.MoveToUnitAsync(targetItem, cancellationToken);
                if (ret == false) return;
                // TODO 拾取物品
                ret = await RobotBackpackHelper.PickUpItem(localUnit, targetItem, cancellationToken);
                if (ret == false) return;

            }
        }

    }
}
