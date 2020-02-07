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
        [SerializeField] private bool global = false;
        [Tooltip("Optional delay in seconds")]
        [SerializeField] private float delayInSeconds = 0;

        private EventDispatcher eventDispatcher;
        private CoroutineService coroutineService;
        private GlobalEventDispatcher globalEventDispatcher;

        public void InjectDependencies(EventDispatcher eventDispatcher, CoroutineService coroutineService, GlobalEventDispatcher globalEventDispatcher)
        {
            this.eventDispatcher = eventDispatcher;
            this.coroutineService = coroutineService;
            this.globalEventDispatcher = globalEventDispatcher;
        }

        public override void OnStarted()
        {
            if (delayInSeconds > 0)
            {
                coroutineService.StartCoroutine(Timer());
            }
            else
            {
                InvokeEvent();
            }
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(delayInSeconds);
            InvokeEvent();
        }

        private void InvokeEvent()
        {
            eventDispatcher.Invoke((IEvent)Activator.CreateInstance(eventType));

            if (global)
            {
                globalEventDispatcher.Invoke((IEvent)Activator.CreateInstance(eventType));
            }
        }
    }
}