using UnityEditor;

namespace Utils.Core.SceneManagement
{
    public class SceneFieldHelper
    {
        public static bool IsSceneAddedToBuild(SceneAsset scene)
        {
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(EditorBuildSettings.scenes[i].guid.ToString());
                SceneAsset asset = AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset)) as SceneAsset;

                if (asset == scene)
                    return true;
            }
            return false;
        }
    }
}