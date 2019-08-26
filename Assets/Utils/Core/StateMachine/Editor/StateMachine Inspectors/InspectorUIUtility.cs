using System;
using UnityEditor;
using UnityEngine;

namespace Utils.Core.Flow.Inspector
{
    public static class InspectorUIUtility
    {
        public static void DrawPropertyFields(SerializedObject serializedObject, string propertyName, GenericMenu.MenuFunction2 contextMenuPressedCallback = null)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            DrawPropertyFields(property, contextMenuPressedCallback);
        }

        public static void DrawPropertyFields(SerializedProperty serializedProperty, GenericMenu.MenuFunction2 contextMenuPressedCallback = null)
        {
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                //if (i != 0) { DrawHorizontalLine(); }
                DrawPropertyArrayField(serializedProperty, i, contextMenuPressedCallback);
            }

            if (serializedProperty.arraySize == 0)
            {
                GUIStyle style = new GUIStyle("Label");
                style.alignment = TextAnchor.UpperLeft;
                GUILayout.Label("Empty", style);
            }
        }

        /// <summary>
        /// Draws all properties inside <see cref="property"/> from an array
        /// </summary>
        /// <param name="property"></param>
        /// <param name="index"></param>
        public static void DrawPropertyArrayField(SerializedProperty property, int index, GenericMenu.MenuFunction2 contextMenuPressedCallback = null)
        {
            property = property.GetArrayElementAtIndex(index);
            if (property.objectReferenceValue == null) { return; }

            Rect header = EditorGUILayout.BeginHorizontal(NodeGUIStyles.InspectorStyle);
            header.x += 3;
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, GUIContent.none, true);

            EditorGUI.LabelField(header, NicifyPropertyName(property), NodeGUIStyles.FieldNameLabelStyle);
            if (contextMenuPressedCallback != null)
            {
                DrawContextMenuDropdown(index, property.objectReferenceValue as ScriptableObject, contextMenuPressedCallback);
            }
            EditorGUILayout.EndHorizontal();

            if (!property.isExpanded) { return; }
            EditorGUI.indentLevel++;

            SerializedObject obj = new SerializedObject(property.objectReferenceValue);
            if (obj == null) { return; }

            SerializedProperty field = obj.GetIterator();
            field.NextVisible(true);

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            EditorGUI.indentLevel++;

            field = obj.GetIterator();
            field.NextVisible(true);

            EditorGUIUtility.labelWidth = header.width / 2.5f;
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
            }

            obj.ApplyModifiedProperties();

            EditorGUI.indentLevel -= 2;
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws all properties of <see cref="TargetObject"/>.
        /// </summary>
        public static void DrawAllProperties(SerializedObject targetObject)
        {
            SerializedProperty property = targetObject.GetIterator();

            if (property.NextVisible(true))
            {
                do
                {
                    Rect header = EditorGUILayout.BeginHorizontal(NodeGUIStyles.InspectorStyle);
                    EditorGUIUtility.labelWidth = header.width / 2.5f;
                    EditorGUILayout.PropertyField(targetObject.FindProperty(property.name), true);
                    EditorGUILayout.EndHorizontal();
                }
                while (property.NextVisible(false));
            }
            targetObject.ApplyModifiedProperties();
        }

        public static string NicifyPropertyName(SerializedProperty property)
        {
            string s = property.objectReferenceValue.GetType().Name;

            string[] trim = { "(", ")" };
            for (int i = 0; i < trim.Length; i++)
            {
                s = s.Replace(trim[i], "");
            }

            return s;
        }

        public static void DrawContextMenuDropdown(int index, ScriptableObject obj, GenericMenu.MenuFunction2 contextMenuPressedCallback)
        {
            GUIStyle iconStyle = new GUIStyle("IconButton");
            GUIContent iconContent = EditorGUIUtility.IconContent("_Popup");
            Vector2 size = iconStyle.CalcSize(iconContent);

            if (GUILayout.Button(iconContent, iconStyle, GUILayout.MaxWidth(size.x)))
            {
                GenericMenu menu = new GenericMenu();
                foreach (var command in (ContextMenu.Command[])Enum.GetValues(typeof(ContextMenu.Command)))
                {
                    menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(command.ToString())), false, contextMenuPressedCallback, new ContextMenu.Result(obj, command, index));
                }
                menu.ShowAsContext();
            }
        }

        public static void DrawHorizontalLine(float height = 1)
        {
            GUILayout.Box(GUIContent.none, GUILayout.MaxWidth(Screen.width), GUILayout.Height(height));
        }

        public static void DrawHeader(string title, params Action[] extraContent)
        {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.fontSize = 13;

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < extraContent.Length; i++)
            {
                extraContent[i].Invoke();
            }
            EditorGUILayout.LabelField(title, style);
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawAddNewButton(Action buttonPressedEvent)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = GUI.skin.button.normal.background;
            style.padding.left = 1;
            style.margin.top = 3;
            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), style, GUILayout.MaxWidth(18)))
            {
                buttonPressedEvent.Invoke();
            }

            GUI.backgroundColor = prevColor;
        }

        public static void OpenTypeFilterWindow(Type type, Action<Type> typeSelectedEvent)
        {
            TypeFilterWindow window = EditorWindow.GetWindow<TypeFilterWindow>(true, string.Format("Choose {0} to add", type.Name));
            TypeFilterWindow.SelectHandler func = (Type t) =>
            {
                typeSelectedEvent.Invoke(t);
                window.Close();
            };
            window.RetrieveTypes(type, func);
        }

        public static void OpenScript(ScriptableObject obj)
        {
            MonoScript script = MonoScript.FromScriptableObject(obj);
            string filePath = AssetDatabase.GetAssetPath(script);
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath, 1);
        }
    }
}