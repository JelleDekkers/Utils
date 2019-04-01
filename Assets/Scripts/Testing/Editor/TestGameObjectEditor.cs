using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestGameObject))]
public class TestGameObjectEditor : Editor
{
    private TestGameObject testTarget;
    private SerializedProperty actionsList;

    private void OnEnable()
    {
        testTarget = (TestGameObject)target;
        actionsList = serializedObject.FindProperty("container").FindPropertyRelative("actions");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(actionsList, true);


        EditorGUILayout.LabelField("StateActions", EditorStyles.boldLabel);
        for (int i = 0; i < actionsList.arraySize; i++)
        {
            DrawPropertyField(actionsList.GetArrayElementAtIndex(i));
        }

        DrawDebugButtons();
    }

    private void DrawDebugButtons()
    {
        if (GUILayout.Button("Set as Test"))
        {
            testTarget.SetTest(CreateInstance<TestAction>());
        }

        if (GUILayout.Button("Set as SpawnPrefab"))
        {
            testTarget.SetTest(CreateInstance<SpawnPrefabAction>());
        }

        if (GUILayout.Button("Add Test to array"))
        {
            testTarget.AddToTest(CreateInstance<TestAction>());
        }

        if (GUILayout.Button("Add SpawnPrefab to array"))
        {
            testTarget.AddToTest(CreateInstance<SpawnPrefabAction>());
        }
    }

    private void DrawPropertyField(SerializedProperty property)
    {
        if (property.objectReferenceValue == null) { return; }

        Rect rect = EditorGUILayout.BeginHorizontal();
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, GUIContent.none, true);
        EditorGUILayout.LabelField(GetPropertyName(property), EditorStyles.largeLabel);
        EditorGUILayout.EndHorizontal();
        if (!property.isExpanded) { return; }

        SerializedObject targetObject = new SerializedObject(property.objectReferenceValue);
        if (targetObject == null) { return; }

        SerializedProperty field = targetObject.GetIterator();
        field.NextVisible(true);

        EditorGUILayout.BeginVertical("Helpbox", GUILayout.ExpandWidth(true));
        EditorGUI.indentLevel++;

        int index = 0;
        field = targetObject.GetIterator();
        field.NextVisible(true);

        while (field.NextVisible(false))
        {
            try
            {
                EditorGUILayout.PropertyField(field, true);
            }
            catch (StackOverflowException)
            {
                field.objectReferenceValue = null;
                Debug.LogError("Detected self-nesting cauisng a StackOverflowException, avoid using the same object iside a nested structure.");
            }

            index++;
        }

        targetObject.ApplyModifiedProperties();

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    private string GetPropertyName(SerializedProperty property)
    {
        string s = property.objectReferenceValue.ToString();

        string[] trim = { "(", ")" };
        for (int i = 0; i < trim.Length; i++)
        {
            s = s.Replace(trim[i], "");
        }

        return s;
    }
}
