using System;

namespace Utils.Core.ObjectPooling
{
    public interface IPoolable
    {
        /// <summary>
        /// Is the object in the pool or is it being used?
        /// </summary>
        bool IsBeingUsedByPool { get; }

        /// <summary>
        /// Called when the object is returned to the pool
        /// </summary>
        Action<IPoolable> OnReturnedToPool { get; set; }

        /// <summary>
        /// Reset to initial state after being used from pool
        /// </summary>
        void ResetState();

        /// <summary>
        /// Called when taken out of the pool to be active
        /// </summary>
        void BecomeActive();

        /// <summary>
        /// Called when to be put back into pool of inactive objects
        /// </summary>
        void BecomeInactive();

        /// <summary>
        /// Return this object to the pool, making it inactive.
        /// </summary>
        void ReturnToPool();

        /// <summary>
        /// Destroy object
        /// Not called Destroy in case of MonoBehaviour 
        /// </summary>
        void DestroyObject();
    }
}