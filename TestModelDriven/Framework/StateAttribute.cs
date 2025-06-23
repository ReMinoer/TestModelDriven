using System;

namespace TestModelDriven.Framework;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class StateAttribute : Attribute
{
    public string AsyncSetterName { get; }
    public StateOwnership Ownership { get; }
    public bool IsOwner => Ownership == StateOwnership.Owner;

    public StateAttribute(string asyncSetterName, StateOwnership ownership = StateOwnership.Owner)
    {
        AsyncSetterName = asyncSetterName;
        Ownership = ownership;
    }
}