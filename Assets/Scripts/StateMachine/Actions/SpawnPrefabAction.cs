using StateMachine;
using UnityEngine;

public class SpawnPrefabAction : StateAction
{
    public override string DisplayName => "Spawn " + prefab?.name;

    [SerializeField]
    private GameObject prefab = null;

    private GameObject instance;

    public override void Start()
    {
        Debug.Log("start() SpawnPrefab");
        instance = Instantiate(prefab);
    }

    public override void Stop()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
    }
}
