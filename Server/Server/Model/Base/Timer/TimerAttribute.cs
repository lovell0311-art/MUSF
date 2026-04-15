using System;
using ETModel;

namespace ETModel
{
    public class TimerAttribute: BaseAttribute
    {
        public int Type { get; }

        public TimerAttribute(int type)
        {
            this.Type = type;
        }
    }
}