using System;

namespace TestModelDriven.Framework;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class StateCollectionAttribute : Attribute
{
    public StateOwnership Ownership { get; }
    public bool IsOwner => Ownership == StateOwnership.Owner;

    public StateCollectionAttribute(StateOwnership ownership = StateOwnership.Owner)
    {
        Ownership = ownership;
    }
}