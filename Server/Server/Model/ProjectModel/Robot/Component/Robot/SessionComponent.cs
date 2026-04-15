using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    public class SessionComponentAwakeSystem : AwakeSystem<SessionComponent>
    {
        public override void Awake(SessionComponent self)
        {
            self.session = null;
        }
    }

    public class SessionComponentDestroySystem : DestroySystem<SessionComponent>
    {
        public override void Destroy(SessionComponent self)
        {
            if(self.session != null)
            {
                self.session.Dispose();
                self.session = null;
            }
        }

    }


    public class SessionComponent : Entity
    {
        public Session session;
    }
}
