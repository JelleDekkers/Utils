﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Manager class for a StateMachine. Keeps track and updates all stacked <see cref="StateMachineLayer"/>s
    /// Use scripting define symbol "DEBUG_FLOW" to print StateMachine related debugs
    /// </summary>
    [Serializable]
    public class StateMachineManager
    {
        public Action<StateMachineLayer, StateMachineLayer> LayerChangedEvent;
        public StateMachineLayer CurrentLayer { get { return layerStack.Peek(); } }

        private Stack<StateMachineLayer> layerStack = new Stack<StateMachineLayer>();

        public StateMachineManager(StateMachineData data)
        {
            StateMachineLayer layer = new StateMachineLayer(this, data);
            layerStack.Push(layer);
            layer.Start();
        }

        public void Update()
        {
            if (CurrentLayer != null)
            {
                CurrentLayer.Update();
            }
        }

        public StateMachineLayer AddNewLayerToStack(StateMachineData data)
        {
            StateMachineLayer prevLayer = layerStack.Peek();
            StateMachineLayer newLayer = new StateMachineLayer(this, data);
            layerStack.Push(newLayer);
            newLayer.Start(prevLayer.CurrentState);

            PrintDebug(string.Format("ADDING NEW LAYER, from {0}, {1} TO: {2}, {3}", prevLayer.Data.name, prevLayer.CurrentState.Title, CurrentLayer.Data.name, CurrentLayer.CurrentState.Title));
            LayerChangedEvent?.Invoke(prevLayer, newLayer);

            return newLayer;
        }

        public void PopCurrentLayer()
        {
            StateMachineLayer prevLayer = layerStack.Pop();
            prevLayer.OnClose(CurrentLayer.CurrentState);

            PrintDebug(string.Format("POPPING LAYER, from {0}, {1} TO: {2}, {3}", prevLayer.Data.name, prevLayer.CurrentState.Title, CurrentLayer.Data.name, CurrentLayer.CurrentState.Title));
            LayerChangedEvent?.Invoke(prevLayer, layerStack.Peek());
        }

        /// <summary>
        /// Prints Debug.Log if DEBUG_FLOW is found in the projects Scripting Define Symbols
        /// </summary>
        /// <param name="log"></param>
        [System.Diagnostics.Conditional("DEBUG_FLOW")]
        public static void PrintDebug(string log)
        {
            Debug.Log(log);
        }
    }
}