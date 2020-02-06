using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow
{
    /// <summary>
    /// Manager class for rendering a state machine in the editor
    /// </summary>
    public class StateMachineRenderer : IDisposable
    {
        private StateMachineLayerRenderer layerRenderer;
        private StateMachineScriptableObjectData stateMachineData;
        private StateMachine statemachine;

        private List<StateMachineScriptableObjectData> linkedLayers = new List<StateMachineScriptableObjectData>();
        private string[] linkedLayerNames = new string[0];
        private int currentLinkedLayerButtonSelected = 0;

        private GUIStyle linkedLayersToolbarStyle;
        private readonly Action repaint;

        public StateMachineRenderer(StateMachineScriptableObjectData data, StateMachine statemachine, Action repaint)
        {
            stateMachineData = data;
            this.statemachine = statemachine;
            this.repaint = repaint;
        }

        public void OnEnable() 
        {
            if (Application.isPlaying)
            {
                if (statemachine != null)
                {
                    statemachine.LayerChangedEvent += OnLayerChanged;
                }
            }
 
            if (stateMachineData != null)
            {
				layerRenderer = new StateMachineLayerRenderer((!Application.isPlaying || statemachine == null) ? stateMachineData : statemachine.CurrentLayer.Data, repaint, statemachine);
				LoadLinkedLayers();
			}

			Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }

        public void OnDisable()
        {
            if (Application.isPlaying)
            {
                if (statemachine != null)
                {
                    statemachine.LayerChangedEvent -= OnLayerChanged;
                }
            }

            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        private void OnUndoRedoPerformed()
        {
            if(Undo.GetCurrentGroupName() != StateMachineEditorUtility.UNDO_INSPECTOR_COMMAND_NAME)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            layerRenderer?.Refresh();
        }

        public void OnInspectorGUI()
        {
            if(linkedLayersToolbarStyle == null)
            {
                linkedLayersToolbarStyle = new GUIStyle("Button");
                linkedLayersToolbarStyle.fixedHeight = 20;
            }

            if (layerRenderer != null)
            {
                EditorGUILayout.Space();
                DrawLayersToolbar();
                EditorGUILayout.Space();
                layerRenderer.OnInspectorGUI();
            }
        }

        public void OnStateMachineDataChanged(StateMachineScriptableObjectData newData)
        {
            stateMachineData = newData;
            layerRenderer = (stateMachineData != null) ? new StateMachineLayerRenderer(stateMachineData, repaint, statemachine) : null;
        }

        private void DrawLayersToolbar()
        {
            float toolbarButtonMaxWidth = 150;

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            GUIContent buttonText = new GUIContent("Load linked layers");
            float heightNeeded = linkedLayersToolbarStyle.CalcSize(buttonText).x + 10;

            linkedLayersToolbarStyle.fontStyle = FontStyle.Bold;
            if (GUILayout.Button(buttonText, linkedLayersToolbarStyle, GUILayout.Width(heightNeeded)))
            {
                LoadLinkedLayers();
            }

            linkedLayersToolbarStyle.fontStyle = FontStyle.Normal;
            int prevSelectionIndex = currentLinkedLayerButtonSelected;
            currentLinkedLayerButtonSelected = GUILayout.Toolbar(currentLinkedLayerButtonSelected, linkedLayerNames, linkedLayersToolbarStyle, GUILayout.Width(toolbarButtonMaxWidth * linkedLayers.Count), GUILayout.MinWidth(10));
            if (currentLinkedLayerButtonSelected != prevSelectionIndex)
            {
                OnCurrentSelectedLinkedLayerChanged();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnCurrentSelectedLinkedLayerChanged()
        {
            layerRenderer = (stateMachineData != null) ? new StateMachineLayerRenderer(linkedLayers[currentLinkedLayerButtonSelected], repaint, statemachine) : null;
        }

        private void LoadLinkedLayers()
        {
            linkedLayers.Clear();
            linkedLayers.Add(stateMachineData);
            GetLinkedLayers(stateMachineData);

            linkedLayerNames = new string[linkedLayers.Count];
            for (int i = 0; i < linkedLayers.Count; i++)
            {
                linkedLayerNames[i] = linkedLayers[i].name;
            }
        }

        private void GetLinkedLayers(StateMachineScriptableObjectData data)
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
                            if (field.GetValue(action) != null && field.GetValue(action).GetType() == typeof(StateMachineScriptableObjectData) && !linkedLayers.Contains(field.GetValue(action) as StateMachineScriptableObjectData))
                            {
                                StateMachineScriptableObjectData linkedLayer = field.GetValue(action) as StateMachineScriptableObjectData;
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
            layerRenderer = (to.Data != null) ? new StateMachineLayerRenderer(to.Data, repaint, statemachine) : null;
        }

        public void Dispose()
        {
            layerRenderer?.OnDestroy();
        }
    }
}
