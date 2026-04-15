using CustomFrameWork;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class RobotDisconnectComponentStartSystem : StartSystem<RobotDisconnectComponent>
    {
        public override void Start(RobotDisconnectComponent self)
        {
            Session session = self.GetParent<Session>();
            self.ClientScene = session.ClientScene();
        }
    }


    [ObjectSystem]
    public class RobotDisconnectComponentDestroySystem : DestroySystem<RobotDisconnectComponent>
    {
        public override void Destroy(RobotDisconnectComponent self)
        {
            RobotManagerComponent.Instance.SceneDict.Remove(self.ClientScene.InstanceId);
            Log.Console($"[{self.ClientScene.Name}] 机器人下线 Robot.Count={RobotManagerComponent.Instance.SceneDict.Count}");
            self.ClientScene.Dispose();
            self.ClientScene = null;
        }

    }


    public class RobotDisconnectComponent : Entity
    {
        public Scene ClientScene;
    }
}
