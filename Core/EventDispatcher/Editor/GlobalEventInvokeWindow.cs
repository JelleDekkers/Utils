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
    public class GlobalEventInvokeWindow
    {
        protected static TypeFilterWindow window;
        protected static GlobalEventInvokeWindow instance;
        protected static GlobalEventDispatcher eventDispatcher;

        [MenuItem("Utils/EventInvokeWindow")]
        private static void Open()
        {
            instance = new GlobalEventInvokeWindow();
            instance.OpenFilterWindow<IEvent>();

            eventDispatcher = GlobalServiceLocator.Instance.Get<GlobalEventDispatcher>();
        }

        public virtual void OpenFilterWindow<T>() where T : IEvent
        {
            window = EditorWindow.GetWindow<TypeFilterWindow>(true, "GlobalEventInvokeWindow - Select an event to manually invoke (only types with default donstructors are supported)");
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
				Debug.Log("GlobalEventInvokeWindow: invoking global event of type: " + @event.GetType());
				eventDispatcher.Invoke(@event);
			}
			catch (Exception e)
			{
				Debug.LogErrorFormat("GlobalEventInvokeWindow: {0}", e);
			}
        }
    }
}