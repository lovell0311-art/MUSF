using System;
using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;

namespace ETHotfix.Robot
{
    [Timer(TimerType.AITimer)]
    public class AITimer : ATimer<AIComponent>
    {
        public override void Run(AIComponent self)
        {
            try
            {
                self.Check();
            }
            catch (Exception e)
            {
                Log.Error($"move timer error: {self.Id}\n{e}");
            }
        }
    }

    [ObjectSystem]
    public class AIComponentAwakeSystem : AwakeSystem<AIComponent, int>
    {
        public override void Awake(AIComponent self, int aiConfigId)
        {
            self.AIConfigId = aiConfigId;
            self.LastNodeName = "Empty";
            self.Timer = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(AIComponent.FrameTime, TimerType.AITimer, self);
        }
    }

    [ObjectSystem]
    public class AIComponentDestroySystem : DestroySystem<AIComponent>
    {
        public override void Destroy(AIComponent self)
        {
            ETModel.ET.TimerComponent.Instance?.Remove(ref self.Timer);
            self.CancellationToken?.Cancel();
            self.CancellationToken = null;
            self.Current = 0;
            self.LastNodeName = "Empty";
        }
    }


    public static partial class AIComponentSystem
    {
        public static void Check(this AIComponent self)
        {
            if (self.Parent == null)
            {
                ETModel.ET.TimerComponent.Instance.Remove(ref self.Timer);
                return;
            }

            var oneAI = AIConfigManager.Instance.AIConfigs[self.AIConfigId];

            foreach (AI_Config aiConfig in oneAI.Values)
            {

                AIDispatcherComponent.Instance.AIHandlers.TryGetValue(aiConfig.Name, out AAIHandler aaiHandler);

                if (aaiHandler == null)
                {
                    Log.Error($"not found aihandler: {aiConfig.Name}");
                    continue;
                }

                int ret = aaiHandler.Check(self, aiConfig);
                if (ret != 0)
                {
                    continue;
                }

                if (self.Current == aiConfig.Id)
                {
                    break;
                }
                Scene clientScene = self.ClientScene();
                Log.Debug($"[{clientScene.Name}] #AI# {self.LastNodeName} => {aiConfig.Name}");
                self.Cancel(); // 取消之前的行为
                ETCancellationToken cancellationToken = new ETCancellationToken();
                self.CancellationToken = cancellationToken;
                self.Current = aiConfig.Id;

                self.LastNodeName = aiConfig.Name;

                async Task Execute()
                {
                    try
                    {
                        await aaiHandler.Execute(self, aiConfig, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"[{clientScene.Name}] {e}");
                    }
                    finally
                    {
                        self.LastNodeName = "Empty";
                        if (!cancellationToken.IsCancel())
                        {
                            Log.Debug($"[{clientScene.Name}] #AI# {aiConfig.Name} => Empty");
                        }
                        self.Current = 0;
                    }
                }

                Execute().Coroutine();

                return;
            }

        }

        private static void Cancel(this AIComponent self)
        {
            self.CancellationToken?.Cancel();
            self.Current = 0;
            self.CancellationToken = null;
        }
    }
}
