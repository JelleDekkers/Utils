using System.IO;
using UnityEngine;
using Utils.Core.Attributes;
using Utils.Core.Flow;

namespace Utils.Core.SceneManagement
{
	public class SceneIsLoadedRule : Rule
	{
		public override bool IsValid => sceneService.IsSceneLoaded(sceneName);
		public override string DisplayName => "Loaded " + sceneName;

		[SerializeField, ScenePath] private string scene = string.Empty;

		private SceneService sceneService;
		private string sceneName;

		public void InjectDependencies(SceneService sceneService)
		{
			this.sceneService = sceneService;
		}

		public override void OnActivate()
		{
			sceneName = GetSceneName(scene);
		}

		private string GetSceneName(string fullPath)
		{
			return Path.GetFileNameWithoutExtension(fullPath);
		}
	}
}