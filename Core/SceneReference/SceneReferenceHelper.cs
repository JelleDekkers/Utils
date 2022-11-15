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
#endif
	}
}