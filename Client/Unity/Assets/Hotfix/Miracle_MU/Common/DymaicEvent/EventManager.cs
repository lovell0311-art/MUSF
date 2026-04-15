using System;
using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// ETHotfix层事件管理
    /// </summary>
    public class EventManager
    {

        public Dictionary<string, Delegate> m_eventTable = new Dictionary<string, Delegate>();
        #region Add event
        public void AddEventHandler(string eventType, Action handler)
        {
            try
            {
                if (OnHandlerAdding(eventType, handler))
                {
                    m_eventTable[eventType] = (Action)Delegate.Combine((Action)m_eventTable[eventType], handler);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Error($"Attempting to add listener with inconsistent signature for event type {eventType}. Current listeners have type {m_eventTable[eventType].GetType().Name} and listener being added has type {handler.GetType().Name}");
            }
        }
        public void AddEventHandler<T1>(string eventType, Action<T1> handler)
        {
            try
            {
                if (this.OnHandlerAdding(eventType, handler))
                {
                    this.m_eventTable[eventType] = (Action<T1>)Delegate.Combine((Action<T1>)this.m_eventTable[eventType], handler);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Error($"Attempting to add listener with inconsistent signature for event type {eventType}. Current listeners have type {m_eventTable[eventType].GetType().Name} and listener being added has type {handler.GetType().Name}");
            }
        }

   
        public void AddEventHandler<T1, T2>(string eventType, Action<T1, T2> handler)
        {
            try
            {
                if (this.OnHandlerAdding(eventType, handler))
                {
                    this.m_eventTable[eventType] = (Action<T1, T2>)Delegate.Combine((Action<T1, T2>)this.m_eventTable[eventType], handler);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Attempting to add listener with inconsistent signature for event type {eventType}. Current listeners have type {m_eventTable[eventType].GetType().Name} and listener being added has type {handler.GetType().Name}");
                throw e;
            }
        }
        public void AddEventHandler<T1, T2, T3>(string eventType, Action<T1, T2, T3> handler)
        {
            try
            {
                if (this.OnHandlerAdding(eventType, handler))
                {
                    this.m_eventTable[eventType] = (Action<T1, T2, T3>)Delegate.Combine((Action<T1, T2, T3>)this.m_eventTable[eventType], handler);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Attempting to add listener with inconsistent signature for event type {eventType}. Current listeners have type {m_eventTable[eventType].GetType().Name} and listener being added has type {handler.GetType().Name}");
                throw e;
            }
        }
        public void AddEventHandler<T1, T2, T3, T4>(string eventType, Action<T1, T2, T3, T4> handler)
        {
            try
            {
                if (this.OnHandlerAdding(eventType, handler))
                {
                    this.m_eventTable[eventType] = (Action<T1, T2, T3, T4>)Delegate.Combine((Action<T1, T2, T3, T4>)this.m_eventTable[eventType], handler);
                }
            } 
            catch (Exception e)
            {
                Log.Error($"Attempting to add listener with inconsistent signature for event type {eventType}. Current listeners have type {m_eventTable[eventType].GetType().Name} and listener being added has type {handler.GetType().Name}");
                throw e;
            }
        }
        public void AddEventHandler<T1, T2, T3, T4, T5>(string eventType, Action<T1, T2, T3, T4, T5> handler)
        {
            try
            {
                if (this.OnHandlerAdding(eventType, handler))
                {
                    this.m_eventTable[eventType] = (Action<T1, T2, T3, T4, T5>)Delegate.Combine((Action<T1, T2, T3, T4, T5>)this.m_eventTable[eventType], handler);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Attempting to add listener with inconsistent signature for event type {eventType}. Current listeners have type {m_eventTable[eventType].GetType().Name} and listener being added has type {handler.GetType().Name}");
                throw e;
            }
        }
        #endregion

        #region Remove event
        public void RemoveEventHandler(string eventType, Action handler)
        {
            if (this.OnHandlerRemoving(eventType, handler))
            {
                this.m_eventTable[eventType] = (Action)Delegate.Remove((Action)this.m_eventTable[eventType], handler);
            }
        }
        public void RemoveEventHandler<T1>(string eventType, Action<T1> handler)
        {
            if (this.OnHandlerRemoving(eventType, handler))
            {
                this.m_eventTable[eventType] = (Action<T1>)Delegate.Remove((Action<T1>)this.m_eventTable[eventType], handler);
            }
           
        }
        public void RemoveEventHandler<T1, T2>(string eventType, Action<T1, T2> handler)
        {
            if (this.OnHandlerRemoving(eventType, handler))
            {
                this.m_eventTable[eventType] = (Action<T1, T2>)Delegate.Remove((Action<T1, T2>)this.m_eventTable[eventType], handler);
            }
        }
        public void RemoveEventHandler<T1, T2, T3>(string eventType, Action<T1, T2, T3> handler)
        {
            if (this.OnHandlerRemoving(eventType, handler))
            {
                this.m_eventTable[eventType] = (Action<T1, T2, T3>)Delegate.Remove((Action<T1, T2, T3>)this.m_eventTable[eventType], handler);
            }
        }
        public void RemoveEventHandler<T1, T2, T3, T4>(string eventType, Action<T1, T2, T3, T4> handler)
        {
            if (this.OnHandlerRemoving(eventType, handler))
            {
                this.m_eventTable[eventType] = (Action<T1, T2, T3, T4>)Delegate.Remove((Action<T1, T2, T3, T4>)this.m_eventTable[eventType], handler);
            }
        }
        #endregion

        #region Broadcast
        public void BroadcastEvent(string eventType)
        {
            if (OnBroadCasting(eventType))
            {
                Action action = m_eventTable[eventType] as Action;
                if (action != null)
                {
                    action();
                }
            }
        }
        public void BroadCastEvent<T1>(string eventType, T1 arg1)
        {
            if (this.OnBroadCasting(eventType))
            {
                Action<T1> action = this.m_eventTable[eventType] as Action<T1>;
                if (action != null)
                {
                    action(arg1);
                }
            }
        }
        public void BroadCastEvent<T1, T2>(string eventType, T1 arg1, T2 arg2)
        {
            if (this.OnBroadCasting(eventType))
            {
                Action<T1, T2> action = this.m_eventTable[eventType] as Action<T1, T2>;
                if (action != null)
                {
                    action(arg1, arg2);
                }
            }
        }
        public void BroadCastEvent<T1, T2, T3>(string eventType, T1 arg1, T2 arg2, T3 arg3)
        {
            if (this.OnBroadCasting(eventType))
            {
                Action<T1, T2, T3> action = this.m_eventTable[eventType] as Action<T1, T2, T3>;
                if (action != null)
                {
                    action(arg1, arg2, arg3);
                }
            }
        }
        public void BroadCastEvent<T1, T2, T3, T4>(string eventType, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (this.OnBroadCasting(eventType))
            {
                Action<T1, T2, T3, T4> action = this.m_eventTable[eventType] as Action<T1, T2, T3, T4>;
                if (action != null)
                {
                    action(arg1, arg2, arg3, arg4);
                }
            }
        }
        #endregion

        private bool OnHandlerAdding(string eventType, Delegate handler)
        {
            bool result = true;
            if (!m_eventTable.ContainsKey(eventType))
            {
                m_eventTable.Add(eventType, null);
            }
#if UNITY_IOS
            result = true;
#else
            Delegate @delegate = this.m_eventTable[eventType]; 
            if (@delegate != null && @delegate.GetType() != handler.GetType())
            {

                result = false;
            }
#endif
            return result;

        }
        private bool OnHandlerRemoving(string eventType, Delegate handler)
        {
            bool result = true;
            if (this.m_eventTable.ContainsKey(eventType))
            {
#if UNITY_IOS
                result = true;
#else
                Delegate @delegate = this.m_eventTable[eventType];
                if (@delegate != null)
                {
                    if (@delegate.GetType() != handler.GetType())
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
#endif
            }
            else
            {
                result = false;
            }
            return result;
        }
        private bool OnBroadCasting(string eventType)
        {
            return this.m_eventTable.ContainsKey(eventType);
        }
    }
}
