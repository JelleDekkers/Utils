using UnityEngine;
using Utils.Core.Flow;

/// <summary>
/// Monobehaviour for using a <see cref="Flow.StateMachineData"/> in the scene. 
/// Has DontDestroyOnLoad called on this gameObject.
/// </summary>
public class StateMachineExecutor : MonoBehaviour
{
    public StateMachineData StateMachineData => stateMachineData;
    [SerializeField] protected StateMachineData stateMachineData;

    public StateMachineManager Manager { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(this);
        Manager = new StateMachineManager(stateMachineData);
        Manager.LayerChangedEvent += OnLayerChangedEvent;
    }

    private void OnDestroy()
    {
        Manager.LayerChangedEvent -= OnLayerChangedEvent;
    }

    private void OnLayerChangedEvent(StateMachineLayer from, StateMachineLayer to)
    {
        stateMachineData = to.Data;
    }

    private void Update()
    {
        Manager.Update();
    }
}
