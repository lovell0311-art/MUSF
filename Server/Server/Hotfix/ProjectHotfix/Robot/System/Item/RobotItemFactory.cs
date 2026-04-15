using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;



namespace ETHotfix.Robot
{
    public static class RobotItemFactory
    {
        public static RobotItem Create(Scene clientScene,long uid,int configId)
        {
            ItemConfigManagerComponent itemConfigManager = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>();
            ItemConfig config = itemConfigManager.Get(configId);
            if (config == null)
            {
                Log.Error($"[{clientScene.Name}] 未知的物品配置:{configId}");
                return null;
            }
            RobotItem item = ComponentFactory.CreateWithId<RobotItem, ItemConfig, Scene>(uid, config, clientScene);
            item.AddComponent<ClientSceneComponent, Scene>(clientScene);
            item.AddComponent<NumericComponent>();

            return item;
        }
    }
}
