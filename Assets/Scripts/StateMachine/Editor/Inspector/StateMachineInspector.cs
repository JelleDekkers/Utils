using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        protected StateMachineRenderer stateMachineRenderer;
        protected InspectorUI inspector;

        public StateMachineInspector(StateMachineRenderer stateMachineRenderer)
        {
            this.stateMachineRenderer = stateMachineRenderer;

            Undo.undoRedoPerformed += Refresh;
        }

        public void OnInspectorGUI(Event e)
        {
            if(inspector == null) { return; }

            inspector.OnInspectorGUI(e);
        }

        public void Inspect(ScriptableObject inspectableObject)
        {
            InspectedObject = inspectableObject;

            inspector = GetCorrectUIBehaviour(inspectableObject);
            inspector.Show(stateMachineRenderer, inspectableObject);
        }

        private InspectorUI GetCorrectUIBehaviour(ScriptableObject inspectableObject)
        {
            IEnumerable<Type> validTypes = ReflectionUtility.GetDerivedTypes(typeof(InspectorUI).Assembly, typeof(InspectorUI));

            foreach (var type in validTypes)
            {
                object[] attributesFound = type.GetCustomAttributes(typeof(CustomInspectorUIAttribute), true);
                if (attributesFound != null || attributesFound.Length == 0)
                {
                    CustomInspectorUIAttribute attribute = attributesFound.FirstOrDefault() as CustomInspectorUIAttribute;

                    if (attribute == null) { continue; }

                    if (attribute.InspectorTargetType == inspectableObject.GetType() || attribute.InspectorTargetType.IsAssignableFrom(inspectableObject.GetType()))
                    {
                        return (InspectorUI)Activator.CreateInstance(type);
                    }
                }
            }

            return new InspectorUI();
        }

        public void Refresh()
        {
            inspector.Refresh();
        }

        public void Clear()
        {
            inspector = null;
        }

        public void Dispose()
        {
            Undo.undoRedoPerformed -= Refresh;
        }
    }
}