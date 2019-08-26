using UnityEditor;
using UnityEngine;

namespace Utils.Core
{
    [InitializeOnLoad]
    public class HierarchyDividerEditor
    {
        static HierarchyDividerEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyOnGUI;
        }

        [MenuItem("GameObject/Hierarchy Divider", false, -1)]
        private static void CreateNewDivider()
        {
            GameObject divider = new GameObject();
            divider.AddComponent<HierarchyDivider>();
            divider.name = "New divider";
            Selection.objects = new GameObject[1] { divider };
            Undo.RegisterCreatedObjectUndo(divider, "Create divider");
        }

        private static void HierarchyOnGUI(int instanceID, Rect rect)
        {
            if (Event.current.type != EventType.Repaint) { return; }

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) { return; }
            HierarchyDivider divider = go.GetComponent<HierarchyDivider>();
            if (divider == null) { return; }

            Color fontColor = Color.black;
            Color backgroundColor = divider.backgroundColor;
            backgroundColor.a = 1;
            EditorGUI.DrawRect(rect, backgroundColor);

            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState() { textColor = fontColor }
            };

            string prefix = " ------------------------------ ";
            EditorGUI.LabelField(rect, prefix + go.name.ToUpper() + prefix, style);
        }
    }
}