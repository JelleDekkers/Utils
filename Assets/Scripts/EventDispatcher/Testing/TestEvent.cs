using Utils.Core.Services;

class TestEvent : IEvent
{
    public float testValue;

    public TestEvent(float testValue)
    {
        this.testValue = testValue;
    }
}
