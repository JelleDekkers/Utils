using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Utils.Core.Services;

namespace Utils.Core.Injection
{
    /// <summary>
    /// Class for handling dependency injection. 
    /// Dependencies need to be of type <see cref="IService>"/> and are resolved by <see cref="GlobalServiceLocator"/>
    /// </summary>
    public class DependencyInjector
    {
        public const string DEFAULT_INJECTION_METHOD_NAME = "InjectDependencies";

        private readonly string injectionMethodName;
        private Dictionary<Type, object> typeInstancePairs = new Dictionary<Type, object>();

        public DependencyInjector(string injectionMethodName = DEFAULT_INJECTION_METHOD_NAME)
        {
            this.injectionMethodName = injectionMethodName;

            RegisterInstance<DependencyInjector>(this);
        }

        public void RegisterInstance<T>(object instance)
        {
            RegisterInstance(typeof(T), instance);
        }

        public void RegisterInstance(Type type, object instance)
        {
            if(typeInstancePairs.ContainsKey(type))
            {
                typeInstancePairs.Remove(type);
            }

            typeInstancePairs.Add(type, instance);
        }

        public void UnRegisterInstance<T>()
        {
            UnRegisterInstance(typeof(T));
        }

        public void UnRegisterInstance(Type type)
        {
            if(typeInstancePairs.ContainsKey(type))
            {
                typeInstancePairs.Remove(type);
            }
        }

        public void InjectMethod(object obj)
        {
            InjectMethod(obj, injectionMethodName);
        }

        public void InjectMethod(object obj, string methodName)
        {
            Type type = obj.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                if (method.Name == methodName)
                {
                    Inject(method, obj);
                    return;
                }
            }
        }

        public void InjectGameObject(GameObject go)
        {
            MonoBehaviour[] monoBehaviours = go.GetComponentsInChildren<MonoBehaviour>();
            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                InjectMethod(monoBehaviours[i]);
            }
        }

        private object Inject(MethodBase method, object obj)
        {
            return method.Invoke(obj, ResolveParameters(method, obj.GetType()));
        }

        public object[] ResolveParameters(MethodBase info, Type objType)
        {
            ParameterInfo[] parameters = info.GetParameters();
            object[] objects = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameters[i];
                Type type = parameters[i].ParameterType;

                if (type.GetInterfaces().Contains(typeof(IService)))
                {
                    objects[i] = GlobalServiceLocator.Instance.Get(type);
                    continue;
                }

                if(typeInstancePairs.ContainsKey(type))
                {
                    objects[i] = typeInstancePairs[type];
                    continue;
                }

                objects[i] = null;
            }

            return objects;
        }
    }
}