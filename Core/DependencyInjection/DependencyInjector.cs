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
    /// Dependencies of type <see cref="IService>"/> are resolved by <see cref="GlobalServiceLocator"/> automatically, 
	/// otherwise an instance has to be registered using RegisterInstance()
    /// </summary>
    public class DependencyInjector : ICloneable
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

				if(type.IsPrimitive || type == typeof(string))
				{
					continue;
				}

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

        /// <summary>
        /// Spawns and injects a new gameobject. 
        /// Use this function to ensure the injection happens before Awake, Start and OnEnable functions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public T InstantiateGameObject<T>(T prefab) where T : MonoBehaviour
        {
            bool wasActive = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);
            T instance = UnityEngine.Object.Instantiate(prefab);

            InjectGameObject(instance.gameObject);

            prefab.gameObject.SetActive(wasActive);
            instance.gameObject.SetActive(wasActive);

            return instance;
        }

        /// <summary>
        /// Spawns and injects a new gameobject. 
        /// Use this function to ensure the injection happens before Awake, Start and OnEnable functions.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject InstantiateGameObject(GameObject prefab) 
        {
            bool wasActive = prefab.gameObject.activeSelf;
            prefab.gameObject.SetActive(false);
            GameObject instance = UnityEngine.Object.Instantiate(prefab);

            InjectGameObject(instance.gameObject);

            prefab.gameObject.SetActive(wasActive);
            instance.gameObject.SetActive(wasActive);

            return instance;
        }

        public object Clone()
		{
			return MemberwiseClone();
		}
	}
}