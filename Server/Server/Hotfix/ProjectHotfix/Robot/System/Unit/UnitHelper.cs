using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;

namespace ETHotfix.Robot
{
    public static class UnitHelper
    {
        public static Unit GetLocalUnitFromClientScene(Scene clientScene)
        {
            PlayerComponent playerComponent = clientScene.GetComponent<PlayerComponent>();
            return playerComponent.LocalUnit;
        }

        public static Unit GetUnitFromClientScene(Scene clientScene,long unitId)
        {
            RobotMapComponent robotMapComponent = clientScene.GetComponent<RobotMapComponent>();
            Unit targetUnit = null;
            if(robotMapComponent.CurrentMap != null)
            {
                targetUnit = robotMapComponent.CurrentMap.GetUnit(unitId);
            }
            if (targetUnit == null)
            {
                // 是不是本地玩家
                Unit localUnit = clientScene.GetComponent<PlayerComponent>().LocalUnit;
                if(localUnit != null && unitId == localUnit.Id)
                {
                    targetUnit = localUnit;
                }
            }
            return targetUnit;
        }
    }
}
