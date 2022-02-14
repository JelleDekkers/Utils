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
		/// Called when scene starts to load with delay
		/// </summary>
		public SceneLoadStartDelayHandler SceneLoadStartDelayEvent;
		public delegate void SceneLoadStartDelayHandler(string sceneName, float delay);

		/// <summary>
		/// Called when scene starts to load, independent of delay
		/// </summary>
		public Action<string> SceneLoadStartEvent;

		/// <summary>
		/// Called when scene is done loading
		/// </summary>
		public SceneLoadFinishEventHandler SceneLoadFinishEvent;
		public delegate void SceneLoadFinishEventHandler(Scene scene, LoadSceneMode loadMode);

		public AsyncOperation SceneLoadOperation { get; protected set; }

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

		public virtual void LoadScene(string sceneName, float delay, Action<string> onDone = null)
		{
			coroutineService.StartCoroutine(StartDelay(sceneName, delay, () => LoadScene(sceneName, onDone)));
		}

		public virtual void LoadSceneAdditive(string sceneName, Action<string> onDone = null)
		{
			SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
			IsLoadingScene = true;
			LoadingSceneName = sceneName;
			SceneLoadStartEvent?.Invoke(sceneName);
		}

		public virtual void LoadSceneAdditive(string sceneName, float delay, Action<string> onDone = null)
		{
			coroutineService.StartCoroutine(StartDelay(sceneName, delay, () => LoadSceneAdditive(sceneName, onDone)));
		}

		public virtual void UnLoadScene(string sceneName, Action<string> onDone = null)
		{
			SceneManager.UnloadSceneAsync(sceneName);
		}

		public virtual void UnLoadScene(string sceneName, float delay, Action<string> onDone = null)
		{
			coroutineService.StartCoroutine(StartDelay(sceneName, delay, () => UnLoadScene(sceneName, onDone)));
		}

		public virtual void LoadSceneAsync(string sceneName, Action<string> onDone = null)
		{
			coroutineService.StartCoroutine(LoadSceneAsyncCoroutine(sceneName, onDone));
			IsLoadingScene = true;
			LoadingSceneName = sceneName;
			SceneLoadStartEvent?.Invoke(sceneName);
		}

		public virtual void LoadSceneAsync(string sceneName, float delay, Action<string> onDone = null)
		{
			coroutineService.StartCoroutine(StartDelay(sceneName, delay, () => LoadSceneAsync(sceneName, onDone)));
		}

		protected virtual IEnumerator StartDelay(string sceneName, float seconds, Action onDone = null)
        {
			globalEventDispatcher.Invoke(new StartDelayedSceneLoadEvent(sceneName, seconds));
			IsLoadingScene = true;
			LoadingSceneName = sceneName;
			SceneLoadStartDelayEvent?.Invoke(sceneName, seconds);
			yield return new WaitForSeconds(seconds);
			onDone?.Invoke();
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