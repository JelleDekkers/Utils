using UnityEngine;
using Utils.Core.Flow.Services;
using Utils.Core.Injection;

namespace Utils.Core.Flow.DefaultActions
{
    /// <summary>
    /// Spawns a prefab within a scope, meaning that if the next State also has this StateAction and references the same prefab, it will not destroy the prefab instance
    /// </summary>
    public class SpawnScopedPrefabAction : SpawnPrefabAction
    {
        protected ScopedGameObjectService scopedGameObjectManager;

        public void InjectDependencies(DependencyInjector injector, ScopedGameObjectService scopedGameObjectManager)
        {
            Debug.Log("child inj deps");

            this.injector = injector;
            this.scopedGameObjectManager = scopedGameObjectManager;
        }

		protected override void SpawnPrefab()
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