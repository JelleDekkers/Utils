using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Utils.Core.Services;

namespace Utils.Core.Injection
{
    public class DependencyInjector
    {
        private readonly string injectionMethodName;

        public DependencyInjector(string injectionMethodName = "InjectDependencies")
        {
            this.injectionMethodName = injectionMethodName;
        }

        public void InjectConstructor(object obj, int constructorIndex = 0)
        {
            Type type = obj.GetType();
            ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Inject(constructors[constructorIndex], obj);
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

        private object[] ResolveParameters(MethodBase info, Type objType)
        {
            ParameterInfo[] parameters = info.GetParameters();
            object[] objects = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameters[i];
                Type type = parameters[i].ParameterType;

                if(type == typeof(DependencyInjector))
                {
                    objects[i] = this;
                    continue;
                }

                // Currently only dependencies of type IService work
                if (!type.GetInterfaces().Contains(typeof(IService)))
                {
                    objects[i] = null;
                    continue;
                }

                objects[i] = ServiceLocator.Instance.Get(type);
            }

            return objects;
        }
    }
}