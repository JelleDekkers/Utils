using System;
using UnityEngine;

namespace Utils.Core.Services
{
    public class UnityCallbackService : MonoBehaviour, IService, IDisposable
    {
        public event Action GUIEvent = delegate { };
        public event Action<bool> ApplicationFocusEvent = delegate { };
        public event Action<bool> ApplicationPauseEvent = delegate { };
        public event Action ApplicationQuitEvent = delegate { };
        public event Action RenderObjectEvent = delegate { };
        public event Action DrawGizmosEvent = delegate { };
        public event Action UpdateEvent = delegate { };
        public event Action FixedUpdateEvent = delegate { };
        public event Action LateUpdateEvent = delegate { };

        private const string OBJECT_NAME = "UnityCallbackService";

        private void Awake()
        {
            gameObject.name = OBJECT_NAME;
            DontDestroyOnLoad(this);
        }

        protected virtual void OnGUI()
        {
            GUIEvent();
        }

        protected virtual void Update()
        {
            UpdateEvent();
        }

        protected virtual void FixedUpdate()
        {
            FixedUpdateEvent();
        }

        protected virtual void LateUpdate()
        {
            LateUpdateEvent();
        }

        protected virtual void OnApplicationFocus(bool focusStatus)
        {
            ApplicationFocusEvent(focusStatus);
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            ApplicationPauseEvent(pauseStatus);
        }

        protected virtual void OnApplicationQuit()
        {
            ApplicationQuitEvent();
        }

        protected virtual void OnRenderObject()
        {
            RenderObjectEvent();
        }

        protected virtual void OnDrawGizmos()
        {
            DrawGizmosEvent();
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}