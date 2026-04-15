using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using ETModel.Robot;
using ETModel.Robot.WaitType;
using ETModel.ET;
using CustomFrameWork;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    /// <summary>
    /// 攻击怪物
    /// </summary>
    [AIHandler]
    public class AI_AttackMonster : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit.CurrentMap.IsSafeArea(localUnit.Position)) return 1;
            if (localUnit.CurrentMap.MonsterDict.Count == 0) return 1;
            return 0;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return;

            while (true)
            {
                if (localUnit.CurrentMap == null) return;
                if (localUnit.CurrentMap.MonsterDict.Count == 0) return;

                // 找到最近的怪物
                Unit targetMonster = localUnit.CurrentMap.MonsterDict.Values.OrderBy(monster => (localUnit.Position - monster.Position).sqrMagnitude).First();

                bool ret = await RobotBattleHelper.AttackUnit(localUnit, targetMonster, cancellationToken);
                if (ret == false) return;
            }
        }

    }
}
