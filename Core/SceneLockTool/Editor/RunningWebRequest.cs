using UnityEditor;
using System;
using UnityEngine.Networking;
using UnityEngine;

namespace Utils.Core.SceneLockTool
{
	public class RunningWebRequest
	{
		public string LoadingText { get; private set; } = "";
		public UnityWebRequest ConnectedWebRequest { get; private set; } = null;
		public bool IsDone { get; private set; } = false;
		public string Result { get; private set; } = "";

		private Action<WebRequestResult, string> callback = null;
		private string requestKey = "";

		private Action onSuccess = null;
		private Action onFail = null;
		private Action onNetwork = null;
		private Action onNull = null;
		private float TimeOfConception = 0f;
		private float timeOutTime = 30f;
		private bool isCleaned = false;

		public RunningWebRequest(UnityWebRequest connectedWebRequest, string requestKey, Action<WebRequestResult, string> callback, string loadingText = "Loading...", Action onSuccess = null, Action onFail = null, Action onNetwork = null, Action onNull = null)
		{
			LoadingText = loadingText;
			ConnectedWebRequest = connectedWebRequest;
			this.requestKey = requestKey;
			this.callback = callback;
			IsDone = false;
			TimeOfConception = Time.realtimeSinceStartup;
			this.onSuccess = onSuccess;
			this.onFail = onFail;
			this.onNetwork = onNetwork;
			this.onNull = onNull;
		}

		~RunningWebRequest()
		{
			Cleanup();
		}

		public void Run()
		{
			if (IsExpired())
			{
				Cleanup();
				callback?.Invoke(WebRequestResult.Unknown, "unknown");
				return;
			}

			if (!ConnectedWebRequest.isDone)
				return;

			if (ConnectedWebRequest.isNetworkError || ConnectedWebRequest.isHttpError)
			{
				Result = "network";
				onNetwork?.Invoke();
				callback?.Invoke(WebRequestResult.NoInternet, "no internet");
			}
			else
			{
				string result = ConnectedWebRequest.downloadHandler.text;

				//int keyLen = requestKey.Length;

				//int additionalCount = result.Count(x => x == 'o') * 2;

				//result = result.Remove(0, Mathf.Min(keyLen, result.Length));

				result = result.Replace("\n", "");
				result = result.Replace("\r", "");
				Result = result;

				if (Result.Contains("success"))
				{
					callback?.Invoke(WebRequestResult.Success, Result);
					onSuccess?.Invoke();
				}
				else if (Result.Contains("failure"))
				{
					callback?.Invoke(WebRequestResult.Failed, Result);
					onFail?.Invoke();
				}
				else if (Result.Contains("error"))
				{
					Result = "Encountered error: " + Result;
					callback?.Invoke(WebRequestResult.Failed, Result);
					onFail?.Invoke();
				}
				else
				{
					onNull?.Invoke();
					callback?.Invoke(WebRequestResult.Null, Result);
				}
			}

			IsDone = true;
			Cleanup();
		}

		public void Cleanup()
		{
			if (!isCleaned)
			{
				isCleaned = true;
				EditorApplication.update -= Run;
				if (ConnectedWebRequest != null)
				{
					ConnectedWebRequest.Abort();
					ConnectedWebRequest.Dispose();
				}
			}
		}

		public string GetResult()
		{
			return Result;
		}

		public bool IsExpired()
		{
			return TimeOfConception + timeOutTime < Time.realtimeSinceStartup;
		}

		public float GetRemainingTime()
		{
			return Mathf.Abs((TimeOfConception - Time.realtimeSinceStartup));
		}
	}
}