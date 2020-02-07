using Utils.Core.Services;

namespace Utils.Core.Events
{
    /// <summary>
    /// Service for globally handling subscribing, unsubscribing and invoking events of type <see cref="IEvent"/>. 
    /// </summary>
    public class GlobalEventDispatcher : EventDispatcher, IService
    {
        public GlobalEventDispatcher(string name) : base(name)
        {

        }
    }
}