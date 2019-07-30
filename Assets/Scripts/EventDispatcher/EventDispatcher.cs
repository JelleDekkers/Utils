using System;
using System.Collections.Generic;
using Utils.Core.Services;

namespace Utils.Core.Events
{
    public class EventDispatcher : IService
    {
        private readonly Dictionary<Type, Action<object>> eventCallbackMap = new Dictionary<Type, Action<object>>();
        private readonly Dictionary<object, Action<object>> anonymousDelegateLookup = new Dictionary<object, Action<object>>();
        private readonly List<Action<object>> subscribeToAnyList = new List<Action<object>>();

        public void SubscribeToAnyEvent(Action<object> callback)
        {
            subscribeToAnyList.Add(callback);
        }

        public void UnsubscribeToAnyEvent(Action<object> callback)
        {
            subscribeToAnyList.Remove(callback);
        }

        public void Subscribe<T>(Action<T> callback) where T : IEvent
        {
            Type type = typeof(T);
            if (!eventCallbackMap.ContainsKey(type))
            {
                eventCallbackMap.Add(type, delegate { });
            }

            // To store the callback into the dictionary it has to be converted as an anonymous delegate of type Action<object> 
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

        public void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            Type type = typeof(T);
            if (eventCallbackMap.ContainsKey(type) && anonymousDelegateLookup.ContainsKey(handler))
            {
                eventCallbackMap[type] -= anonymousDelegateLookup[handler];
                anonymousDelegateLookup.Remove(handler);
                if (eventCallbackMap[type].GetInvocationList().Length == 1)
                {
                    eventCallbackMap.Remove(type);
                }
            }
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
    }
}