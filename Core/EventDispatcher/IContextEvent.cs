namespace Utils.Core.Events
{
    public interface IContextEvent : IEvent 
    {
        object Sender { get; }
    }
}