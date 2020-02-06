using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Core.Injection;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Class for the actual runtime statemachine. Keeps track and updates all stacked <see cref="StateMachineLayer"/>s.
    /// Use scripting define symbol "DEBUG_FLOW" to print StateMachine related debugs
    /// </summary>
    [Serializable]
    public class StateMachine
    {
        public Action<StateMachineLayer, StateMachineLayer> LayerChangedEvent;
        public StateMachineLayer CurrentLayer { get { return LayerStack.Peek(); } }

        public Stack<StateMachineLayer> LayerStack { get; private set; }

		public StateMachine(IStateMachineData data, DependencyInjector injector = null)
        {
            LayerStack = new Stack<StateMachineLayer>();
            LayerStack.Push(new StateMachineLayer(this, data, injector));
        }

		public void Start()
		{
			if (CurrentLayer.Data.EntryState != null)
			{
				CurrentLayer.Start();
			}
            else
            {
                Debug.LogError(CurrentLayer.Data.Name + " has no EntryState");
            }
		}

        public void Update()
        {
            CurrentLayer?.Update();
        }

        public StateMachineLayer AddNewLayerToStack(StateMachineScriptableObjectData data)
        {
            StateMachineLayer prevLayer = LayerStack.Peek();
            StateMachineLayer newLayer = new StateMachineLayer(this, data);
            LayerStack.Push(newLayer);
            newLayer.Start(prevLayer.CurrentState);

            PrintDebug(string.Format("ADDING NEW LAYER, from {0}, {1} TO: {2}, {3}", prevLayer.Data.Name, prevLayer.CurrentState.Title, CurrentLayer.Data.Name, CurrentLayer.CurrentState.Title));
            LayerChangedEvent?.Invoke(prevLayer, newLayer);

            return newLayer;
        }

        public void PopCurrentLayer()
        {
            StateMachineLayer prevLayer = LayerStack.Pop();
            prevLayer.OnClose(CurrentLayer.CurrentState);

            PrintDebug(string.Format("POPPING LAYER, from {0}, {1} TO: {2}, {3}", prevLayer.Data.Name, prevLayer.CurrentState.Title, CurrentLayer.Data.Name, CurrentLayer.CurrentState.Title));
            LayerChangedEvent?.Invoke(prevLayer, LayerStack.Peek());
        }

        /// <summary>
        /// Prints Debug.Log if DEBUG_FLOW is added to the projects Scripting Define Symbols
        /// </summary>
        /// <param name="log"></param>
        [System.Diagnostics.Conditional("DEBUG_FLOW")]
        public static void PrintDebug(string log)
        {
            Debug.Log(log);
        }
    }
}