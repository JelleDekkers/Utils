using System;
using System.Collections;
using UnityEngine;
using Utils.Core;
using Utils.Core.Services;

public class Timer
{
    public bool IsRunning { get; private set; }
    public float TimeStarted { get; private set; }
    public float Duration { get; private set; }
    public float TimeRemaining => Mathf.Clamp(Duration - ElapsedTime, 0, Duration);
    public float ElapsedTime { get; private set; }
    public CoroutineTask Task { get; private set; }
    public Action onDone;

    private readonly CoroutineService coroutineService;

    public Timer()
    {
        coroutineService = GlobalServiceLocator.Instance.Get<CoroutineService>();
    }

    public IEnumerator TimerCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(Duration);
        while(TimeRemaining > 0)
        {
            ElapsedTime += Time.deltaTime;
            yield return null;
        }
        onDone?.Invoke();
        Task = null;
    }

    public void Start(float durationInSeconds, Action onDoneEvent = null)
    {
        if(IsRunning)
            Stop();

        TimeStarted = Time.time;
        ElapsedTime = 0;
        Duration = durationInSeconds;

        Task = coroutineService.StartCoroutine(TimerCoroutine());
        onDone += onDoneEvent;
        IsRunning = true;
    }

    public void Stop()
    {
        if (Task != null)
        {
            Task.Stop();
            Task = null;
        }
    }
}