using UnityEngine;
using Utils.Core.Flow;

public class TitleTestRule : Rule
{
    public override string DisplayName => title;

#pragma warning disable CS0649
    [SerializeField] private string title;
#pragma warning restore CS0649
}
