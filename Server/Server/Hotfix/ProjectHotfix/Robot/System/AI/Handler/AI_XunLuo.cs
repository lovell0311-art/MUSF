using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix.Robot
{
    [AIHandler]
    public class AI_XunLuo : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            long sec = TimeHelper.ClientNow() / 1000 % 15;
            if (sec < 10)
            {
                return 0;
            }
            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return;
            Log.Console($"[{clientScene.Name}] 开始巡逻");

            while(true)
            {
                Vector2Int target = MapHelper.GetRandPos(localUnit.CurrentMap);

                bool ret = await localUnit.MoveToAsync(target, cancellationToken);
                if (ret == false) return;
                ret = await ETModel.ET.TimerComponent.Instance.WaitAsync(100, cancellationToken);
                if (ret == false) return;
            }
        }

    }
}
