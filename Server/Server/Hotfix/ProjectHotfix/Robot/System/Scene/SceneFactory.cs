using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.Robot;

namespace ETHotfix.Robot
{
    public static class SceneFactory
    {
        public static Scene CreateClientScene(Entity parent)
        {
            Scene clientScene = ComponentFactory.CreateWithParent<Scene>(parent);
            clientScene.Name = "";
            clientScene.AddComponent<NetOuterComponent>();
            clientScene.AddComponent<ObjectWait>();
            clientScene.AddComponent<AccountInfoComponent>();
            clientScene.AddComponent<RoleInfoComponent>();
            clientScene.AddComponent<PlayerComponent>();
            clientScene.AddComponent<SessionComponent>();
            clientScene.AddComponent<RobotMapComponent>();
            clientScene.AddComponent<RobotItemManager>();
            clientScene.AddComponent<RobotPetsWindowsComponent>();





            return clientScene;
        }
    }
}
