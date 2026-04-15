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
    public class AI_UseGM : AAIHandler
    {
        public override int Check(AIComponent aiComponent, AI_Config aiConfig)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            NumericComponent numeric = localUnit.GetComponent<NumericComponent>();
            if (numeric.GetAsInt((int)E_GameProperty.GoldCoin) < 1000000) return 0;
            if (numeric.GetAsInt((int)E_GameProperty.Property_Strength) < 2000) return 0;
            if (numeric.GetAsInt((int)E_GameProperty.Property_Willpower) < 2000) return 0;
            if (numeric.GetAsInt((int)E_GameProperty.Property_Agility) < 2000) return 0;
            if (numeric.GetAsInt((int)E_GameProperty.Property_BoneGas) < 2000) return 0;
            if (numeric.GetAsInt((int)E_GameProperty.Property_Command) < 2000) return 0;
            if (numeric.GetAsInt((int)E_GameProperty.PROP_MP_MAX) < 10000) return 0;
            if (numeric.GetAsInt((int)E_GameProperty.PROP_MP) < 100) return 0;

            return 1;
        }
        public override async Task Execute(AIComponent aiComponent, AI_Config aiConfig, ETCancellationToken cancellationToken)
        {
            Scene clientScene = aiComponent.ClientScene();
            Unit localUnit = UnitHelper.GetLocalUnitFromClientScene(clientScene);
            if (localUnit == null) return;
            Log.Console($"[{clientScene.Name}] 开始使用GM");

            using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();

            async Task<bool> StudyAllSkillCoroutine()
            {
                // 学习技能
                await clientScene.GetComponent<SessionComponent>().session.Call(new C2G_GMRequest
                {
                    Command = "学习技能",
                }, cancellationToken);
                if (cancellationToken.IsCancel()) return false;
                return true;
            }

            async Task<bool> SetGoldCoinCoroutine()
            {
                // 获取金币
                if (localUnit.GetComponent<NumericComponent>().GetAsInt((int)E_GameProperty.GoldCoin) < 1000000)
                {
                    C2G_GMRequest mRequest = new C2G_GMRequest()
                    {
                        Command = "获取金币",
                    };
                    mRequest.Parameter.Add(1000000);
                    IResponse mResponse = await clientScene.GetComponent<SessionComponent>().session.Call(mRequest, cancellationToken);
                    if (cancellationToken.IsCancel()) return false;
                }
                return true;
            }

            async Task<bool> SetLevel400Coroutine()
            {
                // 获取等级
                if (localUnit.GetComponent<NumericComponent>().GetAsInt((int)E_GameProperty.Level) < 400)
                {
                    C2G_GMRequest mRequest = new C2G_GMRequest()
                    {
                        Command = "获取等级",
                    };
                    mRequest.Parameter.Add(400);
                    IResponse mResponse = await clientScene.GetComponent<SessionComponent>().session.Call(mRequest, cancellationToken);
                    if (cancellationToken.IsCancel()) return false;
                }
                return true;
            }

            async Task<bool> SetPropCoroutine(E_GameProperty propId, int val)
            {
                // 修改属性
                if (localUnit.GetComponent<NumericComponent>().GetAsInt((int)propId) < val)
                {
                    C2G_GMRequest mRequest = new C2G_GMRequest()
                    {
                        Command = "修改属性",
                    };
                    mRequest.Parameter.Add((int)propId);
                    mRequest.Parameter.Add(val);
                    IResponse mResponse = await clientScene.GetComponent<SessionComponent>().session.Call(mRequest, cancellationToken);
                    if (cancellationToken.IsCancel()) return false;
                }
                return true;
            }

            tasks.Add(StudyAllSkillCoroutine());
            tasks.Add(SetGoldCoinCoroutine());
            //tasks.Add(SetLevel400Coroutine());
            tasks.Add(SetPropCoroutine(E_GameProperty.Property_Strength, 2000));
            tasks.Add(SetPropCoroutine(E_GameProperty.Property_Willpower, 2000));
            tasks.Add(SetPropCoroutine(E_GameProperty.Property_Agility, 2000));
            tasks.Add(SetPropCoroutine(E_GameProperty.Property_BoneGas, 2000));
            tasks.Add(SetPropCoroutine(E_GameProperty.Property_Command, 2000));
            tasks.Add(SetPropCoroutine(E_GameProperty.PROP_MP_MAX, 10000));
            tasks.Add(SetPropCoroutine(E_GameProperty.PROP_MP, 10000));

            await TaskHelper.WaitAll(tasks);
            if (cancellationToken.IsCancel()) return;
        }

    }
}
