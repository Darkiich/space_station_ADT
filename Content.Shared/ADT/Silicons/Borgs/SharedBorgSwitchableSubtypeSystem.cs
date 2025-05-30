using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.ADT.Silicons.Borgs;

public abstract class SharedBorgSwitchableSubtypeSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BorgSwitchableSubtypeComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<BorgSwitchableSubtypeComponent, BorgSelectSubtypeMessage>(OnSubtypeSelection);
    }

    private void OnComponentInit(Entity<BorgSwitchableSubtypeComponent> ent, ref ComponentInit args)
    {
        if (ent.Comp.BorgSubtype == null)
            return;

        SetAppearanceFromSubtype(ent, ent.Comp.BorgSubtype.Value);
    }
    private void OnSubtypeSelection(Entity<BorgSwitchableSubtypeComponent> ent, ref BorgSelectSubtypeMessage args)
    {
        SetSubtype(ent, args.Subtype);
    }

    protected virtual void SetAppearanceFromSubtype(Entity<BorgSwitchableSubtypeComponent> ent, ProtoId<BorgSubtypePrototype> subtype)
    {
    }

    public void SetSubtype(Entity<BorgSwitchableSubtypeComponent> ent, ProtoId<BorgSubtypePrototype> subtype)
    {
        ent.Comp.BorgSubtype = subtype;
        var ev = new BorgSubtypeChangedEvent(subtype);
        RaiseLocalEvent(ent, ref ev);
    }
}

[ByRefEvent]
public readonly record struct BorgSubtypeChangedEvent(ProtoId<BorgSubtypePrototype> Subtype);

[Serializable, NetSerializable]
public sealed class BorgSelectSubtypeMessage(ProtoId<BorgSubtypePrototype> subtype) : BoundUserInterfaceMessage
{
    public ProtoId<BorgSubtypePrototype> Subtype = subtype;
}

[Serializable, NetSerializable]
public enum BorgSwitchableSubtypeUiKey : byte
{
    Key
}
