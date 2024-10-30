using System;
using System.Collections;
using UnityEngine;
using Utils.Core;
using Utils.Core.Services;

public class Timer : IDisposable
{
    public CoroutineTask Task { get; protected set; }
    public Action DoneEvent { get; set; }

    public bool CanBeStarted => !IsRunning && !IsPaused;
    public bool CanBePaused => IsRunning && !IsPaused;
    public bool CanBeStopped => IsRunning || IsPaused;

    public float TimeRemaining => Mathf.Clamp(Duration - ElapsedTime, 0, Duration);
    public virtual bool IsRunning { get; protected set; }
    public virtual bool IsPaused { get; protected set; }
    public virtual float TimeStarted { get; protected set; }
    public virtual float Duration { get; protected set; }
    public virtual float SpeedMultiplier { get; set; } = 1;
    public virtual float ElapsedTime { get; protected set; }
    public virtual bool ClearOnDoneEventOnComplete { get; private set; }

    protected CoroutineService coroutineService;

    /// <summary>
    /// Used to initialize the timer
    /// </summary>
    /// <param name="clearActionOnComplete"> True if the timer should keep action content on finishing allotted time. Default wipes on completion</param>
    public Timer(bool clearActionOnComplete = true)
    {
        SetClearActionOnComplete(clearActionOnComplete);
    }

    public void SetClearActionOnComplete(bool clearActionOnComplete)
	{
        ClearOnDoneEventOnComplete = clearActionOnComplete;
    }

    public virtual void Set(float durationInSeconds, float elapsedTime = 0)
    {
        Duration = durationInSeconds;
        ElapsedTime = elapsedTime;
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

        if(coroutineService == null)
            coroutineService = GlobalServiceLocator.Instance.Get<CoroutineService>();

        if(IsRunning)
            Stop();

        TimeStarted = Time.time;
        ElapsedTime = elapsedTime;

        if(onDoneEvent != null)
            DoneEvent += onDoneEvent;

        Task = coroutineService.StartCoroutine(TimerCoroutine());
        IsRunning = true;
        IsPaused = false;
    }

    public virtual void Stop()
    {
        if (Task != null)
        {
            Task.Stop();
            Task = null;
        }

        IsRunning = false;
        IsPaused = false;
    }

    public virtual void Pause()
    {
        if (Task != null)
        {
            Task.Pause();
        }

        IsRunning = false;
        IsPaused = true;
    }

    public virtual void Resume()
    {
        if (IsRunning)
        {
            Debug.LogWarning("Cant resume a timer thats still running!");
            return;
        }

        if (!IsPaused)
        {
            Debug.LogWarning("Cant resume a timer thats not paused!");
            return;
        }

        Task = coroutineService.StartCoroutine(TimerCoroutine());
        IsRunning = true;
        IsPaused = false;
    }

    protected virtual IEnumerator TimerCoroutine()
    {
        while (TimeRemaining > 0)
        {
            ElapsedTime += Time.deltaTime * SpeedMultiplier;
            yield return null;
        }

        OnTimerEnd();
    }

    protected virtual void OnTimerEnd()
    {
        Action cachedOnDone = DoneEvent;
        if (ClearOnDoneEventOnComplete)
            DoneEvent = null;

        Task = null;
        IsRunning = false;

        cachedOnDone?.Invoke();
    }

    public override string ToString()
    {
        return string.Format("{0:00}:{1:00}", Mathf.FloorToInt(TimeRemaining / 60), Mathf.FloorToInt(TimeRemaining % 60));
    }

    public void Dispose()
    {
        if (IsRunning)
            Stop();
        DoneEvent = null;
    }
}