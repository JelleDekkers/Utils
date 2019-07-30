using UnityEngine;
using Utils.Core.Flow;
using Utils.Core.Events;

public class EventRule : Rule
{
    public override bool IsValid => isValid;
    private bool isValid;

#pragma warning disable CS0649
    [SerializeField] private IEvent eventObject;
#pragma warning restore CS0649
    private EventDispatcher eventDispatcher;

    public void InjectDependencies(EventDispatcher eventDispatcher)
    {
        this.eventDispatcher = eventDispatcher;
    }

    public override void OnActivate()
    {
        eventDispatcher.Subscribe(eventObject.GetType(), OnEventInvoked);
    }

    public override void OnDeactivate()
    {
        eventDispatcher.UnSubscribe(eventObject.GetType(), OnEventInvoked);
    }

    private void OnEventInvoked()
    {
        isValid = true;
    }
}
