using UnityEngine;
using Utils.Core.Flow;

public class KeyPressedRule : Rule
{
    public override string DisplayName => "Pressed " + key.ToString();
    public override bool IsValid => Input.GetKeyDown(key);

#pragma warning disable CS0649
    [SerializeField] private KeyCode key;
#pragma warning restore CS0649
}
