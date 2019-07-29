﻿namespace Utils.ServiceSystem
{
    /// <summary>
    /// Interface for constructing a new service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IServiceFactory<T> : IServiceFactory where T : IService
    {
        T Construct();// (IDependencyInjector serviceLocator);
    }

    public interface IServiceFactory { }
}