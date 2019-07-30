using UnityEngine;
using Utils.Core.Flow;

public class DelayRule : Rule
{
    public override bool IsValid => Time.time > timer;
    public override string DisplayName => "Wait " + delay + " sec";

    [SerializeField] private float delay = 0;

    private float timer;

    public override void OnActivate()
    {
        base.OnActivate();
        timer = Time.time + delay;
    }
}
