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
        protected ScopedGameObjectService scopedGameObjectManager;

        public void InjectDependencies(DependencyInjector injector, ScopedGameObjectService scopedGameObjectManager)
        {
            this.injector = injector;
            this.scopedGameObjectManager = scopedGameObjectManager;
        }

        public override void OnStarted()
        {
			SpawnScopedPrefab();
        }

		protected virtual void SpawnScopedPrefab()
		{
			// disable so that the dependencyinjector gets called before Awake, Start and OnEnable
			bool wasActive = prefab.activeSelf;
			prefab.SetActive(false);

			instance = scopedGameObjectManager.AddScope(prefab, this, out bool isNew);

			if (isNew)
			{
				injector.InjectGameObject(instance);
			}

			prefab.SetActive(wasActive);
			instance.SetActive(wasActive);
		}

        public override void OnStopping()
        {
            scopedGameObjectManager.RemoveScope(prefab, this);
        }
    }
}