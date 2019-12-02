using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Utils.Core.Injection;
using Utils.Core.Injection.UnityCallbacks;

namespace Utils.Core.Services
{
    /// <summary>
    /// Keeps track of global <see cref="IService"/>s. 
    /// When requesting a service, <see cref="InstantiatedServices"/> is checked. If no instance is found a new instance is created. 
    /// If the service has a corresponding <see cref="IServiceFactory"/>, the factory will construct the instance.
    /// </summary>
    public class GlobalServiceLocator
    {
        public static GlobalServiceLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GlobalServiceLocator();
                }
                return instance;
            }
        }
        private static GlobalServiceLocator instance;

        private Dictionary<Type, IService> InstantiatedServices = new Dictionary<Type, IService>();
        private Dictionary<Type, Type> serviceFactories = new Dictionary<Type, Type>();

		private UnityCallbackModule callbackModule;
		private DependencyInjector dependencyInjector;

        public GlobalServiceLocator()
        {
            serviceFactories = GetAllServiceFactories();

			callbackModule = new UnityCallbackModule(this);
			dependencyInjector = new DependencyInjector();
		}

        public T Get<T>() where T : IService
        {
            return (T)Get(typeof(T));
        }

        public IService Get(Type type)
        {
            if (InstantiatedServices.ContainsKey(type))
            {
                return InstantiatedServices[type];
            }
            else
            {
                return CreateNewServiceInstance(type);
            }
        }

		public bool Contains(Type type)
		{
			return InstantiatedServices.ContainsKey(type);
		}

		public void Remove<T>() where T : IService
		{
            Remove(typeof(T));
		}

		public void Remove(Type type)
        {
            if(!InstantiatedServices.ContainsKey(type))
            {
                Debug.LogWarningFormat("No service of type {0} found ", type);
            }

			callbackModule.Remove(InstantiatedServices[type]);
            InstantiatedServices.Remove(type);
        }

        private IService CreateNewServiceInstance(Type serviceType)
        {
            IService service;

            if (serviceFactories.ContainsKey(serviceType))
            {
                Type factoryType = serviceFactories[serviceType];
                service = CreateServiceFromFactory(factoryType);
            }
            else
            {
				if (serviceType.IsSubclassOf(typeof(MonoBehaviour)))
				{
					service = new GameObject().AddComponent(serviceType) as IService;
				}
				else
				{
					ConstructorInfo[] constructors = serviceType.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

					if (constructors.Length == 0)
					{
						service = (IService)Activator.CreateInstance(serviceType);
					}
					else
					{
						service = (IService)Activator.CreateInstance(serviceType, dependencyInjector.ResolveParameters(constructors[0], serviceType));
					}
				}
            }

            InstantiatedServices.Add(service.GetType(), service);
			callbackModule.Add(service);

            return service;
        }

        private IService CreateServiceFromFactory(Type factoryType)
        {
			ConstructorInfo[] constructors = factoryType.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            IServiceFactory factory = (IServiceFactory)Activator.CreateInstance(factoryType, dependencyInjector.ResolveParameters(constructors[0], factoryType));

            MethodInfo constructMethod = factoryType.GetMethod("Construct");
            return (IService)constructMethod.Invoke(factory, null);
        }

        private Dictionary<Type, Type> GetAllServiceFactories()
        {
            Dictionary<Type, Type> typeFactoryPair = new Dictionary<Type, Type>();

            IEnumerable<Type> factories = ReflectionUtility.GetAllTypes(typeof(IServiceFactory), true);

            foreach (Type factoryType in factories)
            {
                foreach (Type interfaceType in factoryType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IServiceFactory<>))
                    {
                        typeFactoryPair.Add(interfaceType.GetGenericArguments()[0], factoryType);
                        break;
                    }
                }
            }

            return typeFactoryPair;
        }
    }
}