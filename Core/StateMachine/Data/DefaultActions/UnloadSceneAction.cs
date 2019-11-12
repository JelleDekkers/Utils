using UnityEngine;
using Utils.Core.Attributes;
using Utils.Core.Flow;

namespace Utils.Core.SceneManagement
{
	public class UnloadSceneAction : StateAction
	{
		[SerializeField, ScenePath] private string scene = string.Empty;

		private SceneService sceneService;

		public void InjectDependencies(SceneService sceneService)
		{
			this.sceneService = sceneService;
		}

		public override void OnStarting()
		{
			sceneService.UnLoadScene(scene);
		}
	}
}