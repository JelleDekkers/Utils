using UnityEngine;
using Utils.Core.Attributes;
using Utils.Core.Events;
using Utils.Core.Flow;

namespace Utils.Core.SceneManagement
{
	public class LoadSceneAction : StateAction
	{
		[SerializeField, ScenePath] private string scene = string.Empty;

		private SceneService sceneService;
		private EventDispatcher eventDispatcher;

		public void InjectDependencies(SceneService sceneService, EventDispatcher eventDispatcher)
		{
			this.sceneService = sceneService;
			this.eventDispatcher = eventDispatcher;
		}

		public override void OnStarting()
		{
			sceneService.LoadScene(scene, OnSceneLoaded);
		}

		private void OnSceneLoaded(string sceneName)
		{
			eventDispatcher.Invoke(new SceneLoadedEvent(sceneName));
		}
	}
}