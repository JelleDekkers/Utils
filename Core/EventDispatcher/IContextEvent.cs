namespace Utils.Core.Events
{
    public interface IContextEvent : IEvent 
    {
        public object Sender { get; }
    }
}