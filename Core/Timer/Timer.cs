using System;
using System.Collections;
using UnityEngine;
using Utils.Core;
using Utils.Core.Services;

public class Timer
{
    public float TimeStarted { get; private set; }
    public float Duration { get; private set; }

    public float TimeRemaining => Mathf.Clamp(TimeStarted + Duration - Time.time, 0, Duration);
    public Action onDone;

    private CoroutineService coroutineService;
    public CoroutineTask Task { get; private set; }

    public Timer()
    {
        coroutineService = GlobalServiceLocator.Instance.Get<CoroutineService>();
    }

    public IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(Duration);
        onDone?.Invoke();
        Task = null;
    }

    public void Start(float durationInSeconds, Action onDoneEvent = null)
    {
        TimeStarted = Time.time;
        Duration = durationInSeconds;
        Task = coroutineService.StartCoroutine(TimerCoroutine());
        onDone += onDoneEvent;
    }

    public void Stop()
    {
        Task?.Stop();
        Task = null;
    }
}