using System;

namespace TestModelDriven.Framework;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class StateAttribute : Attribute
{
    public StateOwnership Ownership { get; }

    public StateAttribute()
    {
        Ownership = StateOwnership.Owner;
    }

    public StateAttribute(StateOwnership ownership)
    {
        Ownership = ownership;
    }
}