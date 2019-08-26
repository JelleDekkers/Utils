namespace Utils.Core.Services
{
    /// <summary>
    /// Interface for constructing a new service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IServiceFactory<T> : IServiceFactory where T : IService
    {
        T Construct();
    }

    public interface IServiceFactory { }
}