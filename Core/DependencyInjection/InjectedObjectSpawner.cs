using UnityEngine;
using Utils.Core.Events;

namespace Utils.Core.Injection
{
    public class InjectedObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefab = null;

        protected virtual void Awake()
        {
            Spawnprefab();
        }

        protected virtual void Spawnprefab()
        {
            DependencyInjector injector = new DependencyInjector(prefab.name);
            EventDispatcher eventDispatcher = new EventDispatcher(prefab.name);
            injector.RegisterInstance<EventDispatcher>(eventDispatcher);

            GameObject instance = injector.InstantiateGameObject(prefab);
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
        }
    }
}