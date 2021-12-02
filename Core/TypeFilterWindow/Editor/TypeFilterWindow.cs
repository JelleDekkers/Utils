using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace Utils.Core
{
    /// <summary>
    /// Editor window for filtering types
    /// </summary>
    public class TypeFilterWindow : EditorWindow
    {
        private const float DOUBLE_CLICK_TIME_DELAY = 0.5f;

        public delegate void SelectHandler(Type type);
        public SelectHandler typeDoubleClickedEvent;
        public SelectHandler selectionChangedEvent;

        private const string DEFAULT_NAMESPACE_NAME = "No Namespace";
        private const int TEXT_FONT_SIZE = 12;
        private const int NAMESPACE_FONT_SIZE = 10;
        private const string SEARCHBOX_GUI_STYLE = "SearchTextField";
        private const string SEARCHBOX_CANCEL_BUTTON_GUI_STYLE = "SearchCancelButton";
        private const string SEARCHBOX_CANCEL_BUTTON_EMPTY_GUI_STYLE = "SearchCancelButtonEmpty";
        private const string SEARCH_FIELD_CONTROL_NAME = "SearchField";

        private readonly Color32 selectionBackgroundColor = new Color32(66, 135, 245, 255);

        protected Type HighlightedType
        {
            get
            {
                if (HighlightIndex >= 0)
                    return filteredTypes[HighlightIndex];
                else
                    return null;
            }
        }

        protected int HighlightIndex
        {
            get => highlightIndex;
            private set
            {
                bool newValue = (highlightIndex != value);
                highlightIndex = value;
                if(newValue)
                    selectionChangedEvent?.Invoke(HighlightedType);
            }
        }

        protected Type[] allTypes;
        protected Type[] filteredTypes;

        private string searchTerm;
        private int highlightIndex = -1;
        private Vector2 scrollPosition;
        private double timeLastClicked;
        private bool isFocusedOnInit;

        protected virtual void OnGUI()
        {
            ProcessUserInput(Event.current);

            DrawHeader();
            DrawContent();
        }

        protected void ProcessUserInput(Event e)
        {
            if (e.type == EventType.KeyDown)
            {
                //if (e.keyCode == KeyCode.DownArrow || e.keyCode == KeyCode.S)
                //{
                //    HighlightIndex++;
                //}
                //else if (e.keyCode == KeyCode.UpArrow || e.keyCode == KeyCode.W)
                //{
                //    HighlightIndex--;
                //}
                //else if (e.keyCode == KeyCode.Return)
                //{
                //    OnTypeDoubleClicked(HighlightedType);
                //}
                if (e.keyCode == KeyCode.Escape)
                {
                    Close();
                }

                HighlightIndex = Mathf.Clamp(HighlightIndex, 0, filteredTypes.Length - 1);
                Repaint();
            }
        }

        public void RetrieveTypes(Type type, SelectHandler onDoubleClick)
        {
            allTypes = ReflectionUtility.GetAllTypes(type).ToArray();
			filteredTypes = allTypes;
			Array.Sort(filteredTypes, delegate (Type x, Type y) { return x.Name.CompareTo(y.Name); });
			typeDoubleClickedEvent = onDoubleClick;
        }

        public void RetrieveTypes<T>(SelectHandler onDoubleClick)
        {
            RetrieveTypes(typeof(T), onDoubleClick);
        }
		
        protected virtual void DrawHeader()
        {
            GUILayout.BeginHorizontal();

            GUI.SetNextControlName(SEARCH_FIELD_CONTROL_NAME);
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

            if (!isFocusedOnInit)
            {
                GUI.FocusControl(SEARCH_FIELD_CONTROL_NAME);
                isFocusedOnInit = true;
            }
        }

        protected void DrawContent()
        {
            if (filteredTypes == null)
            {
                Close();
                return;
            }

            EditorGUILayout.BeginVertical();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < filteredTypes.Length; i++)
            {
                DrawElement(filteredTypes[i], i, filteredTypes[i].Name);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        protected Rect DrawElement(Type type, int index, string text)
        {
            Color backgroundColor = GUI.backgroundColor;

            if (type == HighlightedType)
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
                if (HighlightedType == type && EditorApplication.timeSinceStartup - timeLastClicked < DOUBLE_CLICK_TIME_DELAY)
                {
                    OnTypeDoubleClicked(type);
                }

                HighlightIndex = index;
                timeLastClicked = EditorApplication.timeSinceStartup;
            }

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = backgroundColor;

            return rect;
        }

        private void OnTypeDoubleClicked(Type type)
        {
            typeDoubleClickedEvent.Invoke(type);
        }

        private void FilterTypes(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                filteredTypes = allTypes;
                return;
            }

            filteredTypes = GetFilteredTypes(searchTerm);
            HighlightIndex = Mathf.Clamp(HighlightIndex, 0, filteredTypes.Length - 1);
        }

        private Type[] GetFilteredTypes(string searchTerm)
        {
            List<Type> filteredList = new List<Type>();

            foreach (Type t in allTypes)
            {
                if (t.Name.ToLower().Contains(searchTerm.ToLower()))
                    filteredList.Add(t);
            }

            return filteredList.ToArray();
        }
    }
}