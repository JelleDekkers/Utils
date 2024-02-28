using UnityEditor;

namespace Utils.Core.SceneManagement
{
    public class SceneReferenceHelper
	{
#if UNITY_EDITOR
		public static (bool present, bool enabled, int index) GetSceneInBuildState(string sceneGuid)
		{
			EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

			for (int i = 0; i < scenes.Length; i++)
			{
				if (scenes[i].guid.ToString() == sceneGuid)
					return (true, scenes[i].enabled, i);
			}

			return (false, false, -1);
		}

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
#endif
	}
}