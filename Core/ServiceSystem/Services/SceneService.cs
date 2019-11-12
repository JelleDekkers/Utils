using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Core.Services;

namespace Utils.Core.SceneManagement
{
	public class SceneService : IService
	{
		private CoroutineService coroutineService;

		public void InjectDependencies(CoroutineService coroutineService)
		{
			this.coroutineService = coroutineService;
		}

		public bool IsSceneLoaded(string scene)
		{
			return SceneManager.GetSceneByName(scene).isLoaded;
		}

		public void LoadScene(string scene, Action<string> onDone = null)
		{
			SceneManager.LoadScene(scene, LoadSceneMode.Single);
		}

		public void LoadSceneAdditive(string scene, Action<string> onDone = null)
		{
			SceneManager.LoadScene(scene, LoadSceneMode.Additive);
		}

		public void UnLoadScene(string scene, Action<string> onDone = null)
		{
			SceneManager.UnloadSceneAsync(scene);
		}

		public void LoadSceneAsync(string scene, Action<string> onDone = null)
		{
			coroutineService.StartCoroutine(LoadSceneCoroutine(scene, onDone));
		}

		private IEnumerator LoadSceneCoroutine(string scene, Action<string> onDone = null)
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}

			onDone?.Invoke(scene);
		}
	}
}