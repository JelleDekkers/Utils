using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Editor window for filtering types
/// </summary>
public class TypeFilterWindow : EditorWindow
{
    public Type Selection { protected set; get; }

    public delegate void SelectHandler(Type type);
    private SelectHandler selectionCallback;

    private const string DEFAULT_NAMESPACE_NAME = "No Namespace";
    private const int TEXT_FONT_SIZE = 12;
    private const int NAMESPACE_FONT_SIZE = 10;
    private const string SEARCHBOX_GUI_STYLE = "SearchTextField";
    private const string SEARCHBOX_CANCEL_BUTTON_GUI_STYLE = "SearchCancelButton";
    private const string SEARCHBOX_CANCEL_BUTTON_EMPTY_GUI_STYLE = "SearchCancelButtonEmpty";

    private string searchTerm;
    private IEnumerable<Type> allTypes;
    private IEnumerable<Type> filteredTypes;
    private Vector2 scrollPosition;

    private Color32 selectionBackgroundColor = new Color32(62, 125, 231, 128);

    protected void OnGUI()
    {
        DrawSearchBox();
        DrawScrollView();
    }

    public void RetrieveTypes<T>(SelectHandler onSelection)
    {
        RetrieveTypes(typeof(T), onSelection);
    }

    public void RetrieveTypes(Type type, SelectHandler onSelection)
    {
        allTypes = ReflectionUtility.GetAllTypes(type);
        filteredTypes = allTypes;

        selectionCallback = onSelection;
    }

    protected void DrawScrollView()
    {
        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        foreach (Type type in filteredTypes)
        {
            DrawElement(type, type.Name);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    protected Rect DrawElement(Type type, string text)
    {
        Color backgroundColor = GUI.backgroundColor;

        if (type == Selection)
        {
            GUI.backgroundColor = selectionBackgroundColor;
        }

        Rect rect = EditorGUILayout.BeginVertical(GUI.skin.box);

        string namespaceText = (!string.IsNullOrEmpty(type.Namespace) ? type.Namespace : DEFAULT_NAMESPACE_NAME);

        GUIStyle textStyle = EditorStyles.label;
        textStyle.fontSize = TEXT_FONT_SIZE;
        GUILayout.Label(text, textStyle);

        textStyle.fontSize = NAMESPACE_FONT_SIZE;
        GUILayout.Label(namespaceText, textStyle);

        if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
        {
            OnTypeSelectedEvent(type);
        }

        EditorGUILayout.EndVertical();

        GUI.backgroundColor = backgroundColor;

        return rect;
    }

    private void DrawSearchBox()
    {
        GUILayout.BeginHorizontal();

        string newSearch = GUILayout.TextField(searchTerm, new GUIStyle(SEARCHBOX_GUI_STYLE));

        if (!string.IsNullOrEmpty(searchTerm) && GUILayout.Button(string.Empty, new GUIStyle(SEARCHBOX_CANCEL_BUTTON_GUI_STYLE)))
        {
            newSearch = string.Empty;
        }
        else if (string.IsNullOrEmpty(searchTerm))
        {
            GUILayout.Button(string.Empty, new GUIStyle(SEARCHBOX_CANCEL_BUTTON_EMPTY_GUI_STYLE));
        }

        if (newSearch != searchTerm)
        {
            searchTerm = newSearch;
            FilterTypes(searchTerm);
        }

        GUILayout.EndHorizontal();
    }

    private void OnTypeSelectedEvent(Type type)
    {
        Selection = type;
        selectionCallback.Invoke(type);
    }

    private void FilterTypes(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            filteredTypes = allTypes;
            return;
        }

        filteredTypes = GetFilteredTypes(searchTerm);
    }

    private IEnumerable<Type> GetFilteredTypes(string searchTerm)
    {
        List<Type> filteredList = new List<Type>();

        foreach (Type t in allTypes)
        {
            if (t.Name.ToLower().Contains(searchTerm.ToLower()))
                filteredList.Add(t);
        }

        return filteredList;
    }
}
