using UnityEngine;

namespace Utils.Core.SceneManagement
{
    /// <summary>
    /// IMPORTANT: Known issue: should the SceneAsset's name change, this variable needs to be focused in order to update the sceneName string
	/// 
    /// Class for properly displaying a scene in the inspector. Won't lose track of the scene when changing folders
    /// Conveniently shows wether the scene is added to the build list in the inspector
    /// 
    /// Keeps track of a sceneAsset and sceneName. When making a build, the sceneAsset's reference gets lost due to how Unity internally handles scene assets, 
    /// therefore a seperate string is needed
    /// </summary>
    [System.Serializable]
	public class SceneField : ISerializationCallbackReceiver
	{
#if UNITY_EDITOR
		// Objects of type SceneAsset are not compiled with builds, and needs to be put in UNITY_EDITOR directives
		[SerializeField] protected Object sceneAsset = null;
#endif
		public string SceneName => sceneName;
		[SerializeField] protected string sceneName;

#if UNITY_EDITOR
		public SceneField(Object sceneAsset)
		{
			this.sceneAsset = sceneAsset;
			sceneName = sceneAsset.name;
		}
#endif

		public SceneField(string sceneName)
        {
			this.sceneName = sceneName;
        }

		public static implicit operator string(SceneField sceneField)
		{
			return sceneField.SceneName;
		}

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (sceneAsset != null)
                sceneName = sceneAsset.name;
#endif
        }

        public void OnAfterDeserialize()
        {

        }
    }
}