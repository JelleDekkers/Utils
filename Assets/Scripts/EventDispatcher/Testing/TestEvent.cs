using Utils.Core.Events;

public class TestEvent : IEvent
{
    public float testValue;

    public TestEvent(float testValue)
    {
        this.testValue = testValue;
    }
}
