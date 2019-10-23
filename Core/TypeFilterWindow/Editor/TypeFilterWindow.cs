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
        private SelectHandler selectionCallback;

        private const string DEFAULT_NAMESPACE_NAME = "No Namespace";
        private const int TEXT_FONT_SIZE = 12;
        private const int NAMESPACE_FONT_SIZE = 10;
        private const string SEARCHBOX_GUI_STYLE = "SearchTextField";
        private const string SEARCHBOX_CANCEL_BUTTON_GUI_STYLE = "SearchCancelButton";
        private const string SEARCHBOX_CANCEL_BUTTON_EMPTY_GUI_STYLE = "SearchCancelButtonEmpty";
        private const string SEARCH_FIELD_CONTROL_NAME = "SearchField";

        private readonly Color32 selectionBackgroundColor = new Color32(62, 125, 231, 128);

        private Type HighlightedType
        {
            get
            {
                if (highlightIndex >= 0)
                    return filteredTypes[highlightIndex];
                else
                    return null;
            }
        }

        private string searchTerm;
        private int highlightIndex = -1;
        private Type[] allTypes;
        private Type[] filteredTypes;
        private Vector2 scrollPosition;
        private double timeLastClicked;
        private bool isFocusedOnInit;

        protected void OnGUI()
        {
            ProcessUserInput(Event.current);

            DrawHeader();
            DrawContent();
        }

        private void ProcessUserInput(Event e)
        {
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.DownArrow || e.keyCode == KeyCode.S)
                {
                    highlightIndex++;
                }
                else if (e.keyCode == KeyCode.UpArrow || e.keyCode == KeyCode.W)
                {
                    highlightIndex--;
                }
                else if (e.keyCode == KeyCode.Return)
                {
                    OnTypeSelectedEvent(HighlightedType);
                }
                else if (e.keyCode == KeyCode.Escape)
                {
                    Close();
                }

                highlightIndex = Mathf.Clamp(highlightIndex, 0, filteredTypes.Length - 1);
                Repaint();
            }
        }

        public void RetrieveTypes(Type type, SelectHandler onSelection)
        {
            allTypes = ReflectionUtility.GetAllTypes(type).ToArray();
			filteredTypes = allTypes;
            selectionCallback = onSelection;
        }

        public void RetrieveTypes<T>(SelectHandler onSelection)
        {
            RetrieveTypes(typeof(T), onSelection);
        }

		public void RetrieveTypesWithDefaultConstructors(Type type, SelectHandler onSelection)
		{
			allTypes = ReflectionUtility.GetAllTypes(type).ToArray();
			List<Type> temp = new List<Type>();

			foreach (Type t in allTypes)
			{
				if (t.HasDefaultConstructor())
				{
					temp.Add(t);
				}
			}

			filteredTypes = temp.ToArray();
			selectionCallback = onSelection;
		}

		public void RetrieveTypesWithDefaultConstructors<T>(SelectHandler onSelection)
		{
			RetrieveTypesWithDefaultConstructors(typeof(T), onSelection);
		}
		
        private void DrawHeader()
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
                    OnTypeSelectedEvent(type);
                }

                highlightIndex = index;
                timeLastClicked = EditorApplication.timeSinceStartup;
            }

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = backgroundColor;

            return rect;
        }

        private void OnTypeSelectedEvent(Type type)
        {
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
            highlightIndex = Mathf.Clamp(highlightIndex, 0, filteredTypes.Length - 1);
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