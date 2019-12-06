using UnityEngine;
using Utils.Core.Attributes;
using Utils.Core.Events;
using Utils.Core.Flow;

namespace Utils.Core.SceneManagement
{
	public class LoadSceneAction : StateAction
	{
		[SerializeField, ScenePath] private string scene = string.Empty;

		protected SceneService sceneService;
		protected EventDispatcher eventDispatcher;

		public void InjectDependencies(SceneService sceneService, EventDispatcher eventDispatcher)
		{
			this.sceneService = sceneService;
			this.eventDispatcher = eventDispatcher;
		}

		public override void OnStarting()
		{
            LoadScene();
		}

        protected void LoadScene()
        {
			sceneService.LoadScene(scene, OnSceneLoaded);
        }

		protected virtual void OnSceneLoaded(string sceneName)
		{
			eventDispatcher.Invoke(new SceneLoadedEvent(sceneName));
		}
	}
}