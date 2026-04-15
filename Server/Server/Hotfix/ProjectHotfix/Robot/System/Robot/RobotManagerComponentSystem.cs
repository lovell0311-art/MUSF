using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class RobotManagerComponentStartSystem : StartSystem<RobotManagerComponent>
    {
        public override void Start(RobotManagerComponent self)
        {
            self.Start().Coroutine();
        }
    }

    public static partial class RobotManagerComponentSystem
    {

        public static async Task Start(this RobotManagerComponent self)
        {
//             await ETModel.ET.TimerComponent.Instance.WaitAsync(1000);
//             Scene robot = await self.NewRobot("10000010001", "12345678", 1,1);
//             return;
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            List<Robot_AccountConfig> allAccountInfo = readConfig.GetJson<Robot_AccountConfigJson>().JsonDic.Values.ToList();
            await ETModel.ET.TimerComponent.Instance.WaitAsync(1000);
            int id = 0;
            int.TryParse(OptionComponent.Options.AppendValue,out id);

            foreach (Robot_AccountConfig config in allAccountInfo)
            {
                if((config.Id / 100) == id)
                {
                    self.Enqueue(config);
                }
            }
            self.StartLogin().Coroutine();
        }

        public static async Task StartLogin(this RobotManagerComponent self)
        {
            long instanceId = self.InstanceId;
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.RobotLogin, instanceId))
            {
                if (instanceId != self.InstanceId) return;
                using ListComponent<Task<bool>> tasks = ListComponent<Task<bool>>.Create();
                while (self.LoginAccountQueue.Count > 0)
                {
                    Robot_AccountConfig config = self.LoginAccountQueue.Dequeue();
                    async Task<bool> LoginRobotAsync()
                    {
                        Scene robot = await self.NewRobot(config.Phone, config.Passwd, config.ZoneId, config.LineId);
                        if (robot != null)
                        {
                            robot.AddComponent<RobotReconnectComponent, Robot_AccountConfig>(config);
                        }
                        else
                        {
                            self.AddToLoginList(config).Coroutine();
                        }
                        return true;
                    }

                    tasks.Add(LoginRobotAsync());
                    if (tasks.Count >= 10)
                    {
                        await TaskHelper.WaitAll(tasks);
                        tasks.Clear();
                    }
                }
                if (tasks.Count != 0)
                {
                    await TaskHelper.WaitAll(tasks);
                    tasks.Clear();
                }
            }
        }

        public static async Task<Scene> NewRobot(this RobotManagerComponent self,string phone,string password,int zoneId,int lineId)
        {
            Scene clientScene = SceneFactory.CreateClientScene(self);
            try
            {
                if ((await LoginHelper.LoginAsync(clientScene, ConstValue.LoginAddress, phone, password)) == false)
                {
                    if (await LoginHelper.RegisterAsync(clientScene, ConstValue.LoginAddress, phone, password) == false)
                    {
                        clientScene.Dispose();
                        Log.Warning($"注册账号失败！phone={phone}");
                        return null;
                    }
                    if ((await LoginHelper.LoginAsync(clientScene, ConstValue.LoginAddress, phone, password)) == false)
                    {
                        clientScene.Dispose();
                        Log.Warning($"登录账号失败！phone={phone}");
                        return null;
                    }
                }
            }
            catch(Exception e)
            {
                Log.Warning($"登录过程发生错误:{e.Message}");
                clientScene.Dispose();
                return null;
            }
            try
            {
                if ((await LoginHelper.EnterGameOrCreateAsync(clientScene, zoneId, lineId)) == false)
                {
                    clientScene.Dispose();
                    Log.Warning($"进入游戏失败！phone={phone}");
                    return null;
                }
                if ((await LoginHelper.InitComponent(clientScene)) == false)
                {
                    clientScene.Dispose();
                    Log.Warning($"初始化组件失败！phone={phone}");
                    return null;
                }
            }
            catch(Exception e)
            {
                Log.Warning($"登录Game过程发生错误:{e.Message}");
                clientScene.Dispose();
                return null;
            }
            clientScene.AddComponent<AIComponent, int>(2).AddComponent<ClientSceneComponent, Scene>(clientScene);
            self.SceneDict.Add(clientScene.InstanceId, clientScene);
            Log.Console($"[{clientScene.Name}] 进入游戏成功 Robot.Count={self.SceneDict.Count}");
            return clientScene;
        }

        public static async Task AddToLoginList(this RobotManagerComponent self, Robot_AccountConfig config)
        {
            if (Help_RandomHelper.Range(0, 2) == 0)
            {
                Log.Console($"添加到重连列表 phone={config.Phone}");
                RobotManagerComponent.Instance.Enqueue(config);
                self.StartLogin().Coroutine();
            }
            else
            {
                await ETModel.ET.TimerComponent.Instance.WaitAsync(Help_RandomHelper.Range(0, 1000 * 60 * 5));
                Log.Console($"添加到重连列表 phone={config.Phone}");
                RobotManagerComponent.Instance.Enqueue(config);
                self.StartLogin().Coroutine();
            }
        }
    }
}
