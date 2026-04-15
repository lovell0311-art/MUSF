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
    /// 宠物出战
    /// </summary>
    [AIHandler]
    public class AI_PetsGoToWar : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            RobotPetsWindowsComponent petsWin = clientScene.GetComponent<RobotPetsWindowsComponent>();
            if (petsWin.FirstPetsId == 0) return 1;
            if (!petsWin.PetsDict.TryGetValue(petsWin.FirstPetsId, out RobotPetsInfo petsInfo)) return 1;
            if (petsInfo.IsToWar == true) return 1;// 正在战斗中
            foreach (RobotPetsInfo pet in petsWin.PetsDict.Values)
            {
                if (pet.IsDeath == false) return 0;// 有活在的宠物
            }
            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            Scene clientScene = aiComponent.ClientScene();
            Log.Console($"[{clientScene.Name}] 宠物出战");
            RobotPetsWindowsComponent petsWin = clientScene.GetComponent<RobotPetsWindowsComponent>();
            RobotPetsInfo targetPet = null;
            foreach (RobotPetsInfo pet in petsWin.PetsDict.Values)
            {
                if (pet.IsDeath == false)
                {
                    // 活在的宠物
                    targetPet = pet;
                    break;
                }
            }

            bool ret = await petsWin.GoToWarAsync(targetPet.Id, cancellationToken);
            if (cancellationToken.IsCancel()) return;
            // 不管成功或失败，都重新获取下状态
            await petsWin.OpenPetsWindowsAsync(cancellationToken);
            if (cancellationToken.IsCancel()) return;
        }

    }
}
