using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Abstract class for inspecting serialized properties of a ScriptableObject
    /// </summary>
    public class StateMachineInspector : IDisposable
    {
        public ScriptableObject InspectedObject { get; private set; }

        protected StateMachineEditorManager manager;
        protected InspectorUIBehaviour uiBehaviour;

        private Action layoutEvent;

        public StateMachineInspector(StateMachineEditorManager manager)
        {
            this.manager = manager;

            Undo.undoRedoPerformed += Refresh;
            StateMachineEditorUtility.ObjectResetEvent += OnObjectResetEvent;
        }

        public void OnInspectorGUI(Event e)
        {   
            if (e.type == EventType.Layout)
            {
                layoutEvent?.Invoke();
            }

            uiBehaviour?.OnInspectorGUI(e);
        }

        public void Inspect(ScriptableObject targetObject)
        {
            void inspectOnLayoutEvent()
            {
                InspectedObject = targetObject;
                uiBehaviour = GetUIBehaviour(InspectedObject);
                layoutEvent -= inspectOnLayoutEvent;
            }

            layoutEvent += inspectOnLayoutEvent;
        }

        private InspectorUIBehaviour GetUIBehaviour(ScriptableObject inspectableObject)
        {
            if(manager.ShowDebug)
            {
                return new InspectorUIBehaviour(manager, inspectableObject);
            }

            IEnumerable<Type> validTypes = ReflectionUtility.GetDerivedTypes(typeof(InspectorUIBehaviour).Assembly, typeof(InspectorUIBehaviour));

            foreach (var type in validTypes)
            {
                object[] attributesFound = type.GetCustomAttributes(typeof(CustomInspectorUIAttribute), true);
                if (attributesFound != null || attributesFound.Length == 0)
                {
                    CustomInspectorUIAttribute attribute = attributesFound.FirstOrDefault() as CustomInspectorUIAttribute;

                    if (attribute == null) { continue; }

                    if (attribute.InspectorTargetType == inspectableObject.GetType() || attribute.InspectorTargetType.IsAssignableFrom(inspectableObject.GetType()))
                    {
                        return (InspectorUIBehaviour)Activator.CreateInstance(type, manager, inspectableObject);
                    }
                }
            }

            return new InspectorUIBehaviour(manager, inspectableObject);
        }

        /// <summary>
        /// Reinitializes UI
        /// </summary>
        public void Refresh()
        {
            Inspect(InspectedObject);
        }

        public void Clear()
        {
            uiBehaviour = null;
            InspectedObject = null;
        }

        public void OnObjectResetEvent(ScriptableObject obj)
        {
            if (InspectedObject == obj)
            {
                Refresh();
            }
        }

        public void Dispose()
        {
            Undo.undoRedoPerformed -= Refresh;
            StateMachineEditorUtility.ObjectResetEvent -= OnObjectResetEvent;
        }
    }
}