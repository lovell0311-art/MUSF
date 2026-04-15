using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using System.Threading.Tasks;

namespace ETHotfix.Robot
{
    /// <summary>
    /// 宠物添加属性点
    /// </summary>
    [AIHandler]
    public class AI_PetsAddAttrPoint : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            RobotPetsWindowsComponent petsWin = clientScene.GetComponent<RobotPetsWindowsComponent>();
            foreach (RobotPetsInfo pet in petsWin.PetsDict.Values)
            {
                NumericComponent numeric = pet.GetComponent<NumericComponent>();
                if (numeric.GetAsInt((int)E_GameProperty.FreePoint) > 0) return 0;
            }
            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            Scene clientScene = aiComponent.ClientScene();
            Log.Console($"[{clientScene.Name}] 宠物添加属性点");
            RobotPetsWindowsComponent petsWin = clientScene.GetComponent<RobotPetsWindowsComponent>();

            using ListComponent<RobotPetsInfo> allPetsInfo = ListComponent<RobotPetsInfo>.Create();
            foreach (RobotPetsInfo petsInfo in petsWin.PetsDict.Values)
            {
                NumericComponent numeric = petsInfo.GetComponent<NumericComponent>();
                if (numeric.GetAsInt((int)E_GameProperty.FreePoint) > 0)
                {
                    allPetsInfo.Add(petsInfo);
                }
            }

            foreach(RobotPetsInfo petsInfo in allPetsInfo)
            {
                bool ret = await petsWin.AddAttributePointAsync(petsInfo, cancellationToken);
                if (cancellationToken.IsCancel()) return;
                if (ret == false) return;
            }
        }

    }
}
