using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Editor class for <see cref="StateMachineExecutor"/>
    /// </summary>
    [CustomEditor(typeof(StateMachineExecutor))]
    public class StateMachineExecutorEditor : Editor
    {
        private StateMachineExecutor executor;
        private StateMachineUIImplementation editorUI;
        private StateMachineData stateMachineData;

        private List<StateMachineData> linkedLayers = new List<StateMachineData>();
        private string[] linkedLayerNames = new string[0];
        private int linkedLayersToolbarIndex = 0;

        private GUIStyle linkedLayersToolbarStyle;

        private void Awake()
        {
            executor = (StateMachineExecutor)target;
            stateMachineData = executor.StateMachineData;

            if (Application.isPlaying)
            {
                if (executor.Manager != null)
                {
                    executor.Manager.LayerChangedEvent += OnLayerChanged;
                }
            }

            linkedLayersToolbarStyle = new GUIStyle("Button");
            linkedLayersToolbarStyle.fixedHeight = 20;
        }

        protected void OnEnable() 
        {
            if (stateMachineData != null)
            {
                editorUI = new StateMachineUIImplementation(executor.StateMachineData, Repaint, executor.Manager);
            }            
        }

        public override void OnInspectorGUI()
        {
            Repaint();

            base.OnInspectorGUI();

            if (editorUI != null)
            {
                EditorGUILayout.Space();
                DrawLayersToolbar();
                EditorGUILayout.Space();
                editorUI.OnInspectorGUI();
            }

            if (executor.StateMachineData != stateMachineData)
            {
                stateMachineData = executor.StateMachineData;
                editorUI = (stateMachineData != null) ? new StateMachineUIImplementation(executor.StateMachineData, Repaint, executor.Manager) : null;
            }
        }

        private void DrawLayersToolbar()
        {
            float toolbarButtonMaxWidth = 400;

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            GUIContent buttonText = new GUIContent("Load linked layers");
            float heightNeeded = linkedLayersToolbarStyle.CalcSize(buttonText).x + 10;

            linkedLayersToolbarStyle.fontStyle = FontStyle.Bold;
            if (GUILayout.Button(buttonText, linkedLayersToolbarStyle, GUILayout.Width(heightNeeded)))
            {
                LoadLinkedLayers();
            }

            linkedLayersToolbarStyle.fontStyle = FontStyle.Normal;
            int prevSelectionIndex = linkedLayersToolbarIndex;
            linkedLayersToolbarIndex = GUILayout.Toolbar(linkedLayersToolbarIndex, linkedLayerNames, linkedLayersToolbarStyle, GUILayout.Width(toolbarButtonMaxWidth), GUILayout.MinWidth(10));
            if(linkedLayersToolbarIndex != prevSelectionIndex)
            {
                editorUI = (stateMachineData != null) ? new StateMachineUIImplementation(linkedLayers[linkedLayersToolbarIndex], Repaint, executor.Manager) : null;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void LoadLinkedLayers()
        {
            linkedLayers.Clear();
            linkedLayers.Add(stateMachineData);
            GetLinkedLayers(stateMachineData);

            linkedLayerNames = new string[linkedLayers.Count];
            for(int i = 0; i < linkedLayers.Count; i++)
            {
                linkedLayerNames[i] = linkedLayers[i].name;
            }
        }

        private void GetLinkedLayers(StateMachineData data)
        {
            foreach (State state in data.States)
            {
                foreach (StateAction action in state.TemplateActions)
                {
                    FieldInfo[] fields = action.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (FieldInfo field in fields)
                    {
                        if (field.GetCustomAttributes(typeof(SerializeField), true).Length > 0 || field.IsPublic)
                        {
                            if (field.GetValue(action).GetType() == typeof(StateMachineData) && !linkedLayers.Contains(field.GetValue(action) as StateMachineData))
                            {
                                StateMachineData linkedLayer = field.GetValue(action) as StateMachineData;
                                linkedLayers.Add(linkedLayer);
                                GetLinkedLayers(linkedLayer);
                            }
                        }
                    }
                }
            }
        }

        private void OnLayerChanged(StateMachineLayer from, StateMachineLayer to)
        {
            editorUI = (to.Data != null) ? new StateMachineUIImplementation(to.Data, Repaint, executor.Manager) : null;
        }

        private void OnDestroy()
        {
            editorUI?.Dispose();

            if (Application.isPlaying)
            {
                if (executor.Manager != null)
                {
                    executor.Manager.LayerChangedEvent -= OnLayerChanged;
                }
            }
        }
    }
}