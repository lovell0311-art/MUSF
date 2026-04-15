using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ETHotfix
{
    //里氏转换原则  来避免装箱拆箱
    public interface IEventInfo { }//空接口

    public class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> actions;

        public EventInfo(UnityAction<T> action)
        {
            actions += action;
        }
    }
    public class EventInfo<T,T2> : IEventInfo
    {
        public UnityAction<T, T2> actions;

        public EventInfo(UnityAction<T, T2> action)
        {
            actions += action;
        }
    }

    public class EventInfo<T, T2,T3> : IEventInfo
    {
        public UnityAction<T, T2, T3> actions;

        public EventInfo(UnityAction<T, T2, T3> action)
        {
            actions += action;
        }
    }

    public class EventInfo : IEventInfo
    {
        public UnityAction actions;

        public EventInfo(UnityAction action)
        {
            actions += action;
        }
    }

    public class EventCenter
    {
        /// <summary>
        /// key：事件的名字
        /// vslue: 对应的是监听该事件的委托方法(父类装子类)
        /// </summary>
        private readonly Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

        /// <summary>
        /// 监听事件(带泛型参数)
        /// </summary>
        /// <param name="eventName">事件的名字</param>
        /// <param name="action">用来处理该事件的方法</param>
        public void EventListenner<T>(string eventName, UnityAction<T> action)
        {
            //有没有对应的事件监听
            //有
            if (eventDic.ContainsKey(eventName))
            {
                //委托 一对多
                (eventDic[eventName] as EventInfo<T>).actions += action;
            }
            else//没有
            {
                eventDic[eventName] = new EventInfo<T>(action);
            }
        }
        public void EventListenner<T,T2>(string eventName, UnityAction<T,T2> action)
        {
            //有没有对应的事件监听
            //有
            if (eventDic.ContainsKey(eventName))
            {
                //委托 一对多
                (eventDic[eventName] as EventInfo<T, T2>).actions += action;
            }
            else//没有
            {
                eventDic[eventName] = new EventInfo<T, T2>(action);
            }
        }
        public void EventListenner<T, T2,T3>(string eventName, UnityAction<T, T2, T3> action)
        {
            //有没有对应的事件监听
            //有
            if (eventDic.ContainsKey(eventName))
            {
                //委托 一对多
                (eventDic[eventName] as EventInfo<T, T2, T3>).actions += action;
            }
            else//没有
            {
                eventDic[eventName] = new EventInfo<T, T2, T3>(action);
            }
        }
        /// <summary>
        /// 监听事件不带参数
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
        public void EventListenner(string eventName, UnityAction action)
        {
            //有没有对应的事件监听
            //有
            if (eventDic.ContainsKey(eventName))
            {
                //委托 一对多
                (eventDic[eventName] as EventInfo).actions += action;
            }
            else//没有
            {
                eventDic[eventName] = new EventInfo(action);
            }
        }

        /// <summary>
        /// 事件触发（带泛型参数）
        /// </summary>
        /// <param name="eventName">那个名字的事件触发了</param>
        public void EventTrigger<T>(string eventName, T info)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);//执行委托函数//执行委托函数
            }
        }
        public void EventTrigger<T, T2>(string eventName, T info,T2 info2)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo<T, T2>).actions?.Invoke(info,info2);//执行委托函数//执行委托函数
            }
        }
        public void EventTrigger<T, T2,T3>(string eventName, T info, T2 info2,T3 info3)
        {
            if (eventDic.ContainsKey(eventName))
            {
                // eventDic[eventName]?.Invoke(info);
                (eventDic[eventName] as EventInfo<T, T2, T3>).actions?.Invoke(info, info2,info3);//执行委托函数//执行委托函数
            }
        }
        /// <summary>
        /// 事件触发（不带泛型参数）
        /// </summary>
        /// <param name="eventName"></param>
        public void EventTrigger(string eventName)
        {
            if (eventDic.ContainsKey(eventName))
            {
                // eventDic[eventName]?.Invoke(info);
                (eventDic[eventName] as EventInfo).actions?.Invoke();//执行委托函数//执行委托函数
            }
        }

        /// <summary>
        /// 移除对应事件(事件有加就有减 不然会出问题)
        /// </summary>
        /// <param name="eventName">事件的名字</param>
        /// <param name="action">对应之间添加的委托函数</param>
        public void RemoveEvent<T>(string eventName, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo<T>).actions -= action;
            }
        }
        public void RemoveEvent<T,T2>(string eventName, UnityAction<T, T2> action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo<T, T2>).actions -= action;
            }
        }
        public void RemoveEvent<T, T2,T3>(string eventName, UnityAction<T, T2, T3> action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo<T, T2, T3>).actions -= action;
            }
        }
        /// <summary>
        /// 不带参数的
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
        public void RemoveEvent(string eventName, UnityAction action)
        {
            if (eventDic.ContainsKey(eventName))
            {
                (eventDic[eventName] as EventInfo).actions -= action;
            }
        }
    }
}
