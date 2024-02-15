using System;
using UnityEngine;

namespace Utils.Core.SceneManagement
{
    /// <summary>
    /// Class used to serialize a reference to a scene asset that can be used at runtime in a build, when the asset can no longer be directly
    /// referenced. This caches the scene name based on the SceneAsset to use at runtime to load.
    /// </summary>
    [Serializable]
	public class SceneReference : ISerializationCallbackReceiver
	{
        public string SceneName => sceneName;
        public bool IsAssigned => !string.IsNullOrEmpty(sceneName);

#if UNITY_EDITOR
		public UnityEditor.SceneAsset SceneAsset => scene;

		[SerializeField] private UnityEditor.SceneAsset scene;
#endif
		[Tooltip("The name of the referenced scene. This may be used at runtime to load the scene.")]
		[SerializeField] private string sceneName;
		[SerializeField] private int sceneIndex = -1;
		[SerializeField] private bool sceneEnabled;

		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			if (scene != null)
			{
				string sceneAssetPath = UnityEditor.AssetDatabase.GetAssetPath(scene);
				string sceneAssetGUID = UnityEditor.AssetDatabase.AssetPathToGUID(sceneAssetPath);

                (bool Present, bool Enabled, int Index) sceneInBuild = SceneReferenceHelper.GetSceneInBuildState(sceneAssetGUID);
				if (sceneInBuild.Enabled) sceneName = scene.name;
			}
			else
			{
				sceneName = "";
			}
#endif
		}

		public void OnAfterDeserialize()
		{
		}

        public override string ToString()
        {
			return sceneName;
        }
    }
}