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

        private DependencyInjector injector;
        private ScopedGameObjectManager scopedGameObjectManager;

        private GameObject instance;

        public void InjectDependencies(DependencyInjector injector, ScopedGameObjectManager scopedGameObjectManager)
        {
            this.injector = injector;
            this.scopedGameObjectManager = scopedGameObjectManager;
        }

        public override void OnStarting()
        {
            GameObject instance = scopedGameObjectManager.AddScope(prefab, this, out bool isNew);

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