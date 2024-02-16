using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Core.Events;
using Utils.Core.Services;

namespace Utils.Core.SceneManagement
{
	public class SceneService : IService
	{
        public Scene ActiveScene => SceneManager.GetActiveScene();

		/// <summary>
		/// Is scene being loaded? Also see <see cref="LoadingSceneName"/>.
		/// </summary>
		public bool IsLoadingScene { get; protected set; }

		/// <summary>
		/// The name of the scene that is being loaded, null if nothing is being loaded.
		/// </summary>
		public string LoadingSceneName { get; protected set; }

        /// <summary>
        /// Represents the scene loading progress when using LoadSceneAsync.
        /// </summary>
        public virtual float LoadingProgress
        {
            get
            {
                if (SceneLoadOperation != null)
                    return SceneLoadOperation.progress;
                else 
                    return 1;
            }
        }

		/// <summary>
		/// Called when scene starts to load
		/// </summary>
		public Action<string> SceneLoadStartEvent;

		/// <summary>
		/// Called when scene is done loading
		/// </summary>
		public Action<Scene, LoadSceneMode> SceneLoadFinishEvent;

		public virtual AsyncOperation SceneLoadOperation { get; protected set; }

		protected readonly CoroutineService coroutineService;
		protected readonly GlobalEventDispatcher globalEventDispatcher;

		public SceneService(CoroutineService coroutineService, GlobalEventDispatcher globalEventDispatcher)
		{
			this.coroutineService = coroutineService;
			this.globalEventDispatcher = globalEventDispatcher;

			SceneManager.sceneLoaded += OnSceneLoaded;
		}

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
			SceneLoadFinishEvent?.Invoke(scene, loadMode);
			IsLoadingScene = false;
			LoadingSceneName = null;
        }

        public bool IsSceneLoaded(string sceneName)
		{
			return SceneManager.GetSceneByName(sceneName).isLoaded; 
		}

        public virtual void LoadScene(string sceneName, Action<string> onDone = null)
		{
			SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
			IsLoadingScene = true;
			LoadingSceneName = sceneName;
			SceneLoadStartEvent?.Invoke(sceneName);
		}

		public virtual void LoadSceneAdditive(string sceneName, Action<string> onDone = null)
		{
			SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
			IsLoadingScene = true;
			LoadingSceneName = sceneName;
			SceneLoadStartEvent?.Invoke(sceneName);
		}

		public virtual void UnLoadScene(string sceneName, Action<string> onDone = null)
		{
			SceneManager.UnloadSceneAsync(sceneName);
		}

		public virtual void LoadSceneAsync(string sceneName, Action<string> onDone = null)
		{
			coroutineService.StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onDone));
			IsLoadingScene = true;
			LoadingSceneName = sceneName;
			SceneLoadStartEvent?.Invoke(sceneName);
		}

		public virtual IEnumerator LoadSceneAsyncCoroutine(string sceneName, Action<string> onDone = null)
		{
			SceneLoadOperation = SceneManager.LoadSceneAsync(sceneName);
			while (!SceneLoadOperation.isDone)
			{
				yield return null;
			}

            SceneLoadOperation = null;
			onDone?.Invoke(sceneName);
		}
	}
}