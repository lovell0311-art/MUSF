using System;
using UnityEngine.SceneManagement;
using ETModel;
using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

namespace ETHotfix
{
    [ObjectSystem]
    public class SceneComponentAwake : AwakeSystem<SceneComponent>
    {
        public override void Awake(SceneComponent self)
        {
            self.Awake();
        }
    }
    /// <summary>
    /// 场景加载组件
    /// </summary>
    public class SceneComponent : Component
    {
        public static SceneComponent Instance;
        public string CurrentSceneName="Init";//当前场景的名字
       
        public int CurrentSceneIndex=0;//当前场景的ID

        public Action RemoveLoadingAction;
        private AsyncOperation asyncOperation;
        private int sceneLoadSerial;
        private int sceneLoadProgressTick;
        private bool sceneLoadFinished;

        

        public void Awake()
        {
            Instance = this;
            SafeAreaComponent.Instances.CurrentSceneName = () => CurrentSceneName;

            //TriggerEvents.Instance.AddChangeSceneAction(ChangeScene);
        }



        /// <summary>
        /// 场景加载
        /// </summary>
        /// <param name="sceneName">地图名字</param>
        /// <param name="callback">回调函数</param>
        /// <param name="checkSameScene">好友同地图传送是否允许重复加载</param>
        public  void LoadScene(string sceneName, Action callback = null, bool checkSameScene = true)
        {
            LoginStageTrace.Append($"SceneLoad start target={sceneName} current={CurrentSceneName} checkSame={checkSameScene}");
           
            if (checkSameScene && CurrentSceneName == sceneName)
            {
                LoginStageTrace.Append($"SceneLoad same-scene short-circuit target={sceneName}");
              
                callback?.Invoke();//执行回调函数
                UIComponent.Instance.Remove(UIType.UISceneLoading);
                if (sceneName != "ChooseRole")
                {
                    LoadSceneComplete().Coroutine();
                }
                else
                {
                    LoginStageTrace.Append("SceneLoad same-scene skip load-complete target=ChooseRole");
                }
                return;

                //客户端场景加载完成
                async ETVoid LoadSceneComplete()
                {
                    G2C_LoadSceneCompleteResponse g2C_LoadScene = (G2C_LoadSceneCompleteResponse)await SessionComponent.Instance.Session.Call(new C2G_LoadSceneCompleteRequest());
                }
            }
          
            CurrentSceneName = sceneName;
            CurrentSceneIndex = (int)CurrentSceneName.ToEnum<SceneName>();
            string sceneBundleName = sceneName.StringToAB();
            LoginStageTrace.Append(
                $"SceneLoad resolve target={sceneName} sceneIndex={CurrentSceneIndex} " +
                $"bundle={sceneBundleName} async={Define.IsAsync}");
            try
            {
                if (sceneName != "ChooseRole")
                {
                    LoginStageTrace.Append($"SceneLoad astar begin target={sceneName}");
                    if (AstarComponent.Instance == null)
                    {
                        LoginStageTrace.Append($"SceneLoad astar skipped target={sceneName} reason=instance-null");
                    }
                    else
                    {
                        AstarComponent.Instance.LoadSceneNodes(sceneName, "null");
                        LoginStageTrace.Append(
                            $"SceneLoad astar finish target={sceneName} " +
                            $"width={AstarComponent.Instance.Width} height={AstarComponent.Instance.Height} " +
                            $"nodesNull={AstarComponent.Instance.Nodes == null}");
                    }
                }
            }
            catch (Exception e)
            {
                LoginStageTrace.Append($"SceneLoad astar failed target={sceneName} type={e.GetType().Name} message={e.Message}");
                Log.Error(e.ToString());
            }

            try
            {
                if (SafeAreaComponent.Instances != null)
                {
                    LoginStageTrace.Append($"SceneLoad safe-area begin target={sceneName}");
                    SafeAreaComponent.Instances.GetCurSceneSafeAreas(sceneName);
                    LoginStageTrace.Append($"SceneLoad safe-area finish target={sceneName}");
                }
            }
            catch (Exception e)
            {
                LoginStageTrace.Append($"SceneLoad safe-area failed target={sceneName} type={e.GetType().Name} message={e.Message}");
                Log.Error(e.ToString());
            }

            try
            {
                if (TransferPointTools.Instances != null)
                {
                    LoginStageTrace.Append($"SceneLoad transfer begin target={sceneName}");
                    TransferPointTools.Instances.GetCurSceneTransferPoints(sceneName);
                    LoginStageTrace.Append($"SceneLoad transfer finish target={sceneName}");
                }
            }
            catch (Exception e)
            {
                LoginStageTrace.Append($"SceneLoad transfer failed target={sceneName} type={e.GetType().Name} message={e.Message}");
                Log.Error(e.ToString());
            }
         
            //加载场景资源
            AssetBundleComponent resourcesComponent = ETModel.Game.Scene.GetComponent<AssetBundleComponent>();
          
            try
            {
                if (Define.IsAsync)
                {
                    var nowSceneName = SceneManager.GetActiveScene().name;
                    if (nowSceneName != sceneName)
                    {
                        if (nowSceneName.ToLower() != "init")
                        {
                            //GlobalDataManager.GCClear();
                            string lastBundleName = nowSceneName.StringToAB();
                            LoginStageTrace.Append($"SceneLoad unload previous bundle scene={nowSceneName} bundle={lastBundleName}");
                            resourcesComponent.UnloadBundle(lastBundleName);//卸载之前场景资源
                        }
                        LoginStageTrace.Append($"SceneLoad load current bundle scene={sceneName} bundle={sceneBundleName}");
                        resourcesComponent.LoadBundle(sceneBundleName);
                    }

                }
                ++this.sceneLoadSerial;
                this.sceneLoadProgressTick = 0;
                this.sceneLoadFinished = false;
                LoginStageTrace.Append($"SceneLoad async create target={sceneName}");
                asyncOperation = SceneManager.LoadSceneAsync(sceneName);
                if (asyncOperation != null)
                {
                    int loadSerial = this.sceneLoadSerial;
                    AsyncOperation currentOperation = asyncOperation;
                    LoginStageTrace.Append($"SceneLoad async created target={sceneName} source=poll-only");
                    BarodCastLoadProcess(currentOperation, loadSerial).Coroutine();
                    return;
                }
                LoginStageTrace.Append($"SceneLoad async create returned null target={sceneName}");
            }
            catch (Exception e)
            {
                LoginStageTrace.Append($"SceneLoad exception target={sceneName} type={e.GetType().Name} message={e.Message}");
                Log.DebugRed($"{e.ToString()}");
            }
            BarodCastLoadProcess(null, this.sceneLoadSerial).Coroutine();

            async ETVoid BarodCastLoadProcess(AsyncOperation currentOperation, int loadSerial)
            {
                while (currentOperation != null && loadSerial == this.sceneLoadSerial && !this.sceneLoadFinished)
                {
                    await TimerComponent.Instance.WaitAsync(100);

                    if (currentOperation == null || loadSerial != this.sceneLoadSerial || this.sceneLoadFinished)
                    {
                        break;
                    }

                    ++this.sceneLoadProgressTick;
                    if (this.sceneLoadProgressTick % 5 == 0)
                    {
                        LoginStageTrace.Append($"SceneLoad progress target={sceneName} progress={currentOperation.progress:0.000} isDone={currentOperation.isDone}");
                    }

                    Game.EventCenter.EventTrigger<float>(EventTypeId.LOAD_SCENE_PROGRESS, currentOperation.progress);

                    if (currentOperation.isDone)
                    {
                        this.OnSceneLoadDone(loadSerial, sceneName, callback, currentOperation, "poll");
                        break;
                    }
                }
            }
        }

        private void OnSceneLoadDone(int loadSerial, string sceneName, Action callback, AsyncOperation currentOperation, string source)
        {
            if (loadSerial != this.sceneLoadSerial)
            {
                LoginStageTrace.Append($"SceneLoad done skip-stale target={sceneName} source={source} loadSerial={loadSerial} currentSerial={this.sceneLoadSerial}");
                return;
            }

            if (this.sceneLoadFinished)
            {
                LoginStageTrace.Append($"SceneLoad done duplicate target={sceneName} source={source}");
                return;
            }

            this.sceneLoadFinished = true;
            this.asyncOperation = null;
            string progressText = currentOperation != null ? currentOperation.progress.ToString("0.000") : "null";
            LoginStageTrace.Append($"SceneLoad done target={sceneName} source={source} progress={progressText}");

            try
            {
                callback?.Invoke();
                Game.EventSystem.Run(EventIdType.SceneLoadFnish, sceneName);
            }
            catch (Exception e)
            {
                LoginStageTrace.Append($"SceneLoad done exception target={sceneName} source={source} type={e.GetType().Name} message={e.Message}");
                Log.DebugRed(e.ToString());
            }
        }
 
        /// <summary>
        /// 传送门切换场景
        /// </summary>
        /// <param name="sceneName"></param>
        public void ChangeScene(int sceneID)
        {
          
        }
    }
}
