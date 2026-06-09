# DSPCore

DSPCore is a new common framework standard for Dyson Sphere Program mods.

## Goals

- Provide one shared API for common mod infrastructure.
- Replace scattered requirements such as LDBTool, DSPModSave, CommonAPI, BuildBarTool, ErrorAnalyzer-style diagnostics, achievement policy aggregation, icon registration, and legacy compatibility shims.
- Keep legacy API namespaces available as `[Obsolete]` compatibility shims, so existing mods can run first and migrate later.

## Package

- Thunderstore package name: `MengLei-DSPCore`
- DLL name: `DSPCore.dll`
- New namespace: `DSPCore`
- Chinese documentation: `README.md`

## Project Layout

- `DSPCore/`: main BepInEx plugin project, split into `Authoring/` capabilities and `Systems/` integrations.
- `DSPCore/Authoring/`: capabilities mod authors call directly, such as Core, DataPhases, ProtoAccess, Items, Recipes, Techs, Tutorials, Tabs, BuildBar, Resources, Icons, GameEnums, KeyBinds, Saves, Achievements, Components, Planets, Blueprints, Models, Options, Multiplayer, Networks, Galaxy, and UI.
- `DSPCore/Systems/`: runtime handling for author declarations, such as lifecycle, proto pipeline, tab projection, picker surfaces, quick bar projection, resource loading, save bridge, achievement policy, and error window handling.
- `DSPCore.Preloader/`: BepInEx patchers project for pre-load game assembly patches.
- `DSPCore.Packaging/`: Thunderstore packaging project.

## First Version Scope

- P0/P1 author-facing capabilities: feature lifecycle, data phases, proto access, item/recipe/tech/tutorial registration, build bar placement, resources, icons, localization, tabs, game enum extensions, key binds, saves, achievement policies, and a common UI framework.
- Legacy compatibility shims for `xiaoye97.LDBTool`, `crecheng.DSPModSave`, `CommonAPI`, and `BuildBarTool`; compatibility code lives under the owning `Authoring/<Capability>/Compat/` directory instead of a centralized `Legacy/` directory.
- Bilingual XML summaries for public APIs.

The current version includes P0/P1 and first P2/P3 runtime bridges: BepInEx/Harmony startup, preloader field and reserved enum-slot injection, proto insertion, multi-row build bar binding, player overrides, RebindBuildBar configuration import, resource/icon loading, tabs for item/recipe/replicator/signal/tag-icon surfaces, picker popups and live filtering, custom recipe type pre-selection filtering, custom item type marker entries, key callbacks, DSPCore sidecar saves, legacy DSPModSave handler bridging, entity component lifecycle, planet/star/galaxy lifecycle, blueprint parameter blocks, model and prefab cloning, option binding, optional multiplayer soft bridge, network query adapters, patch platform, achievement/abnormality/platform policy patches, error reporting, copyable diagnostic text with game/entity context, fatal-window copy/close buttons, localization entries, and common UI window lifecycle forwarding.

## Feature Blocks

P0/P1 blocks are the current implementation target.

- Feature lifecycle: declare capability blocks, dependencies, priority, and initialization, and use `Lifecycle` to register DSPCore started, update, destroyed, and common save-chain callbacks.
- Data phases: `Data`, `DataUpdates`, and `DataFinalFixes`.
- Proto capabilities: DataPhases owns the three phases; ProtoAccess provides second/third phase lookup and mutation of registered data through `ProtoPhaseContext.FindItem(...)` / `FindRecipe(...)` and `data.Access`; Items, Recipes, Techs, and Tutorials own typed proto registration; `ItemProto` / `RecipeProto` can use object-centric short entries to set `GridIndex`, bind icons, and register; ProtoRegistration remains the low-level aggregate and compatibility entry.
- Build bar placement: bind an `ItemProto` or item id to a tab/row/index slot; row 1 writes to the vanilla build bar, row 2+ uses DSPCore extended buttons, and BuildBarTool compatibility entries remain available. Other authoring capabilities, such as item registration, should prefer `ItemProto.SetBuildBar(...)` after they have the item proto; BuildBar does not own proto creation.
- Resources, icons, and localization: register resource roots and translation entries through `ModResources`; one mod can use `ModResources.Pack(...)` to reuse owner, root path, and assembly; register icons through `Icons.FromResources(...)`, `Icons.FromFile(...)`, `Icons.FromEmbedded(...)`, `Icons.FromAssetBundle(...)`, or `Icons.BindToProto(...)`.
- Tabs: authors can declare custom pages, receive a `TabSlot`, and use that slot to generate item/recipe `GridIndex` values. Picker surfaces are DSPCore system implementation.
- Game enums: `GameEnums.RegisterRecipeType(...)` declares custom recipe type restrictions, and `ItemProto.SetCustomItemType()` marks an item with DSPCore's reserved custom item type. Runtime code should use `GameEnums.CustomRecipeTypeValue` / `CustomItemTypeValue` instead of compiling directly against Preloader-injected enum members.
- Saves: `Saves.Auto(...)` automatic schemas, delegate-based simple save handlers, raw `BinaryReader`/`BinaryWriter` handlers, and tagged block helpers.
- Achievement policies: declare policy effects such as Milky Way / leaderboard upload blocking. Error window and error collection belong to DSPCore systems.
- UI framework: window lifecycle helpers, tabbed windows, base controls, declarative grid layout, theme/card helpers, and standard form, list, detail, and status-footer scaffolds; concrete business pages are not included.
- Entity components: use `Components.Register<TComponent>(...)` to attach custom components to entities by item id, model index, or `PrefabDesc`, then forward removal, ticks, and saves; use descriptors for complex construction.
- Planet/star/galaxy systems: use `PlanetSystems.Register<TSystem>(...)`, `GalaxySystems.RegisterStar<TSystem>(...)`, or `RegisterGalaxy<TSystem>(...)` to register systems, create instances for `PlanetFactory`, `StarData`, or `GalaxyData`, and forward lifecycle callbacks; use descriptors for complex construction.
- Blueprint parameters: use `Blueprints.Register(blockId, ownerModGuid, copy, paste, ...)` to register integer-payload tagged blocks so multiple mods do not compete for fixed `BuildingParameters.parameters` slots; use descriptors for complex block handling.
- Models and prefabs: clone existing `ModelProto` entries, configure independent `PrefabDesc` instances, and rebuild model derived caches; prefer `ModelProto.CloneAsModel(...)` when the caller already has the source object, and use `Models.CloneModel(...)` when only the source model index is available.
- Options, multiplayer, and networks: provide `Options.String/Bool/Int/Float/Enum/IntRange/FloatRange` short entries, same-name overloads with `OptionUi` for page, display, in-page order, and Reset button metadata, BepInEx config binding, a DSPCore unified settings window, option page and settings version descriptors, option import/export, Nebula soft detection, packet/host relay/planet data/client save declarations, adapter snapshot/query entries, and the `Networks.Register(...)` short entry for factory network query adapters.
- Patch platform: use `Patches.Register(...)` / `RegisterForPlugin(...)` to centralize conditional patch declarations, required plugin GUID/version checks, disabled reasons, and apply failure reporting; descriptors remain the advanced path.

## Runtime Status

Implemented runtime bridges:

- `DSPCorePlugin` starts from BepInEx and applies Harmony patches.
- `Lifecycle` raises `OnStarted` after DSPCore runtime bridges are assembled, raises `OnUpdate` / `OnDestroyed` during plugin update and destroy, and forwards `OnNewGame`, `OnBeforeSave`, `OnBeforeLoad`, `OnAfterLoad`, and `OnSaveDeleted` from SaveRuntime.
- Proto registration runs Factorio-like `Data`, `DataUpdates`, and `DataFinalFixes` callbacks. Phase contexts provide lookup and mutation access to currently visible protos. Runtime writes the resulting protos around `VFPreload.InvokeOnLoadWorkEnded`; DSPCore rebuilds `ProtoSet` indices and key derived caches after final fixes.
- `BuildBarRegistry.BindQuickBar` maps item ids or `ItemProto` instances to build bar tab/row/index slots; row 1 writes vanilla `UIBuildMenu.protos`, and row 2+ uses DSPCore extended buttons. `BuildBar.SetPlayerOverride(...)` writes a player override layer to the `.dspcore` save, and runtime uses author defaults overlaid with player overrides. When no DSPCore BuildBar save data exists, DSPCore imports row-1 player configuration from RebindBuildBar's `CustomBarBind.cfg`.
- `IconSetRegistry` can load Unity `Resources` sprites, local PNG files, embedded PNG files from loaded assemblies, or `Sprite` / `Texture2D` assets from AssetBundles, cache them, and apply them to target protos. Author-side short entries are `Icons.FromResources(...)`, `Icons.FromFile(...)`, `Icons.FromEmbedded(...)`, `Icons.FromAssetBundle(...)`, and `Icons.BindToProto(...)`. If one mod has a shared resource root, create `ModResources.Pack(...)` first and use the pack icon methods to reduce repeated parameters.
- `TabRegistry` assigns a `TabSlot` for each stable page id and projects custom pages to item picker, recipe picker, replicator, signal picker, and tag-icon picker surfaces through the existing GridIndex category flow.
- `PickerSurfaces` handles item, recipe, and signal picker surfaces. Live grids apply filters, duplicate `GridIndex` fallbacks, and dynamic row/column expansion.
- `GameEnums.RegisterRecipeType(...)` currently marks declared recipes as custom recipe types and hides recipes unsupported by the current assembler before the recipe picker selection; `ItemProto.SetCustomItemType()` marks existing items with DSPCore's reserved custom item type; `RecipeTypes` remains a legacy alias, and `AssemblerComponent.SetRecipe` remains the final guard.
- `KeyBindRegistry` polls registered key bindings and invokes callbacks; the author-side short entry is `KeyBinds.Register(id, ownerModGuid, displayName, defaultKey, callback, ...)`, including simple `Ctrl`/`Alt`/`Shift` modifier combinations. Bindings with `CanOverride=true` enter the DSPCore unified settings window, where players can capture a key or edit text directly, and same-key bindings in the same `ConflictGroup` show a conflict hint. Runtime prefers player config and falls back to the default key when config is empty or invalid.
- `SaveRegistry` writes a `.dspcore` sidecar save file and imports handlers by `CoreLoadOrder`.
- `AchievementPolicyRegistry` aggregates each mod's achievement-disable declaration. Not declaring, or declaring `disableAchievements: false`, does not request disabling. If any mod declares true, DSPCore globally blocks achievement mutation, Milky Way / leaderboard uploads, and platform achievement/metadata calls. If no declaration is true, DSPCore blocks vanilla abnormality checks and keeps achievements available.
- `ErrorWindow` receives Unity fatal/error logs and fatal-window events, and builds copyable diagnostic text with current game state, optional planet/entity context, recent errors, candidate plugin text hits, DSPCore declarations, and a Harmony patch-map overview.
- `ResourceRegistry.RegisterLocalization` is applied to DSP localization keys and language strings. Author-side short entries are `ModResources.Root(...)`, `ModResources.Text(...)`, and `ModResources.Pack(...)`.
- `UiWindowManager` forwards DSPCore window lifecycle through `UIRoot` open, update, and destroy events; mods still create and open concrete windows themselves.
- `Components` creates components after `PlanetFactory.CreateEntityLogicComponents`; parameterless components can be registered through the `Components.Register<TComponent>(...)` short entry. Runtime forwards entity removal, power ticks, factory ticks, and post phases. Component data is stored in `.dspcore`; data for unloaded planet factories is restored after `GameData.GetOrCreateFactory`.
- `Planets` creates planet systems after `GameData.GetOrCreateFactory`; parameterless systems can be registered through the `PlanetSystems.Register<TSystem>(...)` short entry. Runtime forwards local planet rendering, power ticks, factory ticks, and post phases.
- `Blueprints` encodes author parameter blocks at the end of `BuildingParameters` arrays and preserves block IDs across copy, blueprints, paste, and prebuild apply; simple blocks can use `Blueprints.Register(...)` with direct `int[]` payloads.
- `Models` clones `ModelProto` and `PrefabDesc` before final derived cache rebuilds, then rebuilds `ModelProto` indices and `PlanetFactory.PrefabDescByModelIndex`.
- `Options` binds author-declared string options to the DSPCore BepInEx config file and stores option page and settings version descriptors. `String`, `Bool`, `Int`, `Float`, `Enum`, `IntRange`, and `FloatRange` register an option and return the current value; same-name short entries accept `OptionUi` when page, display-name, order, or Reset metadata is needed; `ExportValues` / `ExportText` and `ImportValues` / `ImportText` provide option snapshot import/export; `Options.OpenWindow()` opens the DSPCore-owned unified settings window.
- `Multiplayer` currently detects whether Nebula is loaded, stores packet, host relay, planet data request, and client missing-save declarations, and exposes adapter snapshot/query entries. Actual Nebula sending belongs to a dedicated adapter.
- `Networks` provides the `Register(...)` adapter short entry plus the `TryGetCommonNetwork(...)` and `IsConnectedToNetwork(...)` query surfaces; concrete scanning is supplied by registered adapters.
- `Galaxy` creates star and galaxy systems after galaxy data exists; parameterless systems can be registered through `GalaxySystems.RegisterStar<TSystem>(...)` / `RegisterGalaxy<TSystem>(...)` short entries. Runtime forwards `SpaceSector.GameTick` updates and sidecar saves.
- `PatchRuntime` applies conditional patches declared through `Patches.Register(...)` or `PatchDescriptor` and records disabled reasons or apply failures.

Current runtime limits:

- RebindBuildBar `BuildBarBinds` configuration is imported into DSPCore row-1 player overrides; DSPCore does not take over RebindBuildBar's rebinding UI, hotkeys, or later config writes.
- Tab projection currently covers vanilla `UIItemPicker`, `UIRecipePicker`, `UIReplicatorWindow`, `UISignalPicker`, and `UISignalTagPicker`. Blueprint icons, blueprint description icons, smart input icons, and other vanilla surfaces that use those pickers benefit from this. DSPCore does not skip injection based on GenesisBook, OrbitalRing, FE, or other plugin GUIDs. Truly rebuilt third-party picker surfaces that do not reuse vanilla pickers need dedicated adapters.
- Picker row and column counts are computed from runtime `GridIndex` data. DSPCore starts from the current UI surface's real dimensions, then scans the relevant item, recipe, or signal cells to find the largest required row/column count and expands arrays, materials, mouse hit testing, and visible size together. Mods do not declare picker width or height separately.
- The UI framework provides common scaffolding only. It does not register concrete pages, business navigation, unlock conditions, or save state.
- The proto phase hook is a conservative first bridge, not the final VFPreload mid-stage lifecycle.
- The optional multiplayer bridge does not directly send Nebula packets yet; a dedicated Nebula adapter should read DSPCore's multiplayer registry.
- The network module is currently a query adapter platform, not a built-in scanner for every vanilla or third-party network.
- Player convenience modules such as RecipeFinder, FreeMechaCustom, and AssemblerUI-style features are still outside the core.

## Example: Proto Phase Registration

```csharp
using DSPCore;

ProtoRegistration.Data("com.example.my-mod", data =>
{
    itemProto
        .SetGridIndex(tab: 3, row: 1, index: 5)
        .RegisterItem("com.example.my-mod", purpose: "Declare base item");
});

ProtoRegistration.DataUpdates("com.example.my-mod", data =>
{
    recipeProto
        .SetGridIndex(tab: 3, row: 1, index: 6)
        .RegisterRecipe("com.example.my-mod", CoreDataPhase.DataUpdates, "Attach recipe after item declarations");

    ItemProto baseItem = data.FindItem(1001);
    if (baseItem != null)
        baseItem.GridIndex = GridIndexes.From(tab: 3, row: 1, index: 5);
});

ProtoRegistration.DataFinalFixes("com.example.my-mod", data =>
{
    data.RegisterTutorial(tutorialProto, "Final tutorial chain fix");
});
```

## Example: Achievement Policy

```csharp
using DSPCore;

Achievements.Declare("com.example.my-mod", disableAchievements: true);

bool disabled = Achievements.ShouldDisableAchievements();
```

Not calling the API, or declaring `disableAchievements: false`, means that mod does not request achievement disabling. When multiple mods declare policies, any true wins. See `DSPCore/Authoring/Achievements/README-EN.md` for the detailed cases.

## Example: Build Bar

```csharp
using DSPCore;

myItemProto.SetBuildBar(tab: 3, row: 2, index: 5);
BuildBar.BindQuickBar(tab: 3, row: 2, index: 4, itemId: 9554);
```

## Example: Options, Resources, And Icon Short Entries

```csharp
using DSPCore;

bool enabled = Options.Bool("Example", "Enabled", true, "Enable example feature.");
int rows = Options.Int("Example", "Rows", 2, "Example row count.");
int maxRows = Options.IntRange("Example", "MaxRows", 3, "Maximum rows.", minimum: 1, maximum: 6);
// Open from a button, key bind, or custom UI callback after UIRoot is ready.
Options.OpenWindow();

var pack = ModResources.Pack(
    ownerModGuid: "com.example.my-mod",
    rootPath: "assets/icons",
    assembly: typeof(MyPlugin).Assembly);

pack.Text("ExampleMachines", "enUS", "Example Machines");
pack.IconFromEmbedded("example-embedded", "ExampleMod.Assets.example.png");
pack.IconFromAssetBundle("example-bundle", "example-icons", "example-machine");
pack.BindIconToProto("example-machine", "example.png", ProtoKind.Item, 9554);
```

## Example: Automatic Saves, Delegate Saves, And Lifecycle

```csharp
using DSPCore;

private sealed class ExampleState
{
    [CoreSaveField("counter")]
    public int Counter { get; set; }
}

private static readonly ExampleState State = Saves.Auto("com.example.auto-mod", new ExampleState());

Saves.Register(
    modGuid: "com.example.my-mod",
    export: writer => writer.Write(counter),
    import: reader => counter = reader.ReadInt32(),
    intoOtherSave: () => counter = 0);

Lifecycle.OnStarted(InitializeAfterDspCore);
Lifecycle.OnBeforeSave(saveName => FlushTransientCache(saveName));
Lifecycle.OnAfterLoad(RebuildTransientCache);
```

## Example: Conditional Patches

```csharp
using DSPCore;

Patches.Register(
    id: "example.core-patch",
    ownerModGuid: "com.example.my-mod",
    apply: ApplyCorePatch,
    description: "Apply example runtime patch.",
    isEnabled: IsFeatureEnabled,
    getDisabledReason: () => "example feature is disabled");

Patches.RegisterForPlugin(
    id: "example.target-plugin-integration",
    ownerModGuid: "com.example.my-mod",
    requiredPluginGuid: "com.example.target-plugin",
    apply: ApplyTargetPluginIntegration,
    description: "Enable integration when the target plugin is loaded.",
    minimumPluginVersion: "1.2.0");
```

## Example: Tabs And GridIndex

```csharp
using DSPCore;

TabSlot machinesTab = Tabs.AddTab(
    id: "example-machines",
    ownerModGuid: "com.example.my-mod",
    title: "Example Machines",
    iconId: "example-machine-tab",
    order: 100);

itemProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
recipeProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
```

## Example: Legacy BuildBarTool Compatibility

```csharp
#pragma warning disable CS0618
BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
#pragma warning restore CS0618
```

The old call is accepted, but it is marked obsolete. New mods should prefer `ItemProto.SetBuildBar(...)`, and use `DSPCore.BuildBar.BindQuickBar(...)` only when they only have an item id.

## Documentation

- `README.md`
- Capability examples use paired `Examples/<Scenario>.md` + `Examples/<Scenario>Example.cs` files. `.cs` examples are documentation artifacts and are excluded from compilation.
- `DSPCore/Authoring/Core/Examples/LifecycleExample.cs`
- `DSPCore/Authoring/Core/Examples/Lifecycle.md`
- `DSPCore/Authoring/Core/Examples/ModuleDeclarationExample.cs`
- `DSPCore/Authoring/Core/Examples/ModuleDeclaration.md`
- `DSPCore/Authoring/Core/Examples/PatchPlatformExample.cs`
- `DSPCore/Authoring/Core/Examples/PatchPlatform.md`
- `DSPCore/Authoring/Achievements/Examples/AchievementPolicyExample.cs`
- `DSPCore/Authoring/Achievements/Examples/AchievementPolicy.md`
- `DSPCore/Authoring/BuildBar/Examples/QuickBarBindingExample.cs`
- `DSPCore/Authoring/BuildBar/Examples/QuickBarBinding.md`
- `DSPCore/Authoring/Saves/Examples/SaveHandlerExample.cs`
- `DSPCore/Authoring/Saves/Examples/SaveHandler.md`
- `DSPCore/Authoring/Saves/Examples/SaveBlocksExample.cs`
- `DSPCore/Authoring/Saves/Examples/SaveBlocks.md`
- `DSPCore/Authoring/Icons/Examples/IconSetRegistrationExample.cs`
- `DSPCore/Authoring/Icons/Examples/IconSetRegistration.md`
- `DSPCore/Authoring/Resources/Examples/ResourceRegistrationExample.cs`
- `DSPCore/Authoring/Resources/Examples/ResourceRegistration.md`
- `DSPCore/Authoring/Items/Examples/ItemAuthoringChainExample.cs`
- `DSPCore/Authoring/Items/Examples/ItemAuthoringChain.md`
- `DSPCore/Authoring/Recipes/Examples/RecipeAuthoringChainExample.cs`
- `DSPCore/Authoring/Recipes/Examples/RecipeAuthoringChain.md`
- `DSPCore/Authoring/Tabs/Examples/TabRegistrationExample.cs`
- `DSPCore/Authoring/Tabs/Examples/TabRegistration.md`
- `DSPCore/Systems/PickerSurfaces/Examples/PickerRequestExample.cs`
- `DSPCore/Systems/PickerSurfaces/Examples/PickerRequest.md`
- `DSPCore/Authoring/GameEnums/Examples/RecipeTypeRegistrationExample.cs`
- `DSPCore/Authoring/GameEnums/Examples/RecipeTypeRegistration.md`
- `DSPCore/Authoring/DataPhases/Examples/ProtoPhasesExample.cs`
- `DSPCore/Authoring/DataPhases/Examples/ProtoPhases.md`
- `DSPCore/Authoring/KeyBinds/Examples/KeyBindRegistrationExample.cs`
- `DSPCore/Authoring/KeyBinds/Examples/KeyBindRegistration.md`
- `DSPCore/Authoring/UI/Examples/WindowScaffoldExample.cs`
- `DSPCore/Authoring/UI/Examples/WindowScaffold.md`
- `DSPCore/Authoring/Components/Examples/EntityComponentExample.cs`
- `DSPCore/Authoring/Components/Examples/EntityComponent.md`
- `DSPCore/Authoring/Planets/Examples/PlanetSystemExample.cs`
- `DSPCore/Authoring/Planets/Examples/PlanetSystem.md`
- `DSPCore/Authoring/Blueprints/Examples/BuildingParametersExample.cs`
- `DSPCore/Authoring/Blueprints/Examples/BuildingParameters.md`
- `DSPCore/Authoring/Models/Examples/CloneModelExample.cs`
- `DSPCore/Authoring/Models/Examples/CloneModel.md`
- `DSPCore/Authoring/Options/Examples/OptionRegistrationExample.cs`
- `DSPCore/Authoring/Options/Examples/OptionRegistration.md`
- `DSPCore/Authoring/Multiplayer/Examples/SoftPacketExample.cs`
- `DSPCore/Authoring/Multiplayer/Examples/SoftPacket.md`
- `DSPCore/Authoring/Networks/Examples/CommonNetworkExample.cs`
- `DSPCore/Authoring/Networks/Examples/CommonNetwork.md`
- `DSPCore/Authoring/Galaxy/Examples/GalaxyLifecycleExample.cs`
- `DSPCore/Authoring/Galaxy/Examples/GalaxyLifecycle.md`
