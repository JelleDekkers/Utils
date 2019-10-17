using System;
using UnityEditor;
using UnityEngine;
using Utils.Core.Events;
using Utils.Core.Services;

namespace Utils.Core
{
	/// <summary>
	/// Editor window for manually invoking events of type <see cref="IEvent"/>
	/// Will throw an error if the event does not have an empty constructor
	/// </summary>
    public class EventInvokeWindow
    {
        protected static TypeFilterWindow window;
        protected static EventInvokeWindow instance;
        protected static EventDispatcher eventDispatcher;

        [MenuItem("Utils/EventInvokeWindow")]
        private static void Open()
        {
            instance = new EventInvokeWindow();
            instance.OpenFilterWindow<IEvent>();

            eventDispatcher = GlobalServiceLocator.Instance.Get<EventDispatcher>();
        }

        public virtual void OpenFilterWindow<T>() where T : IEvent
        {
            window = EditorWindow.GetWindow<TypeFilterWindow>(true, "EventInvokeWindow - Select an event to manually invoke (only types with default donstructors are supported)");
            window.RetrieveTypesWithDefaultConstructors<T>(OnTypeSelectedEvent);
        }

        private static void OnTypeSelectedEvent(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

			IEvent @event;
			try
			{
				@event = Activator.CreateInstance(type) as IEvent;
				Debug.Log("EventInvokeWindow: invoking event of type: " + @event.GetType());
				eventDispatcher.Invoke(@event);
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat("EventInvokeWindow: {0}", e);
			}
        }
    }
}