﻿using StateMachine;
using UnityEngine;
using Utils.Core.Injection;

public class SpawnPrefabAction : StateAction
{
    public override string DisplayName => "Spawn " + prefab?.name;

    [SerializeField] private GameObject prefab = null;
#pragma warning disable CS0649
    [SerializeField] private Vector3 position;
    [SerializeField] private string instanceName;
    [SerializeField] private bool cleanUpOnStop;
#pragma warning disable CS0649

    private GameObject instance;
    private DependencyInjector injector;

    public void InjectDependencies(DependencyInjector injector)
    {
        this.injector = injector;
    }

    public override void Start()
    {
        instance = Instantiate(prefab, position, Quaternion.identity);
        injector.InjectGameObject(instance);

        if(instanceName != string.Empty)
        {
            instance.name = instanceName;
        }
    }

    public override void Stop()
    {
        if (cleanUpOnStop && instance != null)
        {
            Destroy(instance);
        }
    }
}
