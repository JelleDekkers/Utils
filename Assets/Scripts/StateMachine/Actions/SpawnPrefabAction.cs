using StateMachine;
using UnityEngine;

public class SpawnPrefabAction : StateAction
{
    public override string DisplayName => "Spawn " + prefab?.name;

    [SerializeField]
    private GameObject prefab = null;

    public override void OnStateStart()
    {
        Instantiate(prefab);
    }
}
