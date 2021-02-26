using UnityEngine;
using Utils.Core.Events;
using Utils.Core.Flow;

namespace Utils.Core.SceneManagement
{
    public class LoadSceneAction : StateAction
	{
		[SerializeField] private SceneField scene = null;

		protected SceneService sceneService;
		protected EventDispatcher eventDispatcher;

		public void InjectDependencies(SceneService sceneService, EventDispatcher eventDispatcher)
		{
			this.sceneService = sceneService;
			this.eventDispatcher = eventDispatcher;
		}

		public override void OnStarted()
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