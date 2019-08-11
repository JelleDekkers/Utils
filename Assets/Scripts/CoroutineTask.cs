using System.Collections;
using UnityEngine;

/// <summary>
/// Wrapper class for coroutines. Can start, stop and pause coroutines
/// </summary>
public class CoroutineTask
{
    /// <summary>
    /// Returns true if the coroutine is running. Paused tasks are considered to be running
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Returns true if the coroutine is currently paused
    /// </summary>
    public bool IsPaused { get; private set; }

    /// <summary>
    /// The enumerator (function) thats being called by the coroutine
    /// </summary>
    public IEnumerator Enumerator { get; private set; }

    /// <summary>
    /// The component that is executing the coroutine
    /// </summary>
    public MonoBehaviour Executor { get; private set; }

    public delegate void FinishedEventHandler(CoroutineTask task, bool stoppedManually);

    /// <summary>
    /// Termination event. Called when the coroutine completes execution.
    /// The bool parameter is for checking wether the coroutine was stopped manually
    /// </summary>
    public event FinishedEventHandler FinishedEvent;

    private bool stoppedManually;

    public CoroutineTask(IEnumerator enumerator, MonoBehaviour executor, bool autoStart = true)
    {
        Enumerator = enumerator;
        Executor = executor;

        if(autoStart)
        {
            Start();
        }
    }

    public void Pause()
    {
        IsPaused = true;
    }

    public void Unpause()
    {
        IsPaused = false;
    }

    public void Start()
    {
        IsRunning = true;
        Executor.StartCoroutine(CallWrapper());
    }

    public void Stop()
    {
        stoppedManually = true;
        IsRunning = false;
    }

    private IEnumerator CallWrapper()
    {
        yield return null;

        while (IsRunning)
        {
            if (IsPaused)
            {
                yield return null;
            }
            else
            {
                if (Enumerator != null && Enumerator.MoveNext())
                {
                    yield return Enumerator.Current;
                }
                else
                {
                    IsRunning = false;
                }
            }
        }

        FinishedEvent?.Invoke(this, stoppedManually);
    }
}