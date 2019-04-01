using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class GraphNode : BaseNode
    {
        private BehaviourGraph previousGraph;

        public override void DrawWindow()
        {
            if(BehaviourEditor.currentGraph == null)
            {
                EditorGUILayout.LabelField("Add graph to modify:");
            }

            BehaviourEditor.currentGraph = (BehaviourGraph)EditorGUILayout.ObjectField(BehaviourEditor.currentGraph, typeof(BehaviourGraph), false);

            if(BehaviourEditor.currentGraph == null)
            {
                if(previousGraph != null)
                {
                    //clear windows
                    previousGraph = null;
                }

                EditorGUILayout.LabelField("No graph assigned");
                return;
            }

            if(previousGraph != BehaviourEditor.currentGraph)
            {
                previousGraph = BehaviourEditor.currentGraph;
                BehaviourEditor.LoadGraph();
            }
        }

        public override void DrawCurve()
        {

        }
    }
}