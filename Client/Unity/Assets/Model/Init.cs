using System;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;


namespace ETModel
{
    /// <summary>
    /// 充值
    /// </summary>
    public enum E_SDK
    {
     NONE,//默认
     HAO_YI_SDK,//好易
     TIKTOK_SDK,//抖音
     XY_SDK,//XYSDK
     SHOU_Q,//手Q
     ZHIFUBAO_WECHAT,//支付宝、微信支付
     LiZi,//梨子运营
     TapTap,
     HaXi,//其他（哈西网络）
     Other_2,//其他（备用）
     Other_3,//其他（备用）
     Other_4,//其他（备用）
     Other_5,//其他（备用）
    }

#if UNITY_EDITOR
[CustomEditor(typeof(Init))]
public class InitHelper : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("生产ApkConfig"))
        {
            var component = target as Init;
            component.CreatConfig();
        }
    }
}
#endif

    public class Init : MonoBehaviour
    {
       
        public static Init instance;

        [Header("开启真机模拟")]
        public bool IsAsync = false;
       
        [Header("开启新手引导")]
        public bool IsBeginnerGuide;
        [Header("SDK")]
        public E_SDK e_SDK = E_SDK.NONE;

        [Header("是否开启日志")]
        public bool logEnabled = false;
        [Header("代理ID")]
        public string agentStr = "test_20230510";

         string path = @"./Assets/StreamingAssets/ApkConfig.txt";
        ApkConfig apkconfig;

        public void CreatConfig() 
        {
            if (File.Exists(path) == false)
            {
                apkconfig = new ApkConfig() { AgentId = agentStr };
              
            }
            else
            {
                this.apkconfig = JsonHelper.FromJson<ApkConfig>(File.ReadAllText(path));
                this.apkconfig.AgentId = agentStr;
            }
            File.WriteAllText(path, JsonHelper.ToJson(this.apkconfig));
            Log.DebugGreen($"完成：{this.apkconfig.AgentId}");
           
        }
        private void Awake()
        {

            instance = this;

            Define.IsAsync = IsAsync;

#if !UNITY_EDITOR
            Define.IsAsync = true;
            path = Path.Combine(PathHelper.AppResPath4Web, "ApkConfig.txt");
         
#endif


            //关闭所有log
            Define.LogEnabled = true;
            Debug.unityLogger.logEnabled = true;
           
            // 限制帧率，尽量避免手机发烫
            QualitySettings.vSyncCount = 0;

            //渲染帧率
            //帧率60 没5帧渲染一次
            Application.targetFrameRate = 30;
            // 设置手机屏幕常亮
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.runInBackground = true;//后台运行

            

        }
        private void Start()
        {
           
            this.StartAsync().Coroutine();

            Guidance_Define.IsBeginnerGuide = IsBeginnerGuide;
#if !UNITY_EDITOR
            GetAgentTxt(path).Coroutine();
#endif
        }

        async ETVoid GetAgentTxt(string path)
        {
            using (UnityWebRequestAsync request = ComponentFactory.Create<UnityWebRequestAsync>())
            {
                await request.DownloadAsync(path);
                this.apkconfig = JsonHelper.FromJson<ApkConfig>(request.Request.downloadHandler.text);
                AgentTool.agentstr = this.apkconfig.AgentId;
                //Log.DebugGreen($"AgentStr:{AgentTool.agentstr}");
            }
        }

        private async ETVoid StartAsync()
        {
            try
            {
                //上下文同步
                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);


                //禁止销毁 在切换场景的时候
                DontDestroyOnLoad(gameObject);

                //遍历Model的程序集 缓存各个加了特性标签的对象
                Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);
                Game.Scene.AddComponent<TimerComponent>();//计时器
                Game.Scene.AddComponent<GlobalConfigComponent>();//全局配置
                Game.Scene.AddComponent<NetOuterComponent>();//外网组件 提供创建网络通道 在每个通道内部维护消息是收发处理
                Game.Scene.AddComponent<AssetBundleComponent>();//资源组件  提供AB加载
                Game.Scene.AddComponent<ResourcesComponent>();//资源组件 GameObject获取
                Game.Scene.AddComponent<UIComponent>();//UI组件 

                // 下载ab包
                await BundleHelper.DownloadBundle();

                //SDK组件
             
                Game.Scene.AddComponent<SdkCallBackComponent>();
                
                // 加载资源配置
                Game.Scene.AddComponent<ConfigComponent>();

                //寻路组件
                Game.Scene.AddComponent<AstarComponent>();
              
                Game.Scene.AddComponent<CameraComponent>();
                Game.Scene.AddComponent<CameraFollowComponent>();
                //音效组件
                Game.Scene.AddComponent<SoundComponent>();
                //Sprite组件
                Game.Scene.AddComponent<SpriteUtility>();
                //协程锁
               // Game.Scene.AddComponent<CoroutineLockComponent>();
                //操作码 协议号
                Game.Scene.AddComponent<OpcodeTypeComponent>();
                //消息分发
                Game.Scene.AddComponent<MessageDispatcherComponent>();

              

                //加载热更新代码
                Game.Hotfix.LoadHotfixAssembly();

                //测试热修复订阅事件
                // Game.EventSystem.Run(EventIdType.TestHotfixSubscribMonoEvent, "TestHotfixSubscribMonoEvent");

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        //每帧更新 热更dll的事件 与 游戏系统本身的事件
        private void Update()
        {     
            OneThreadSynchronizationContext.Instance.Update();
         
            //?.是否为空的判断 不为空则:热更新中函数不等于空就执行
            Game.Hotfix.Update?.Invoke();
         
            //事件系统
        
            Game.EventSystem?.Update();
          

            //#if UNITY_EDITOR
            ///游戏截图
            if (Input.GetKeyDown(KeyCode.J))
            {
                string pathfold = "../../游戏截图/";
                if (!Directory.Exists(pathfold))
                {
                    Directory.CreateDirectory(pathfold);
                }
                ScreenCapture.CaptureScreenshot($"../../游戏截图/{DateTime.Now:yyyy-MM-dd HH:mm:ss ffff}-游戏截图.png");
                Log.DebugBrown("截图完成");
            }
            //#endif
        }

        //固定更新 热更dll的事件 与 游戏系统本身的事件
        private void LateUpdate()
        {
            Game.Hotfix.LateUpdate?.Invoke();
            Game.EventSystem.LateUpdate();
        }

        public void FixedUpdate()
        {
            Game.Hotfix.FixedUpdate?.Invoke();
        }

        //游戏退出 热更dll的事件 与 游戏系统本身的事件
        private void OnApplicationQuit()
        {
            Game.Hotfix.OnApplicationQuit?.Invoke();
            Game.Close();
        }

      
    }
}