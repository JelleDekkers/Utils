using System.IO;
using UnityEngine;
using Utils.Core.Attributes;
using Utils.Core.Flow;

namespace Utils.Core.SceneManagement
{
	public class SceneIsLoadedRule : Rule
	{
		public override bool IsValid => sceneService.IsSceneLoaded(SceneName);
		public override string DisplayName => "Loaded " + SceneName;

		private string SceneName
		{
			get
			{
				if (sceneName == string.Empty)
					sceneName = GetSceneName(scene);
				return sceneName;
			}
		}
		private string sceneName = string.Empty;

		[SerializeField] private SceneField scene = null;

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