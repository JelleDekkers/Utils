#if UNITY_EDITOR
[SerializedClassDrawer(TargetType = typeof(string), TargetAttributeType = typeof(GuidResourceAttribute))]
public class GuidResourceAttributeDrawer : ISerializedClassDrawer
{
    public object Draw(GUIContent label, object value, Type type, PropertyAttribute attribute)
    {
        UnityEngine.Object current = null;
        bool isMissing = false;

        if (!string.IsNullOrEmpty((string)value))
        {
            try
            {
                current = GuidDatabaseManager.Instance.MapGuidToObject((string)value, ((GuidResourceAttribute)attribute).BaseType);
            }
            catch { }

            isMissing = current == null;
        }
        if (isMissing)
        {
            GUI.contentColor = Color.red;
        }

        EditorGUI.BeginChangeCheck();
        UnityEngine.Object input = EditorGUILayout.ObjectField(label, current, ((GuidResourceAttribute)attribute).BaseType, false);
        bool dirty = EditorGUI.EndChangeCheck();

        string resourcePath = ResourceUtils.GetResourcePath(input);
        if (input != null)
        {
            if (string.IsNullOrEmpty(resourcePath))
            {
                EditorUtility.DisplayDialog("Weak resources", "Needs to be in a resource folder", "ok");
            }

            return GuidDatabaseManager.Instance.MapAssetToGuid(input);
        }

        if (!dirty)
        {
            return (string)value;
        }

        return "";
    }
}
#endif