using Content.Server.Speech.Components;
using Content.Shared.CombatMode;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared._Impstation.Toys;
using Content.Shared.Interaction;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;


namespace Content.Server._Impstation.Toys;

public sealed class FuzzboSystem : EntitySystem
{

    [Dependency] private readonly EmagSystem _emag = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FuzzboComponent, GotEmaggedEvent>(OnEmagged);
        SubscribeLocalEvent<FuzzboComponent, InteractUsingEvent>(OnInteractUsing);
    }

    /// <summary>
    /// Makes an eating noise play when keys are inserted, emag rules.
    /// </summary>
    private void OnInteractUsing(Entity<FuzzboComponent> ent, ref InteractUsingEvent args)
    {
        if (HasComp<FuzzboComponent>(args.Used))
        {
            args.Handled = true;
            TryInsertKey(ent, ref args);
        }
    }

    private void TryInsertKey(Entity<FuzzboComponent> ent, ref InteractUsingEvent args)
    {
        if (_container.Insert(args.Used, ent.Comp.KeyContainer))
        {
            _audio.PlayPredicted(ent.Comp.KeyInsertionSound, args.Target, args.User);
            args.Handled = true;
            return;
        }
    }

    /// <summary>
    /// Allows the player inhabiting the ghost role to activate Harm Mode at will, removes Relentless Positivity accent.
    /// </summary>
    private void OnEmagged(Entity<FuzzboComponent> ent, ref GotEmaggedEvent args)
    {
        {
            if (!_emag.CompareFlag(args.Type, EmagType.Interaction))
                return;

            if (_emag.CheckFlag(ent, EmagType.Interaction))
                return;

            args.Handled = true;

            EnsureComp<CombatModeComponent>(ent);

            RemCompDeferred<RelentlessPositivityComponent>(ent);
        }
    }

}
