using UnityEngine;
using Utils.Core.Flow;
using Utils.Core.Services;

public class UpdatableAction : StateAction
{
#pragma warning disable CS0649
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private float rotateSpeed;
#pragma warning restore CS0649

    private GameObject cubeInstance;

    public override void OnEnter()
    {
        cubeInstance = Instantiate(cubePrefab);
    }

    public override void Update()
    {
        cubeInstance.transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
    }

    public override void OnExit()
    {
        Destroy(cubeInstance);
    }
}
