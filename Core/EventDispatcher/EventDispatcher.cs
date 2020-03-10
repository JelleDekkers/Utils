using System;
using System.Collections.Generic;

namespace Utils.Core.Events
{
    /// <summary>
    /// Class for handling subscribing, unsubscribing and invoking events of type <see cref="IEvent"/>. 
    /// </summary>
    public class EventDispatcher
    {
        /// <summary>
        /// Name used for debugging purposes
        /// </summary>
        public readonly string Name;

        private readonly Dictionary<Type, Action<object>> eventCallbackMap = new Dictionary<Type, Action<object>>();
        private readonly Dictionary<object, Action<object>> anonymousDelegateLookup = new Dictionary<object, Action<object>>();
        private readonly List<Action<object>> subscribeToAnyList = new List<Action<object>>();

        public EventDispatcher(string name)
        {
            Name = name;
        }

        public void Subscribe<T>(Action<T> callback) where T : IEvent
        {
            Type type = typeof(T);
            if (!eventCallbackMap.ContainsKey(type))
            {
                eventCallbackMap.Add(type, delegate { });
            }

            // To store the callback into the dictionary it has to be converted to an anonymous delegate of type Action<object> 
            // This also prevents having to use DynamicInvoke in Invoke() which is slow
            Action<object> func = obj => callback((T)obj);
            eventCallbackMap[type] += func;

            // Store the anonymous delegate in a lookup, to be able to unsubscribe it later 
            if (anonymousDelegateLookup.ContainsKey(callback))
            {
                anonymousDelegateLookup[callback] = func;
            }
            else
            {
                anonymousDelegateLookup.Add(callback, func);
            }
        }

        public void Unsubscribe<T>(Action<T> callback) where T : IEvent
        {
            Type type = typeof(T);
            if (eventCallbackMap.ContainsKey(type) && anonymousDelegateLookup.ContainsKey(callback))
            {
                eventCallbackMap[type] -= anonymousDelegateLookup[callback];
                anonymousDelegateLookup.Remove(callback);
                if (eventCallbackMap[type].GetInvocationList().Length == 1)
                {
                    eventCallbackMap.Remove(type);
                }
            }
        }

        public void SubscribeToType(Type type, Action<IEvent> callback)
        {
            if (!eventCallbackMap.ContainsKey(type))
            {
                eventCallbackMap.Add(type, delegate { });
            }

            // To store the callback into the dictionary it has to be converted to an anonymous delegate of type Action<object> 
            // This also prevents having to use DynamicInvoke in Invoke() which is slow
            Action<object> func = obj => callback((IEvent)obj);
            eventCallbackMap[type] += func;

            // Store the anonymous delegate in a lookup, to be able to unsubscribe it later 
            if (anonymousDelegateLookup.ContainsKey(callback))
            {
                anonymousDelegateLookup[callback] = func;
            }
            else
            {
                anonymousDelegateLookup.Add(callback, func);
            }
        }

        public void UnsubscribeToType(Type type, Action<IEvent> callback)
        {
            if (eventCallbackMap.ContainsKey(type) && anonymousDelegateLookup.ContainsKey(callback))
            {
                eventCallbackMap[type] -= anonymousDelegateLookup[callback];
                anonymousDelegateLookup.Remove(callback);
                if (eventCallbackMap[type].GetInvocationList().Length == 1)
                {
                    eventCallbackMap.Remove(type);
                }
            }
        }

        public void SubscribeToAnyEvent(Action<object> callback)
        {
            subscribeToAnyList.Add(callback);
        }

        public void UnsubscribeToAnyEvent(Action<object> callback)
        {
            subscribeToAnyList.Remove(callback);
        }

        public void Invoke<T>(T eventObject) where T : IEvent
        {
            Type type = eventObject.GetType();
            Invoke(type, eventObject);
        }

        public void Invoke(Type type, IEvent eventObject)
        {
            Action<object> callback;

            if (eventCallbackMap.TryGetValue(type, out callback))
            {
                callback.Invoke(eventObject);
            }

            for (int i = 0; i < subscribeToAnyList.Count; i++)
            {
                callback = subscribeToAnyList[i];
                callback.Invoke(eventObject);
            }
        }

        public override string ToString()
        {
            return "EventDispatcher " + Name;
        }
    }
}