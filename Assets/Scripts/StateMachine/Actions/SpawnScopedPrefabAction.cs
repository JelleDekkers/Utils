using UnityEngine;
using Utils.Core.Flow;
using Utils.Core.Injection;

public class SpawnScopedPrefabAction : StateAction
{
    [SerializeField] private GameObject prefab = null;

    private DependencyInjector injector;
    private ScopedGameObjectManager scopedGameObjectManager;

    public void InjectDependencies(DependencyInjector injector, ScopedGameObjectManager scopedGameObjectManager)
    {
        this.injector = injector;
        this.scopedGameObjectManager = scopedGameObjectManager;
    }

    public override void OnStarting()
    {
        GameObject instance = scopedGameObjectManager.AddScope(prefab, this, out bool isNew);

        if(isNew)
        {
            injector.InjectGameObject(instance);
        }
    }

    public override void OnStopping()
    {
        scopedGameObjectManager.RemoveScope(prefab, this);
    }
}
