using System.Collections.Generic;
using UnityEditor;

namespace Utils.Core.Extensions
{
    public static class EditorExtensions
    {
        public static List<T> FindAllAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        public static void RepaintAllInspectors()
        {
            System.Reflection.MethodInfo repaintInspectorsMethod = null;
            if (repaintInspectorsMethod == null)
            {
                var inspWin = typeof(EditorApplication).Assembly.GetType("UnityEditor.InspectorWindow");
                repaintInspectorsMethod = inspWin.GetMethod("RepaintAllInspectors", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            }
            repaintInspectorsMethod.Invoke(null, null);
        }
    }
}