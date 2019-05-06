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
        protected InspectorUIBehaviour uiBehaviour;

        public StateMachineInspector(StateMachineRenderer stateMachineRenderer)
        {
            this.stateMachineRenderer = stateMachineRenderer;

            Undo.undoRedoPerformed += Refresh;
        }

        public void OnInspectorGUI(Event e)
        {
            if(uiBehaviour == null) { return; }

            uiBehaviour.OnInspectorGUI(e);
        }

        public void Inspect(IInspectable inspectable)
        {
            InspectedObject = inspectable.InspectableObject;

            uiBehaviour = GetCorrectUIBehaviour(InspectedObject);
            uiBehaviour.Show(stateMachineRenderer, InspectedObject);
        }

        private InspectorUIBehaviour GetCorrectUIBehaviour(ScriptableObject inspectableObject)
        {
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
                        return (InspectorUIBehaviour)Activator.CreateInstance(type);
                    }
                }
            }

            return new InspectorUIBehaviour();
        }

        public void Refresh()
        {
            if(InspectedObject != null)
            {
                uiBehaviour.Refresh();
            }
        }

        public void Clear()
        {
            uiBehaviour = null;
            InspectedObject = null;
        }

        public void Dispose()
        {
            Undo.undoRedoPerformed -= Refresh;
        }
    }
}