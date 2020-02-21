using UnityEngine;
using Utils.Core.Injection;

namespace Utils.Core.Flow.DefaultActions
{
    public class SpawnPrefabAction : StateAction
    {
        [SerializeField] protected GameObject prefab = null;

        protected GameObject instance;
        protected DependencyInjector injector;

        public void InjectDependencies(DependencyInjector injector)
        {
            this.injector = injector;
        }

        public override void OnStarted()
        {
            SpawnPrefab();
        }

        protected virtual void SpawnPrefab()
        {
            instance = injector.InstantiateGameObject(prefab);
        }
    }
}