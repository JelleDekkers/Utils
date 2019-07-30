using UnityEngine;
using Utils.Core.Events;
using Utils.Core.Services;

public class EventDispatcherTester : MonoBehaviour
{
    private EventDispatcher eventDispatcher;

    private void Start()
    {
        eventDispatcher = ServiceLocator.Instance.Get<EventDispatcher>();
        eventDispatcher.Subscribe<TestEvent>(OnTestEventInvoked);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            eventDispatcher.Invoke(new TestEvent(5));
        }
    }

    private void OnTestEventInvoked(TestEvent testEvent)
    {
        Debug.Log("Invoked testEvent " + testEvent.testValue);
        eventDispatcher.UnSubscribe<TestEvent>(OnTestEventInvoked);
    }
}

