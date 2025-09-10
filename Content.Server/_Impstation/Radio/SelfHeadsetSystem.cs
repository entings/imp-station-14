using Content.Server.Emp;
using Content.Server._Impstation.Radio.Components;
using Content.Server.Radio.Components;
using Robust.Shared.Containers;
using Content.Shared.Examine;
using Content.Shared.Radio;
using Content.Shared.Radio.Components;

namespace Content.Server._Impstation.Radio;

public sealed class SelfHeadsetSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _container = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SelfHeadsetComponent, EncryptionChannelsChangedEvent>(OnKeysChanged);
        SubscribeLocalEvent<SelfHeadsetComponent, EmpPulseEvent>(OnEmpPulse);

    }

    /// <summary>
    /// Used to give an entity access to radio channels when an Encryption Key is inserted, without the need for a headset.
    /// </summary>

    public void UpdateChannels(Entity<SelfHeadsetComponent> ent)
    {
        if (!ent.Comp.Initialized)
            return;

        ent.Comp.Channels.Clear();
        ent.Comp.DefaultChannel = null;

        foreach (var comp in ent.Comp.KeyContainer.ContainedEntities)
        {
            if (TryComp<SelfHeadsetComponent>(ent, out var key))
            {
                ent.Comp.Channels.UnionWith(key.Channels);
                ent.Comp.DefaultChannel ??= key.DefaultChannel;
            }
        }

        RaiseLocalEvent(ent, new EncryptionChannelsChangedEvent(ent));
    }

    private void OnStartup(Entity<SelfHeadsetComponent> ent, ComponentStartup args)
    {
        ent.Comp.KeyContainer = _container.EnsureContainer<Container>(ent, SelfHeadsetComponent.KeyContainerName);
        UpdateChannels(ent);
    }

    private void OnExamine(Entity<SelfHeadsetComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.KeyContainer.ContainedEntities.Count == 0)
            args.PushMarkup(Loc.GetString("encryption-keys-no-keys"));

        if (ent.Comp.KeyContainer.ContainedEntities.Count > 0)
            args.PushMarkup(Loc.GetString("encryption-keys-some-keys"));
        return;
    }

    private void OnKeysChanged(Entity<SelfHeadsetComponent> ent, ref EncryptionChannelsChangedEvent args)
    {
        UpdateRadioChannels(ent, args.Component);
    }

    private void UpdateRadioChannels(Entity<SelfHeadsetComponent> ent, EncryptionKeyHolderComponent? keyHolder = null)
    {
        {
            if (!Resolve(ent, ref keyHolder))
                return;

            if (keyHolder.Channels.Count == 0)
                RemCompDeferred<ActiveRadioComponent>(ent);
            else
                EnsureComp<ActiveRadioComponent>(ent).Channels = new(keyHolder.Channels);

            if (!Resolve(ent, ref keyHolder))
                return;

            if (keyHolder.Channels.Count == 0)
                RemCompDeferred<IntrinsicRadioReceiverComponent>(ent);
            else
                EnsureComp<IntrinsicRadioReceiverComponent>(ent);

            if (!Resolve(ent, ref keyHolder))
                return;

            if (keyHolder.Channels.Count == 0)
                RemCompDeferred<IntrinsicRadioTransmitterComponent>(ent);
            else
                EnsureComp<IntrinsicRadioTransmitterComponent>(ent).Channels = new(keyHolder.Channels);
        }
    }

    /// <summary>
    /// Disables radio when hit by an EMP.
    /// </summary>
    private void OnEmpPulse(Entity<SelfHeadsetComponent> ent, ref EmpPulseEvent args)
    {
        {
            args.Affected = true;
            args.Disabled = true;
        }
    }

}
