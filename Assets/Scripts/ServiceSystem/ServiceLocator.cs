using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utils.Core.Services
{
    public class ServiceLocator
    {
        public static ServiceLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceLocator();
                }
                return instance;
            }
        }
        private static ServiceLocator instance;

        private Dictionary<Type, IService> InstantiatedServices = new Dictionary<Type, IService>();
        private Dictionary<Type, Type> serviceFactories = new Dictionary<Type, Type>();

        public ServiceLocator()
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
                service = (IService)Activator.CreateInstance(serviceType);
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