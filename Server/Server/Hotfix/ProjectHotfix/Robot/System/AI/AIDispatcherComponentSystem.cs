using System;
using System.Collections.Generic;
using System.Text;
using ETModel.Robot;
using ETModel;
using CustomFrameWork;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class AIDispatcherComponentAwakeSystem : AwakeSystem<AIDispatcherComponent>
    {
        public override void Awake(AIDispatcherComponent self)
        {
            AIDispatcherComponent.Instance = self;
            self.Load();
        }
    }


    [ObjectSystem]
    public class AIDispatcherComponentLoadSystem : LoadSystem<AIDispatcherComponent>
    {
        public override void Load(AIDispatcherComponent self)
        {
            self.Load();
        }

    }

    [ObjectSystem]
    public class AIDispatcherComponentDestroySystem : DestroySystem<AIDispatcherComponent>
    {
        public override void Destroy(AIDispatcherComponent self)
        {
            self.Load();
            AIDispatcherComponent.Instance = null;
        }
    }

    public static partial class AIDispatcherComponentSystem
    {
        public static void Load(this AIDispatcherComponent self)
        {
            var types = Game.EventSystem.GetTypes(typeof(AIHandlerAttribute));
            foreach (Type type in types)
            {
                AAIHandler aaiHandler = Activator.CreateInstance(type) as AAIHandler;
                if (aaiHandler == null)
                {
                    Log.Error($"robot ai is not AAIHandler: {type.Name}");
                    continue;
                }
                self.AIHandlers.Add(type.Name, aaiHandler);
            }
        }
    }
}
