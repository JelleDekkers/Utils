using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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

        public GlobalServiceLocator()
        {
            serviceFactories = GetAllServiceFactories();
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

        public void Remove<T>() where T : IService
        {
            Type serviceType = typeof(T);
            if(!InstantiatedServices.ContainsKey(serviceType))
            {
                Debug.LogWarningFormat("No service of type {0} found ", serviceType);
            }

            InstantiatedServices.Remove(serviceType);
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
                    service = (IService)Activator.CreateInstance(serviceType);
                }
            }

            InstantiatedServices.Add(service.GetType(), service);

            return service;
        }

        private IService CreateServiceFromFactory(Type factoryType)
        {
            IServiceFactory factory = (IServiceFactory)Activator.CreateInstance(factoryType);

            MethodInfo constructMethod = factoryType.GetMethod("Construct");
            return (IService)constructMethod.Invoke(factory, null);
        }

        private Dictionary<Type, Type> GetAllServiceFactories()
        {
            Dictionary<Type, Type> typeFactoryPair = new Dictionary<Type, Type>();

            IEnumerable<Type> factories = ReflectionUtility.GetAllTypes(typeof(IServiceFactory));

            foreach (var factoryType in factories)
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