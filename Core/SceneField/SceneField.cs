using System.Linq;
using System.Text.RegularExpressions;
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
#pragma warning disable CS0414
        [SerializeField] private Object sceneAsset = null;
#pragma warning disable CS0414
#endif
        [SerializeField] private string sceneName = string.Empty;

		public string actualName = "";
		public Vector2 dimensionsForChecks = new Vector2();
		public string dimensionsInMeters = "";
		public string dimensionsInImperial = "";

		public string SceneName => sceneName;

#if UNITY_EDITOR
        public SceneField(Object sceneAsset)
        {
            this.sceneAsset = sceneAsset;
            sceneName = sceneAsset.name;
        }
#endif
        public SceneField(string name)
        {
            sceneName = name;
        }

		public SceneField(string fileName, string sceneName)
		{
			this.sceneName = sceneName;
			AnalyzeName(fileName);
		}

		public void AnalyzeName(string levelName)
		{
			actualName = levelName.Substring(levelName.IndexOf("_") + 1, levelName.Length - (levelName.IndexOf("_") + 1));
			actualName = actualName.Replace(" ", "");
			actualName = actualName.Replace("_", " ");
			char space = ' ';
			while(actualName[actualName.Length - 1] == space)
				actualName = actualName.Substring(0, actualName.Length - 1);
			while(actualName[0] == space)
				actualName = actualName.Substring(1, actualName.Length - 1);

			Regex reg = new Regex("[ ]{2,}", RegexOptions.None);
			actualName = reg.Replace(actualName, " ");
			actualName = string.Join(" ", actualName.Split(' ').ToList().ConvertAll(word => word.Substring(0, 1).ToUpper() + word.Substring(1)));

			string dimensions = levelName.Substring(0, levelName.IndexOf("_"));

			string xDimCountStr = dimensions.Substring(0, dimensions.IndexOf("x"));
			xDimCountStr = new string(xDimCountStr.Where(c => char.IsDigit(c)).ToArray());
			int xDimCount = int.Parse(xDimCountStr);

			string yDimCountStr = dimensions.Substring(dimensions.IndexOf("x") + 1, dimensions.Length - (dimensions.IndexOf("x") + 1));
			yDimCountStr = new string(yDimCountStr.Where(c => char.IsDigit(c)).ToArray());
			int yDimCount = int.Parse(yDimCountStr);

			dimensionsForChecks = new Vector2(xDimCount, yDimCount);

			dimensionsInMeters = xDimCount.ToString() + "x" + yDimCount.ToString() + "m";
			int xDimImp = Mathf.RoundToInt((float)xDimCount * 3.281f);
			int yDimImp = Mathf.RoundToInt((float)yDimCount * 3.281f);
			dimensionsInImperial = xDimImp.ToString() + "x" + yDimImp.ToString() + "ft";
		}

		public static implicit operator string(SceneField sceneField)
        {
            return sceneField.SceneName;
        }
    }
}