using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;


namespace ETModel.Robot
{
    [ObjectSystem]
    public class AIConfigManagerAwakeSystem : AwakeSystem<AIConfigManager>
    {
        public override void Awake(AIConfigManager self)
        {
            AIConfigManager.Instance = self;

            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var configJson = readConfig.GetJson<AI_ConfigJson>().JsonDic;


            foreach (var kv in configJson)
            {
                SortedDictionary<int, AI_Config> aiNodeConfig;
                if (!self.AIConfigs.TryGetValue(kv.Value.AIConfigId, out aiNodeConfig))
                {
                    aiNodeConfig = new SortedDictionary<int, AI_Config>();
                    self.AIConfigs.Add(kv.Value.AIConfigId, aiNodeConfig);
                }
                aiNodeConfig.Add(kv.Value.Order, kv.Value);
            }
        }
    }

    [ObjectSystem]
    public class AIConfigManagerDestroySystem : DestroySystem<AIConfigManager>
    {
        public override void Destroy(AIConfigManager self)
        {
            self.AIConfigs.Clear();
            AIConfigManager.Instance = null;
        }
    }



    public class AIConfigManager : Entity
    {
        public static AIConfigManager Instance;

        public Dictionary<int, SortedDictionary<int, AI_Config>> AIConfigs = new Dictionary<int, SortedDictionary<int, AI_Config>>();
    }
}
