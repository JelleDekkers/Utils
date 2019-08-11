using System.Collections;
using UnityEngine;
using Utils.Core.Flow;
using Utils.Core.Services;

public class DelayRule : Rule
{
    public override bool IsValid => isValid;
    private bool isValid;

    public override string DisplayName => "Wait " + delay + " sec";

    [SerializeField] private float delay = 0;

    private CoroutineService coroutineService;
    private IEnumerator timerCoroutine;

    public void InjectDependencies(CoroutineService coroutineService)
    {
        this.coroutineService = coroutineService;
    }

    public override void OnActivate()
    {
        timerCoroutine = Timer(Time.time + delay);
        coroutineService.StartCoroutine(timerCoroutine);
    }

    public override void OnDeactivate()
    {
        coroutineService.StopCoroutine(timerCoroutine);
    }

    private IEnumerator Timer(float timeTarget)
    {
        while (Time.time <= timeTarget)
        {
            yield return null;
        }

        isValid = true;
    }
}
