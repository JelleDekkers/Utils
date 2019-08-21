using System;
using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow.Inspector
{
    /// <summary>
    /// Abstract class for inspecting serialized properties of a ScriptableObject
    /// </summary>
    public class StateMachineInspector : IDisposable
    {
        public object CurrentInspectedObject { get; private set; }

        protected StateMachineLayerRenderer editorUI;
        protected IInspectorUIBehaviour uiBehaviour;
        private Action layoutEvent;

        public StateMachineInspector(StateMachineLayerRenderer editorUI)
        {
            this.editorUI = editorUI;

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

        public void Inspect(object target, IInspectorUIBehaviour uiBehaviour)
        {
            // This is needed to prevent UI errors 
            void InspectOnLayoutEvent()
            {
                if (editorUI.ShowDebug)
                {
                    this.uiBehaviour = new InspectorUIFallbackBehaviour(editorUI, new SerializedObject(editorUI.StateMachineData.SerializedObject));
                }
                else
                {
                    this.uiBehaviour = uiBehaviour;
                }

                layoutEvent -= InspectOnLayoutEvent;
            }

            layoutEvent += InspectOnLayoutEvent;
        }

        //public void Inspect(Type type, UnityEngine.Object parentObject, string propertyName)
        //{
        //    void inspectOnLayoutEvent()
        //    {
        //        this.parentObject = new SerializedObject(parentObject);
        //        targetProperty = this.parentObject.FindProperty(propertyName);

        //        uiBehaviour = GetUIBehaviour(type);
        //        layoutEvent -= inspectOnLayoutEvent;
        //    }

        //    layoutEvent += inspectOnLayoutEvent;
        //}

        //private InspectorUIFallbackBehaviour GetUIBehaviour(Type propertyType)
        //{
        //    if(manager.ShowDebug)
        //    {
        //        return new InspectorUIFallbackBehaviour(manager, parentObject, targetProperty);
        //    }
        //    IEnumerable<Type> validTypes = ReflectionUtility.GetDerivedTypes(typeof(InspectorUIFallbackBehaviour).Assembly, typeof(InspectorUIFallbackBehaviour));

        //    foreach (var type in validTypes)
        //    {
        //        object[] attributesFound = type.GetCustomAttributes(typeof(CustomInspectorUIAttribute), true);
        //        if (attributesFound != null || attributesFound.Length == 0)
        //        {
        //            CustomInspectorUIAttribute attribute = attributesFound.FirstOrDefault() as CustomInspectorUIAttribute;

        //            if (attribute == null) { continue; }

        //            Debug.Log(attribute.InspectorTargetType);
        //            if (attribute.InspectorTargetType == propertyType || attribute.InspectorTargetType.IsAssignableFrom(propertyType))
        //            {
        //                return (InspectorUIFallbackBehaviour)Activator.CreateInstance(type, manager, targetProperty);
        //            }
        //        }
        //    }

        //    return new InspectorUIFallbackBehaviour(manager, parentObject, targetProperty);
        //}

        /// <summary>
        /// Reinitializes UI
        /// </summary>
        public void Refresh()
        {
            if (uiBehaviour != null && CurrentInspectedObject != null)
            {
                uiBehaviour.Refresh();
            }
            else
            {
                Clear();
            }
        }

        public void Clear()
        {
            uiBehaviour = null;
        }

        private void OnObjectResetEvent(ScriptableObject obj)
        {
            if (uiBehaviour != null)
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