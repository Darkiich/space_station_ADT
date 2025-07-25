using Content.Server.Temperature.Systems;

namespace Content.Server.ADT.Temperature;

/// <summary>
/// CTurn on and turn off AmbientSound when Flammable OnFire os changed
/// </summary>
[RegisterComponent, Access(typeof(EntityHeaterSystem))]
public sealed partial class ADTFlammableAmbientSoundComponent : Component
{
}

/// <summary>
/// Raised whenever an FlammableComponen OnFire is Changed
/// </summary>
[ByRefEvent]
public readonly record struct OnFireChangedEvent(bool OnFire)
{
    public readonly bool OnFire = OnFire;
}
