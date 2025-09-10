using Robust.Shared.Prototypes;
using Content.Shared.Radio;
using Content.Shared.Chat;
using Robust.Shared.Containers;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server._Impstation.Radio.Components;

/// <summary>
/// Gives the user access to a given channel via encryption key without the need for a headset.
/// </summary>
[RegisterComponent]
public sealed partial class SelfHeadsetComponent : Component
{
    /// <summary>
    /// The radio channel(s) to grant access to.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<RadioChannelPrototype>> RadioChannels = new();

    /// <summary>
    /// The radio channels that have been added via encryption key to a user's ActiveRadioComponent.
    /// Used to track which channels were successfully added (not already in user)
    /// </summary>
    public HashSet<ProtoId<RadioChannelPrototype>> ActiveAddedChannels = new();

    /// <summary>
    /// The radio channels that have been added via encryption key to a user's IntrinsicRadioTransmitterComponent.
    /// Used to track which channels were successfully added (not already in user)
    /// </summary>
    [DataField]
    public HashSet<ProtoId<RadioChannelPrototype>> TransmitterAddedChannels = new();

    [DataField("channels", customTypeSerializer: typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
    public HashSet<string> Channels = new();

    /// <summary>
    ///     This is the channel that will be used when using the default/department prefix (<see cref="SharedChatSystem.DefaultChannelKey"/>).
    /// </summary>
    [DataField("defaultChannel", customTypeSerializer: typeof(PrototypeIdSerializer<RadioChannelPrototype>))]
    public string? DefaultChannel;

    [ViewVariables]
    public Container KeyContainer = default!;
    public const string KeyContainerName = "key_slots";
}
