using UnityEngine;
using Utils.Core;
using Utils.Core.Flow;
using Utils.Core.Events;

/// <summary>
/// Rule that goes valid when <see cref="eventType"/> is invoked by <see cref="eventDispatcher"/>
/// </summary>
public class EventRule : Rule
{
    public override bool IsValid => isValid;
    private bool isValid;

    public override string DisplayName => (eventType != null && eventType.Type != null) ? "On " + eventType.ToString() : "No Event Picked!";

#pragma warning disable CS0649
    [ClassTypeImplements(typeof(IEvent)), SerializeField] private ClassTypeReference eventType;
#pragma warning restore CS0649
    private EventDispatcher eventDispatcher;

    public void InjectDependencies(EventDispatcher eventDispatcher)
    {
        this.eventDispatcher = eventDispatcher;
    }

    public override void Start()
    {
        eventDispatcher.SubscribeToType(eventType, OnEventInvoked);
    }

    public override void Stop()
    {
        eventDispatcher.UnsubscribeToType(eventType, OnEventInvoked);
    }

    private void OnEventInvoked(IEvent eventObject)
    {
        Debug.Log("onEvent recieved " + eventObject.GetType());
        isValid = true;
    }
}
