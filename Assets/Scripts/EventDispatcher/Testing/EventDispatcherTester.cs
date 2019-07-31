using UnityEngine;
using Utils.Core.Events;
using Utils.Core.Services;

public class EventDispatcherTester : MonoBehaviour
{
    private EventDispatcher eventDispatcher;

    private void Start()
    {
        eventDispatcher = ServiceLocator.Instance.Get<EventDispatcher>();
        eventDispatcher.Subscribe<TestEvent>(OnEventInvoked);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            eventDispatcher.Invoke(new TestEvent(20));
        }
    }

    private void OnEventInvoked(TestEvent testEvent)
    {
        Debug.Log("Invoked testEvent " + testEvent.testValue);
        eventDispatcher.Unsubscribe<TestEvent>(OnEventInvoked);
    }
}

