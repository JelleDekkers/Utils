using System;
using UnityEditor;

namespace Utils.Core.Events
{
    /// <summary>
    /// Editor window for displaying all events of type <see cref="IEvent"/>
    /// </summary>
    public class EventListWindow
    {
        protected static TypeFilterWindow window;
        protected static EventListWindow instance;

        [MenuItem("Utils/Events/EventListWindow")]
        private static void Open()
        {
            instance = new EventListWindow();
            instance.OpenFilterWindow<IEvent>();
        }

        public virtual void OpenFilterWindow<T>() where T : IEvent
        {
            window = EditorWindow.GetWindow<TypeFilterWindow>(true, "Displays all events, double-click to open the corresponding class.");
            window.RetrieveTypes<T>(OnTypeSelectedEvent);
        }

        private static void OnTypeSelectedEvent(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            EditorUtil.OpenScript(type);
        }
    }
}