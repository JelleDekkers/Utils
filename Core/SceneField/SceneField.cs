using UnityEngine;

namespace Utils.Core.SceneManagement
{
    /// <summary>
    /// Class for properly displaying a scene in the inspector. Won't lose track of the scene when changing folders
    /// Conveniently shows wether the scene is added to the build list in the inspector
    /// 
    /// Known issue: should the SceneAsset's name change, this variable needs to be focused in order to update the sceneName string
    /// 
    /// Keeps track of a sceneAsset and sceneName. When making a build, the sceneAsset's reference gets lost due to how Unity internally handles scene assets, 
    /// therefore a seperate string is needed
    /// </summary>
    [System.Serializable]
    public class SceneField
    {
#if UNITY_EDITOR
        [SerializeField] private Object sceneAsset = null;
#endif
        [SerializeField] private string sceneName = string.Empty;

        public string SceneName => sceneName;

        public static implicit operator string(SceneField sceneField)
        {
            return sceneField.SceneName;
        }
    }
}