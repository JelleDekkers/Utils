using UnityEngine;

namespace Utils.Core.SceneManagement
{
    /// <summary>
    /// Class for properly displaying a scene in the inspector. Won't lose track of the scene when changing it's name and/or change folders
    /// Shows wether the scene is added to the build list in the inspector
    /// </summary>
    [System.Serializable]
    public class SceneField
    {
        [SerializeField] private Object sceneAsset = null;

        public string SceneName => sceneAsset.name;

        // makes it work with the existing Unity methods (LoadLevel/LoadScene)
        public static implicit operator string(SceneField sceneField)
        {
            return sceneField.SceneName;
        }
    }
}