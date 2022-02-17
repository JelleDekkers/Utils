using UnityEngine;

namespace Utils.Core.FPSCounter
{
    /// <summary>
    /// Simple frames per second counter. 
    /// First create either an instance in the editor or call <see cref="CreateCounterInstance"/>, then simply retrieve the fps with <see cref="CurrentFPS"/>
    /// </summary>
    public class FPSCounter : MonoBehaviour
    {
        public static float CurrentFPS { get; protected set; }
        public static FPSCounter CounterInstance { get; protected set; }

        public const float UPDATE_INTERVAL = 0.5f;

        protected float frameTime;
        protected uint frameCount;

        /// <summary>
        /// Creates a gameobject required for calculating fps
        /// </summary>
        public static FPSCounter CreateCounterInstance()
        {
            FPSCounter counter = new GameObject("FPS Counter", typeof(FPSCounter)).GetComponent<FPSCounter>();
            DontDestroyOnLoad(counter);
            return counter;
        }

        protected virtual void Awake()
        {
            if (CounterInstance != null)
                Destroy(CounterInstance);
            CounterInstance = this;
        }

        protected virtual void Update()
        {
            frameCount++;
            frameTime += Time.unscaledDeltaTime;

            if (frameTime > UPDATE_INTERVAL)
            {
                CurrentFPS = frameCount / frameTime;

                frameCount = 0;
                frameTime = 0;
            }
        }
    }
}