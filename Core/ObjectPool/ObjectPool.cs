using System;
using System.Collections.Generic;

namespace Utils.Core.ObjectPooling
{
    public abstract class ObjectPool<T> : IDisposable where T : IPoolable
    {
        public int CurrentPoolSize { get { return inactiveObjects.Count + usedObjects.Count; } }
        public int InitialPoolSize { get; private set; }

        protected List<IPoolable> inactiveObjects = new List<IPoolable>();
        protected List<IPoolable> usedObjects = new List<IPoolable>();

        private bool cleanupOverFlow = false;

        public ObjectPool(int size, bool cleanupOverFlow = true)
        {
            InitialPoolSize = size;
            this.cleanupOverFlow = cleanupOverFlow;
        }

        /// <summary>
        /// Get the next poolable object from <see cref="inactiveObjects"/>, creates a new one if none are available
        /// </summary>
        /// <returns></returns>
        public virtual T GetNextPoolableObject()
        {
            T instance;
            if (inactiveObjects.Count > 0)
            {
                instance = (T)inactiveObjects[0];
                inactiveObjects.RemoveAt(0);
            }
            else
            {
                instance = CreateNewPoolableObject();
            }

            instance.ResetState();
            instance.BecomeActive();
            usedObjects.Add(instance);
            return instance;
        }

        public void FillPool()
        {
            FillPool(InitialPoolSize);
        }

        public void FillPool(int size)
        {
            while (inactiveObjects.Count + usedObjects.Count < size)
                inactiveObjects.Add(CreateNewPoolableObject());
        }

        protected abstract T InstantiateNewPoolableObject();

        private T CreateNewPoolableObject()
        {
            T instance = InstantiateNewPoolableObject();
            instance.BecomeInactive();
            instance.OnReturnedToPool += OnReturnToPoolCallback;

            return instance;
        }

        public virtual void DeactivateAllUsedObjects()
        {
            foreach (IPoolable item in usedObjects)
                item.ReturnToPool();
        }

        /// <summary>
        /// Callback used for when a used poolable object needs to be inactivated and go back into the pool
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnReturnToPoolCallback(IPoolable obj)
        {
            if (inactiveObjects.Contains(obj))
                return;

            usedObjects.Remove(obj);
			if (CurrentPoolSize > InitialPoolSize && cleanupOverFlow)
			{
				DestroyObject(obj);
				return;
			}

			obj.BecomeInactive();
            inactiveObjects.Add(obj);
        }

        protected virtual void DestroyObject(IPoolable obj)
        {
            obj.OnReturnedToPool -= OnReturnToPoolCallback;
            obj.DestroyObject();
        }

        public virtual void Dispose()
        {
            foreach (IPoolable obj in inactiveObjects)
                DestroyObject(obj);

            foreach (IPoolable obj in usedObjects)
                DestroyObject(obj);

            inactiveObjects.Clear();
            usedObjects.Clear();
        }
    }
}