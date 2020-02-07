using UnityEngine;
using Utils.Core.Events;

namespace Utils.Core.Flow.DefaultRules
{
    /// <summary>
    /// Rule that becomes valid when <see cref="eventType"/> is invoked by <see cref="eventDispatcher"/>
    /// </summary>
    public class EventRule : Rule
    {
        public override bool IsValid => isValid;
        private bool isValid;

        public override string DisplayName => (eventType != null && eventType.Type != null) ? "On " + eventType.Name : "No Event Picked!";

        [ClassTypeImplements(typeof(IEvent)), SerializeField] private ClassTypeReference eventType = null;
        [SerializeField] private bool globalEvent = false;

        private EventDispatcher eventDispatcher;

        public void InjectDependencies(EventDispatcher eventDispatcher, GlobalEventDispatcher globalEventDispatcher)
        {
            this.eventDispatcher = (globalEvent) ? globalEventDispatcher : eventDispatcher;
        }

        public override void OnActivate()
        {
            eventDispatcher.SubscribeToType(eventType, OnEventInvoked);
        }

        public override void OnDeactivate()
        {
            eventDispatcher.UnsubscribeToType(eventType, OnEventInvoked);
        }

        private void OnEventInvoked(IEvent eventObject)
        {
            isValid = true;
        }
    }
}