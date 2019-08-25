using NUnit.Framework;
using System;

namespace Utils.Core.Events.Testing
{
    [TestFixture]
    public class EventDispatcherUnitTests
    {
        public class TestEvent : IEvent
        {
        }

        public class SecondaryTestEvent : IEvent
        {
        }

        [Test]
        public void Subscribe_To_Event()
        {
            EventDispatcher eventDispatcher = new EventDispatcher();
            bool received = false;
            Action<TestEvent> callback = (e) => received = true;
            eventDispatcher.Subscribe(callback);

            eventDispatcher.Invoke(new SecondaryTestEvent());
            Assert.IsFalse(received);

            eventDispatcher.Invoke(new TestEvent());
            Assert.IsTrue(received);
        }

        [Test]
        public void Unsubscribe_From_Event()
        {
            EventDispatcher eventDispatcher = new EventDispatcher();
            bool received = false;
            Action<TestEvent> callback = (e) => received = true;

            eventDispatcher.Subscribe(callback);
            eventDispatcher.Invoke(new TestEvent());
            Assert.IsTrue(received);

            eventDispatcher.Unsubscribe(callback);
            received = false;
            eventDispatcher.Invoke(new TestEvent());
            Assert.IsFalse(received);
        }

        [Test]
        public void Subscribe_To_Type()
        {
            EventDispatcher eventDispatcher = new EventDispatcher();
            bool received = false;
            Action<IEvent> callback = (e) => received = true;

            eventDispatcher.SubscribeToType(typeof(TestEvent), callback);

            eventDispatcher.Invoke(new SecondaryTestEvent());
            Assert.IsFalse(received);

            eventDispatcher.Invoke(new TestEvent());
            Assert.IsTrue(received);
        }

        [Test]
        public void Unsubscribe_From_Type()
        {
            EventDispatcher eventDispatcher = new EventDispatcher();
            bool received = false;
            Action<object> callback = (e) => received = true;

            eventDispatcher.SubscribeToAnyEvent(callback);
            eventDispatcher.Invoke(new TestEvent());
            Assert.IsTrue(received);

            eventDispatcher.UnsubscribeToAnyEvent(callback);
            received = false;
            eventDispatcher.Invoke(new TestEvent());
            Assert.IsFalse(received);
        }

        [Test]
        public void Subscribe_To_Any_Event()
        {
            EventDispatcher eventDispatcher = new EventDispatcher();
            bool received = false;
            Action<object> callback = (e) => received = true;

            eventDispatcher.SubscribeToAnyEvent(callback);

            eventDispatcher.Invoke(new TestEvent());
            Assert.IsTrue(received);
            received = false;

            eventDispatcher.Invoke(new SecondaryTestEvent());
            Assert.IsTrue(received);
        }

        [Test]
        public void Unsubscribe_From_Any_Event()
        {
            EventDispatcher eventDispatcher = new EventDispatcher();
            bool received = false;
            Action<object> callback = (e) => received = true;

            eventDispatcher.SubscribeToAnyEvent(callback);
            TestEvent testEvent = new TestEvent();
            eventDispatcher.Invoke(testEvent);
            Assert.IsTrue(received);

            eventDispatcher.UnsubscribeToAnyEvent(callback);
            received = false;
            eventDispatcher.Invoke(testEvent);
            Assert.IsFalse(received);
        }
    }
}
