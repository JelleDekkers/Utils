using UnityEngine;
using Utils.Core.Flow;

public class ValueIsHigherThanRule : Rule
{
    public override string DisplayName => string.Format("{0} > {1}", value, targetValue);
    public override bool IsValid { get { return value > targetValue; } }

#pragma warning disable CS0649
    [SerializeField] private int value;
    [SerializeField] private int targetValue;
#pragma warning restore CS0649
}
