using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Core.Services
{
    public class CoroutineService : MonoBehaviour, IService, IDisposable
    {
        private const string OBJECT_NAME = "[SERVICE] CoroutineService";

        private Dictionary<IEnumerator, CoroutineTask> currentRunningTasks = new Dictionary<IEnumerator, CoroutineTask>(); 

        private void Awake()
        {
            name = OBJECT_NAME;
            DontDestroyOnLoad(this);
        }

        public new CoroutineTask StartCoroutine(IEnumerator enumerator)
        {
            CoroutineTask task = new CoroutineTask(enumerator, this);
            task.FinishedEvent += OnCoroutineTaskFinishedEvent;
            currentRunningTasks.Add(enumerator, task);

            return task;
        }

        private void OnCoroutineTaskFinishedEvent(CoroutineTask task, bool stoppedManually)
        {
            if (currentRunningTasks.ContainsKey(task.Enumerator))
            {
                currentRunningTasks.Remove(task.Enumerator);
            }
        }

        public new void StopCoroutine(IEnumerator enumerator)
        {
            if(currentRunningTasks.ContainsKey(enumerator))
            {
                currentRunningTasks[enumerator].Stop();
                currentRunningTasks.Remove(enumerator);
            }
        }

        public void StopCoroutine(CoroutineTask task)
        {
            StopCoroutine(task.Enumerator);
        }

        public void Dispose()
        {
            StopAllCoroutines();
            currentRunningTasks.Clear();
        }
    }
}
