using UnityEngine;
using Utils.Core.Flow;

namespace Utils.Core.SceneManagement
{
    public class UnloadSceneAction : StateAction
	{
        [SerializeField] private SceneField scene = null;

		private SceneService sceneService;

		public void InjectDependencies(SceneService sceneService)
		{
			this.sceneService = sceneService;
		}

		public override void OnStarted()
		{
			sceneService.UnLoadScene(scene);
		}
	}
}