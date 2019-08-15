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

    public StateMachineLogic Logic { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(this);
        Logic = new StateMachineLogic(stateMachineData);

        Logic.LayerChangedEvent += OnStateMachineChangedEvent;
    }

    private void OnDestroy()
    {
        Logic.LayerChangedEvent -= OnStateMachineChangedEvent;
    }

    private void OnStateMachineChangedEvent(StateMachineLayer from, StateMachineLayer to)
    {
        stateMachineData = to.Data;
    }

    private void Update()
    {
        Logic.Update();
    }
}
