using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Core.Services;

namespace Utils.Core.SceneManagement
{
	public class SceneService : IService
	{
        public Scene ActiveScene => SceneManager.GetActiveScene();

        /// <summary>
        /// Represents the scene loading progress when using LoadSceneAsync.
        /// </summary>
        public virtual float LoadingProgress
        {
            get
            {
                if (sceneLoadOperation != null)
                    return sceneLoadOperation.progress;
                else 
                    return 1;
            }
        }

        protected readonly CoroutineService coroutineService;
        private AsyncOperation sceneLoadOperation;

		public SceneService(CoroutineService coroutineService)
		{
			this.coroutineService = coroutineService;
		}

		public bool IsSceneLoaded(string sceneName)
		{
			return SceneManager.GetSceneByName(sceneName).isLoaded; 
		}

        public virtual void LoadScene(string scenePath, Action<string> onDone = null)
		{
			SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
		}

        public virtual void LoadSceneAdditive(string scene, Action<string> onDone = null)
		{
			SceneManager.LoadScene(scene, LoadSceneMode.Additive);
		}

        public virtual void UnLoadScene(string scene, Action<string> onDone = null)
		{
			SceneManager.UnloadSceneAsync(scene);
		}

        public virtual void LoadSceneAsync(string scene, Action<string> onDone = null)
		{
			coroutineService.StartCoroutine(LoadSceneAsyncCoroutine(scene, onDone));
		}

        public virtual IEnumerator LoadSceneAsyncCoroutine(string scene, Action<string> onDone = null)
		{
			sceneLoadOperation = SceneManager.LoadSceneAsync(scene);

			while (!sceneLoadOperation.isDone)
			{
				yield return null;
			}

            sceneLoadOperation = null;
			onDone?.Invoke(scene);
		}
	}
}