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
            bool wasActive = prefab.activeSelf;
            prefab.SetActive(false);

            instance = injector.InstantiateGameObject(prefab);

            prefab.SetActive(wasActive);
            instance.SetActive(wasActive);
        }
    }
}