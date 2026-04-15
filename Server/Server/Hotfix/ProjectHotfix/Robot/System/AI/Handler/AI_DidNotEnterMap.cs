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
    /// 没有进入地图
    /// </summary>
    [AIHandler]
    public class AI_DidNotEnterMap : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return 0;
            if (localUnit.CurrentMap == null) return 0;
            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            await ETModel.ET.TimerComponent.Instance.WaitAsync(1000 * 10, cancellationToken);
        }

    }
}
