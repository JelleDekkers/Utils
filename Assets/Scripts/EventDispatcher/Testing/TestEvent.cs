using Utils.Core.Events;

class TestEvent : IEvent
{
    public float testValue;

    public TestEvent(float testValue)
    {
        this.testValue = testValue;
    }
}
