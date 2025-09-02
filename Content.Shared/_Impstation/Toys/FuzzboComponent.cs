using Robust.Shared.Audio;
using Robust.Shared.Containers;

namespace Content.Shared._Impstation.Toys;

/// <summary>
/// Hides radio channels on examine, allows for emag.
/// </summary>
[RegisterComponent]
public sealed partial class FuzzboComponent : Component

{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("keyInsertionSound")]
    public SoundSpecifier KeyInsertionSound = new SoundPathSpecifier("eating");

    [ViewVariables]
    public Container KeyContainer = default!;
    public const string KeyContainerName = "key_slots";

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("keySlots")]
    public int KeySlots = 2;
}
