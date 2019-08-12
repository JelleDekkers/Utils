using System;
using System.Collections;
using UnityEngine;
using Utils.Core.Events;
using Utils.Core.Services;

namespace Utils.Core.Flow.DefaultActions
{
    /// <summary>
    /// Invokes event with an optional delay
    /// </summary>
    public class DispatchEventAction : StateAction
    {
        [ClassTypeImplements(typeof(IEvent)), SerializeField] private ClassTypeReference eventType = null;

        [Tooltip("Optional delay in seconds")]
        [SerializeField] private float delayInSeconds = 0;

        private EventDispatcher eventDispatcher;
        private CoroutineService coroutineService;

        public void InjectDependencies(EventDispatcher eventDispatcher, CoroutineService coroutineService)
        {
            this.eventDispatcher = eventDispatcher;
            this.coroutineService = coroutineService;
        }

        public override void OnStarted()
        {
            if (delayInSeconds > 0)
            {
                coroutineService.StartCoroutine(Timer());
            }
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(delayInSeconds);
            eventDispatcher.Invoke((IEvent)Activator.CreateInstance(eventType));
        }
    }
}