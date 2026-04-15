using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System;
using System.Threading;

namespace ETHotfix
{
    /// <summary>
    /// 场景加载完成
    /// </summary>
    [Event(EventIdType.SceneLoadFnish)]
    public class Event_SceneLoadFinish : AEvent<string>
    {
        public override void Run(string name)
        {
            SceneName sceneName = name.ToEnum<SceneName>();//将string转为枚举
            LoginStageTrace.Append($"SceneLoadFinish event start scene={sceneName}");
            if(sceneName != SceneName.EMoGuangChang && sceneName != SceneName.XueSeChengBao)
            {
                UIMainComponent.Instance?.ExitFuBenBtn?.gameObject.SetActive(false);
            }
            LoadComponentEvent(sceneName);
            if (sceneName != SceneName.ChooseRole)
            {
                TraceWorldState(sceneName).Coroutine();
                TryRunDiagnosticMapDelivery(sceneName).Coroutine();
            }
            // await TimerComponent.Instance.WaitAsync(1500);
            UIComponent.Instance.Remove(UIType.UISceneLoading);  //移除场景加载面板
            GlobalDataManager.IsEnteringGame = false;
            LoginStageTrace.Append($"SceneLoadFinish remove loading scene={sceneName}");

            TaskDatas.AutoNavCallBack?.Invoke();

            TransformPointManager.Instance.Clear();
            if (UIComponent.Instance.Get(UIType.UIMainCanvas) != null)
            {
                LoginStageTrace.Append($"SceneLoadFinish update minimap scene={sceneName} sceneId={(int)sceneName}");
                UIMainComponent.Instance.UpdateMiniMap(UIMainComponent.Instance.miniMap, (int)sceneName);//更新小地图

                UIMainComponent.Instance.ShowSceneName((int)sceneName);
                //暂停挂机
                if (UnitEntityComponent.Instance.LocalRole.GetComponent<RoleOnHookComponent>() is RoleOnHookComponent onHookComponent && onHookComponent.IsOnHooking)
                {
                    UIMainComponent.Instance.HookTog.isOn = false;
                }
            }
            SoundComponent.Instance.SetBgSoundMute(LocalDataJsonComponent.Instance.gameSetInfo.CloseMusic);
            if (sceneName != SceneName.ChooseRole)
            {
                LoadSceneComplete().Coroutine();
            }
            else
            {
                LoginStageTrace.Append("SceneLoadFinish skip load-complete scene=ChooseRole");
            }

            async ETVoid LoadSceneComplete()
            {
                LoginStageTrace.Append($"SceneLoadFinish load-complete request scene={sceneName}");
                G2C_LoadSceneCompleteResponse g2C_LoadScene = (G2C_LoadSceneCompleteResponse)await SessionComponent.Instance.Session.Call(new C2G_LoadSceneCompleteRequest());
                LoginStageTrace.Append($"SceneLoadFinish load-complete response scene={sceneName} error={g2C_LoadScene.Error}");
            }

            async ETVoid TryRunDiagnosticMapDelivery(SceneName currentScene)
            {
                int transferId = LoginDiagnosticOptions.AutoMapDeliveryTargetId;
                if (transferId <= 0)
                {
                    return;
                }

                Map_TransferPointConfig transferConfig = ConfigComponent.Instance.GetItem<Map_TransferPointConfig>(transferId);
                if (transferConfig == null)
                {
                    LoginStageTrace.Append($"DiagMapDelivery missing transferId={transferId} currentScene={currentScene}");
                    LoginDiagnosticOptions.ClearAutoMapDeliveryTarget();
                    return;
                }

                if (transferConfig.MapId == (int)currentScene)
                {
                    LoginStageTrace.Append($"DiagMapDelivery already-at-target transferId={transferId} mapId={transferConfig.MapId} scene={currentScene}");
                    LoginDiagnosticOptions.ClearAutoMapDeliveryTarget();
                    return;
                }

                if (SessionComponent.Instance?.Session == null)
                {
                    LoginStageTrace.Append($"DiagMapDelivery skip no-session transferId={transferId} currentScene={currentScene}");
                    return;
                }

                await TimerComponent.Instance.WaitAsync(400);
                if (SessionComponent.Instance?.Session == null)
                {
                    LoginStageTrace.Append($"DiagMapDelivery skip session-lost transferId={transferId} currentScene={currentScene}");
                    return;
                }

                LoginStageTrace.Append(
                    $"DiagMapDelivery request transferId={transferId} mapId={transferConfig.MapId} currentScene={currentScene}");
                G2C_MapDeliveryResponse response =
                    (G2C_MapDeliveryResponse)await SessionComponent.Instance.Session.Call(
                        new C2G_MapDeliveryRequest { MapId = transferId });
                LoginStageTrace.Append(
                    $"DiagMapDelivery response transferId={transferId} mapId={transferConfig.MapId} error={response.Error}");
                if (response.Error == 0)
                {
                    LoginDiagnosticOptions.ClearAutoMapDeliveryTarget();
                }
            }

            async ETVoid TraceWorldState(SceneName currentScene)
            {
                LoginStageTrace.AppendWorldSnapshot($"SceneLoadFinish {currentScene} immediate");
                await TimerComponent.Instance.WaitAsync(500);
                LoginStageTrace.AppendWorldSnapshot($"SceneLoadFinish {currentScene} +500ms");
                await TimerComponent.Instance.WaitAsync(1500);
                LoginStageTrace.AppendWorldSnapshot($"SceneLoadFinish {currentScene} +2000ms");
            }
        }
        private void LoadComponentEvent(SceneName sceneName)
        {
            LoginStageTrace.Append($"SceneLoadFinish component event scene={sceneName}");
            switch (sceneName)
            {
                case SceneName.ChooseRole:

                    GlobalDataManager.ChangeSceneIsChooseRole = false;
                    LoginStageTrace.Append("SceneLoadFinish run choose-role event");
                    Game.EventSystem.Run(EventIdType.LoadScene_ChooseRole);
                    break;
                case SceneName.YongZheDaLu:
                    Game.EventSystem.Run(EventIdType.LoadScene_YongZheDaLu);
                    break;
                case SceneName.XianZongLin:
                    Game.EventSystem.Run(EventIdType.LoadScene_XianZongLin);
                    break;
                case SceneName.DiXiaCheng:
                    Game.EventSystem.Run(EventIdType.LoadScene_DiXiaCheng);
                    break;
                case SceneName.HuanShuYuan:
                    Game.EventSystem.Run(EventIdType.LoadScene_HuanShuYuan);
                    break;
                case SceneName.BingFengGu:
                    Game.EventSystem.Run(EventIdType.LoadScene_BingFengGu);
                    break;
                case SceneName.YaTeLanDiSi:
                    Game.EventSystem.Run(EventIdType.LoadScene_YaTeLanDiSi);
                    break;
                case SceneName.ShiLuoZhiTa:
                    Game.EventSystem.Run(EventIdType.LoadScene_ShiLuoZhiTa);
                    break;
                case SceneName.SiWangShaMo:
                    Game.EventSystem.Run(EventIdType.LoadScene_SiWangShaMo);
                    break;
                case SceneName.YouAnSenLin:
                    Game.EventSystem.Run(EventIdType.LoadScene_YouAnSenLin);
                    break;
                case SceneName.TianKongZhiCheng:
                    Game.EventSystem.Run(EventIdType.LoadScene_TianKongZhiCheng);
                    break;
                case SceneName.LangHunYaoSai:
                    Game.EventSystem.Run(EventIdType.LoadScene_LangHunYaoSai);
                    break;
                case SceneName.KanTeLuFeiXu:
                    Game.EventSystem.Run(EventIdType.LoadScene_KanTeLuFeiXu);
                    break;
                case SceneName.KanTeLuYiZhi:
                    Game.EventSystem.Run(EventIdType.LoadScene_KanTeLuYiZhi);
                    break;
                case SceneName.BingShuangZhiCheng:
                    Game.EventSystem.Run(EventIdType.LoadScene_BingShuangZhiCheng);
                    break;
                case SceneName.FuHuaMoDi:
                    Game.EventSystem.Run(EventIdType.LoadScene_BingShuangZhiCheng_FuHuaMoDi);
                    break;
                case SceneName.AnNingChi:
                    Game.EventSystem.Run(EventIdType.LoadScene_AnNingChi);
                    break;
                case SceneName.XueSeChengBao:
                    Game.EventSystem.Run(EventIdType.LoadScene_XueSeChengBao);
                    break;
                case SceneName.EMoGuangChang:
                    Game.EventSystem.Run(EventIdType.LoadScene_EMoGuangChang);
                    break;
                case SceneName.GuZhanChang:
                    Game.EventSystem.Run(EventIdType.LoadScene_GuGuangChang);
                    break;
                case SceneName.kalima_map:
                    Game.EventSystem.Run(EventIdType.LoadScene_KaNiMa);
                    break;
                case SceneName.cangbaotu:
                    Game.EventSystem.Run(EventIdType.LoadScene_CangBaoTu);
                    break;
                case SceneName.WuMingDao:
                    Game.EventSystem.Run(EventIdType.LoadScene_WuMingDao);
                    break;
                case SceneName.Feiliya:
                    Game.EventSystem.Run(EventIdType.LoadScene_Feiliya);
                    break;
                case SceneName.ShiLianZhiDi:
                    Game.EventSystem.Run(EventIdType.LoadScene_ShiLianZhiDi);
                    break;
                case SceneName.GuZhanChang2:
                    Game.EventSystem.Run(EventIdType.LoadScene_GuGuangChang);
                    break;
                default:
                    break;

            }
        }
    }
}
