using System;

namespace CustomFrameWork
{
    /// <summary>
    /// 事件
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventMethodAttribute : BaseAttribute
    {
        public bool IsByEventType { get; private set; }
        public EventSystemType EventSystemType { get; private set; }
        public string EventName { get; private set; }
        public Type Type { get; private set; }
        public EventMethodAttribute(Type b_CustomComponentType, EventSystemType b_EventSystemType)
        {
            this.Type = b_CustomComponentType;
            this.EventSystemType = b_EventSystemType;
            IsByEventType = true;
        }
        public EventMethodAttribute(string b_EventName)
        {
            this.EventName = b_EventName;
            IsByEventType = false;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BaseAttribute : Attribute
    {

    }
}
