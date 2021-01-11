using UnityEditor;
using UnityEngine;

namespace Utils.Core.EditorHierarchy
{
    [InitializeOnLoad]
    [CustomEditor(typeof(HierarchySeperator)), CanEditMultipleObjects]
    public class HierarchySeperatorEditor : Editor
    {

        [MenuItem("GameObject/Hierarchy Seperator", false, -1)]
        private static void CreateNewSeperator()
        {
            GameObject go = new GameObject("New Seperator");
            HierarchySeperator seperator = go.AddComponent<HierarchySeperator>();
            seperator.childBackgroundColor = HierarchySettings.Instance.seperatorGroupChildBackgroundColor;
            seperator.titleBackgroundColor = HierarchySettings.Instance.seperatorTitleBackgroundColor;
            
            Selection.objects = new GameObject[1] { go };
            Undo.RegisterCreatedObjectUndo(go, "Create seperator");
        }
    }
}