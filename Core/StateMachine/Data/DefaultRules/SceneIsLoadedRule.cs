using System.IO;
using UnityEngine;
using Utils.Core.Attributes;
using Utils.Core.Flow;

namespace Utils.Core.SceneManagement
{
	public class SceneIsLoadedRule : Rule
	{
		public override bool IsValid => sceneService.IsSceneLoaded(scene);
		public override string DisplayName => "Loaded " + GetSceneName(scene);

		[SerializeField, ScenePath] private string scene = string.Empty;

		private SceneService sceneService;

		public void InjectDependencies(SceneService sceneService)
		{
			this.sceneService = sceneService;
		}

		private string GetSceneName(string fullPath)
		{
			return Path.GetFileNameWithoutExtension(fullPath);
		}
	}
}