using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{
    public class AIDispatcherComponent : Entity
    {
        public static AIDispatcherComponent Instance;
        public Dictionary<string, AAIHandler> AIHandlers = new Dictionary<string, AAIHandler>();
    }
}
