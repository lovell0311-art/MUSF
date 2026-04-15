using System;

namespace ETModel
{
    public class SessionMessageDispatcherAttribute : BaseAttribute
    {
        public SessionMessageDispatcherType Type;

        public SessionMessageDispatcherAttribute(SessionMessageDispatcherType type)
        {
            this.Type = type;
        }
    }
}