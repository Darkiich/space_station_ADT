using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using Robust.Client.AutoGenerated;
using Content.Shared.ADT.ModSuits;
using Robust.Client.UserInterface.XAML;
using Robust.Client.Graphics;
using Content.Shared.Access.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client.ADT.Modsuits.UI;

[GenerateTypedNameReferences]
public sealed partial class ModSuitMenu : FancyWindow
{

    [Dependency] private readonly IEntityManager _ent = default!;
    private readonly ModSuitSystem _modsuit = default!;
    private readonly SpriteSystem spriteSystem = default!;
    private EntityUid _mod;

    public event Action<EntityUid>? OnRemoveButtonPressed;
    public event Action<EntityUid>? OnActivateButtonPressed;
    public event Action<EntityUid>? OnDeactivateButtonPressed;

    public ModSuitMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);
        _modsuit = _ent.System<ModSuitSystem>();
        spriteSystem = _ent.System<SpriteSystem>();

    }

    public void SetEntity(EntityUid uid)
    {
        MechView.SetEntity(uid);
        _mod = uid;
    }

    public void UpdateModStats()
    {
        if (!_ent.TryGetComponent<ModSuitComponent>(_mod, out var modComp))
            return;

        ModComplex.Text = Loc.GetString("mod-module-space", ("complexity", modComp.CurrentComplexity), ("maxcomplexity", modComp.MaxComplexity)) + Environment.NewLine +
        Loc.GetString("mod-energy-waste", ("energy", modComp.ModEnergyBaseUsing));
        var styleBox = new StyleBoxFlat
        {
            BackgroundColor = modComp.BackpanelsColor
        };
        UsernamePanel.PanelOverride = styleBox;
        ComplexityPanel.PanelOverride = styleBox;
        StatePanel.PanelOverride = styleBox;
        ScrollPanel.PanelOverride = styleBox;
        BackTexture.Texture = spriteSystem.Frame0(new SpriteSpecifier.Texture(new(modComp.BackgroundPath)));


        LockButton.Text = modComp.UserName != null ? Loc.GetString("mod-lock") : Loc.GetString("mod-locked");
        ModUsername.Text = modComp.UserName != null ? Loc.GetString("mod-user") + modComp.UserName : Loc.GetString("mod-no-user");
        switch (_modsuit.GetAttachedToggleStatus(_mod, modComp))
        {
            case ModSuitAttachedStatus.NoneToggled:
                ModState.ModulateSelfOverride = new Color(0.86f, 0.22f, 0.22f, 0.7f);
                ModState.Text = Loc.GetString("mod-none-toggled");
                break;
            case ModSuitAttachedStatus.PartlyToggled:
                ModState.ModulateSelfOverride = new Color(0.95f, 0.78f, 0.25f, 1f);
                ModState.Text = Loc.GetString("mod-partly-toggled");
                break;
            case ModSuitAttachedStatus.AllToggled:
                ModState.ModulateSelfOverride = new Color(0.35f, 0.84f, 0.33f, 1f);
                ModState.Text = Loc.GetString("mod-all-toggled");
                break;
        }
    }

    public void UpdateModuleView(ModBoundUiState state)
    {
        if (!_ent.TryGetComponent<ModSuitComponent>(_mod, out var modComp))
            return;

        EquipmentControlContainer.RemoveAllChildren();
        var list = state.EquipmentStates.Keys;
        foreach (var item in list)
        {
            var ent = _ent.GetEntity(item);
            if (!_ent.TryGetComponent<ModSuitModComponent>(ent, out var modulecomp))
                continue;
            if (!_ent.TryGetComponent<MetaDataComponent>(ent, out var metaData))
                continue;

            var uicomp = _ent.GetComponentOrNull<UIFragmentComponent>(ent);
            var ui = uicomp?.Ui?.GetUIFragmentRoot();


            var control = new ModuleControl(ent, metaData.EntityName, ui);

            control.EjectButton.ModulateSelfOverride = new Color(0.18f, 0.25f, 0.35f, 1f);
            control.EjectButton.Text = Loc.GetString("mod-eject");
            if (modulecomp.Active)
            {
                control.ActivateButton.Text = Loc.GetString("mod-activate-nonactive");
                control.DeactivateButton.Text = Loc.GetString("mod-deactivate-active");
                control.ActivateButton.ModulateSelfOverride = new Color(0.04f, 0.06f, 0.10f, 1f);
                control.DeactivateButton.ModulateSelfOverride = new Color(0.18f, 0.25f, 0.35f, 1f);
            }
            else
            {
                control.ActivateButton.Text = Loc.GetString("mod-activate-active");
                control.DeactivateButton.Text = Loc.GetString("mod-deactivate-nonactive");
                control.ActivateButton.ModulateSelfOverride = new Color(0.18f, 0.25f, 0.35f, 1f);
                control.DeactivateButton.ModulateSelfOverride = new Color(0.04f, 0.06f, 0.10f, 1f);
            }
            control.OnRemoveButtonPressed += () => OnRemoveButtonPressed?.Invoke(ent);
            control.OnActivateButtonPressed += () => OnActivateButtonPressed?.Invoke(ent);
            control.OnDeactivateButtonPressed += () => OnDeactivateButtonPressed?.Invoke(ent);
            EquipmentControlContainer.AddChild(control);
        }
    }
}

