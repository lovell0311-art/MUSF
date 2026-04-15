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
    /// 死亡尝试复活
    /// </summary>
    [AIHandler]
    public class AI_DeathTryResurrect : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit.GetComponent<NumericComponent>().GetAsInt((int)E_GameProperty.PROP_HP) <= 0) return 0;
            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            await ETModel.ET.TimerComponent.Instance.WaitAsync(1000 * 10, cancellationToken);
        }

    }
}
