using System;
using System.Collections;
using UnityEngine;
using Utils.Core;
using Utils.Core.Services;

public class Timer
{
    public bool IsRunning { get; protected set; }
    public float TimeStarted { get; protected set; }
    public float Duration { get; protected set; }
    public float TimeRemaining => Mathf.Clamp(Duration - ElapsedTime, 0, Duration);
    public float ElapsedTime { get; protected set; }
    public float SpeedMultiplier { get; set; } = 1;
    public CoroutineTask Task { get; protected set; }
    public Action onDone;

    private readonly CoroutineService coroutineService;

    public Timer()
    {
        coroutineService = GlobalServiceLocator.Instance.Get<CoroutineService>();
    }

    public virtual void Set(float durationInSeconds)
    {
        Duration = durationInSeconds;
    }

    public virtual void Reset()
    {
        ElapsedTime = 0;
    }

    /// <summary>
    /// Starts the timer
    /// </summary>
    /// <param name="onDoneEvent">Callback when the timer runs out</param>
    /// <param name="elapsedTime">How much time has already elapsed before starting? Useful when synchronizing a timer over network that is already running.</param>
    public virtual void Start(Action onDoneEvent = null, float elapsedTime = 0)
    {
        if (Duration == 0)
            throw new Exception("Duration is cannot be 0, did you call Set()?");

        if(IsRunning)
            Stop();

        TimeStarted = Time.time;
        ElapsedTime = elapsedTime;

        if(onDoneEvent != null)
            onDone += onDoneEvent;

        Task = coroutineService.StartCoroutine(TimerCoroutine());
        IsRunning = true;
    }

    public virtual void Stop()
    {
        if (Task != null)
        {
            Task.Stop();
            Task = null;
        }
        IsRunning = false;
    }

    public virtual void Resume()
    {
        if (IsRunning)
        {
            Debug.LogWarning("Cant resume a timer thats still running!");
            return;
        }


        Task = coroutineService.StartCoroutine(TimerCoroutine());
        IsRunning = true;
    }

    protected virtual IEnumerator TimerCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(Duration);
        while (TimeRemaining > 0)
        {
            ElapsedTime += Time.deltaTime * SpeedMultiplier;
            yield return null;
        }
        onDone?.Invoke();
        Task = null;
        onDone = null;
        IsRunning = false;
    }
}