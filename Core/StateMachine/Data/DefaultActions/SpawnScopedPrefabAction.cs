using UnityEngine;
using Utils.Core.Flow.Services;
using Utils.Core.Injection;

namespace Utils.Core.Flow.DefaultActions
{
    /// <summary>
    /// Spawns a prefab within a scope, meaning that if the next State also has this StateAction and references the same prefab, it will not destroy the prefab instance
    /// </summary>
    public class SpawnScopedPrefabAction : StateAction
    {
        [SerializeField] private GameObject prefab = null;

		protected GameObject instance;

        protected DependencyInjector injector;
        protected ScopedGameObjectManager scopedGameObjectManager;

        public void InjectDependencies(DependencyInjector injector, ScopedGameObjectManager scopedGameObjectManager)
        {
            this.injector = injector;
            this.scopedGameObjectManager = scopedGameObjectManager;
        }

        public override void OnStarting()
        {
			SpawnScopedPrefab();
        }

		protected virtual void SpawnScopedPrefab()
		{
			instance = scopedGameObjectManager.AddScope(prefab, this, out bool isNew);

			if (isNew)
			{
				injector.InjectGameObject(instance);
			}
		}

        public override void OnStopping()
        {
            scopedGameObjectManager.RemoveScope(prefab, this);
        }
    }
}