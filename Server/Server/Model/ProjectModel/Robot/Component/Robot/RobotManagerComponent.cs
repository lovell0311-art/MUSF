using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ETModel.Robot
{
    [ObjectSystem]
    public class RobotManagerComponentAwakeSystem : AwakeSystem<RobotManagerComponent>
    {
        public override void Awake(RobotManagerComponent self)
        {
            RobotManagerComponent.Instance = self;
        }
    }

    [ObjectSystem]
    public class RobotManagerComponentDestroySystem : DestroySystem<RobotManagerComponent>
    {
        public override void Destroy(RobotManagerComponent self)
        {
            Scene[] sceneArray = self.SceneDict.Values.ToArray();
            foreach(Scene scene in sceneArray)
            {
                scene.Dispose();
            }
            self.SceneDict.Clear();
            self.LoginAccountQueue.Clear();
            RobotManagerComponent.Instance = null;
        }
    }



    public class RobotManagerComponent : Entity
    {
        public static RobotManagerComponent Instance;
        public readonly Dictionary<long, Scene> SceneDict = new Dictionary<long, Scene>();

        public readonly Queue<Robot_AccountConfig> LoginAccountQueue = new Queue<Robot_AccountConfig>();

        public void Enqueue(Robot_AccountConfig config)
        {
            if (IsDisposed) return;
            LoginAccountQueue.Enqueue(config);
        }
    }
}
