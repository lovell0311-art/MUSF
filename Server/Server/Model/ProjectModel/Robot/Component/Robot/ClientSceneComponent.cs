using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class ClientSceneComponentAwakeSystem : AwakeSystem<ClientSceneComponent,Scene>
    {
        public override void Awake(ClientSceneComponent self, Scene clientScene)
        {
            self.ClientScene = clientScene;
        }
    }


    [ObjectSystem]
    public class ClientSceneComponentDestroySystem : DestroySystem<ClientSceneComponent>
    {
        public override void Destroy(ClientSceneComponent self)
        {
            self.ClientScene = null;
        }
    }


    public class ClientSceneComponent : Entity
    {
        public Scene ClientScene;
    }


    public static class ClientSceneComponentHelper
    {
        public static Scene ClientScene(this Entity self)
        {
            ClientSceneComponent clientSceneComponent = self.GetComponent<ClientSceneComponent>();
            if(clientSceneComponent == null)
            {
                return null;
            }
            return clientSceneComponent.ClientScene;
        }
    
    }


}
