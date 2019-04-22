using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StateMachine
{
    /// <summary>
    /// Abstract class for inspecting serialized properties of a ScriptableObject
    /// </summary>
    public class StateMachineInspector : IDisposable
    {
        public IInspectable InspectedObject { get; protected set; }

        protected StateMachine stateMachine;
        protected InspectorUI objectInspector;

        public StateMachineInspector(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;

            Undo.undoRedoPerformed += Refresh;
        }

        public void OnInspectorGUI(Event e)
        {
            if(objectInspector == null) { return; }

            objectInspector.OnInspectorGUI(e);
        }

        public void Inspect(IInspectable inspectableObject)
        {
            InspectedObject = inspectableObject;

            Type behaviour = InspectedObject.InspectorBehaviour;
            objectInspector = (StateInspectorUI)Activator.CreateInstance(behaviour);
            objectInspector.Show(stateMachine, InspectedObject);
        }

        public void Refresh()
        {
            objectInspector.Refresh();
        }

        public void Clear()
        {
            InspectedObject = null;
            objectInspector = null;
        }

        public void Dispose()
        {
            Undo.undoRedoPerformed -= Refresh;
        }
    }
}