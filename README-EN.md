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

- `DSPCore/`: main BepInEx plugin project, including Core and feature blocks.
- `DSPCore.Preloader/`: BepInEx patchers project for pre-load game assembly patches.
- `DSPCore.Packaging/`: Thunderstore packaging project.

## First Version Scope

- P0/P1 author-facing feature blocks: feature lifecycle, data phases, proto registration, build bar placement, resources, icons, localization, tabs, pickers, recipe types, key binds, saves, achievements, error reports, and a common UI framework.
- Legacy compatibility shims for `xiaoye97.LDBTool`, `crecheng.DSPModSave`, `CommonAPI`, and `BuildBarTool`; compatibility code lives under the owning feature block's `Compat/` directory instead of a centralized `Legacy/` directory.
- Bilingual XML summaries for public APIs.

The current version includes P0/P1 runtime bridges: BepInEx/Harmony startup, proto insertion, multi-row build bar binding, player overrides, RebindBuildBar configuration import, resource/icon loading, tabs for item/recipe/replicator/signal/tag-icon surfaces, picker popups and live filtering, custom recipe type pre-selection filtering, key callbacks, DSPCore sidecar saves, legacy DSPModSave handler bridging, achievement/abnormality/platform policy patches, error reporting, fatal-window copy/close buttons, localization entries, and common UI window lifecycle forwarding.

## Feature Blocks

P0/P1 blocks are the current implementation target.

- Feature lifecycle: declare feature blocks, dependencies, priority, and initialization.
- Data phases: `Data`, `DataUpdates`, and `DataFinalFixes`.
- Proto registration: item, recipe, tech, tutorial, model/building binding, and vanilla data query descriptors.
- Build bar placement: bind an `ItemProto` or item id to a tab/row/index slot; row 1 writes to the vanilla build bar, row 2+ uses DSPCore extended buttons, and BuildBarTool compatibility entries remain available. Other feature blocks, such as item registration, should prefer `ItemProto.SetBuildBar(...)` after they have the item proto; BuildBar does not own proto creation.
- Resources, icons, and localization: resource roots, icon descriptors, and translation entries.
- Tabs and pickers: authors can declare custom pages, receive a `TabSlot`, and use that slot to generate item/recipe `GridIndex` values; they can also open item/recipe/signal picker requests from their own UI.
- Saves: raw `BinaryReader`/`BinaryWriter` handlers and tagged block helpers.
- Achievements and errors: achievement policy aggregation and structured error reports.
- UI framework: window lifecycle helpers, tabbed windows, base controls, declarative grid layout, and theme/card helpers; concrete business pages are not included.

## Runtime Status

Implemented runtime bridges:

- `DSPCorePlugin` starts from BepInEx and applies Harmony patches.
- Proto registration runs Factorio-like `Data`, `DataUpdates`, and `DataFinalFixes` callbacks. Runtime writes the resulting protos around `VFPreload.InvokeOnLoadWorkEnded`; DSPCore rebuilds `ProtoSet` indices and key derived caches after final fixes.
- `BuildBarRegistry.BindQuickBar` maps item ids or `ItemProto` instances to build bar tab/row/index slots; row 1 writes vanilla `UIBuildMenu.protos`, and row 2+ uses DSPCore extended buttons. `BuildBar.SetPlayerOverride(...)` writes a player override layer to the `.dspcore` save, and runtime uses author defaults overlaid with player overrides. When no DSPCore BuildBar save data exists, DSPCore imports row-1 player configuration from RebindBuildBar's `CustomBarBind.cfg`.
- `IconSetRegistry` can load Unity `Resources` sprites or local PNG files, cache them, and apply them to target protos.
- `TabRegistry` assigns a `TabSlot` for each stable page id and projects custom pages to item picker, recipe picker, replicator, signal picker, and tag-icon picker surfaces through the existing GridIndex category flow.
- `Pickers.Open` requests item, recipe, and signal picker popups. Live grids apply request filters, duplicate `GridIndex` fallbacks, and dynamic row/column expansion, and the returned value is still checked again before callback delivery.
- `RecipeTypeRegistry` marks declared recipes as custom recipe types and hides recipes unsupported by the current assembler before the recipe picker selection; `AssemblerComponent.SetRecipe` remains the final guard.
- `KeyBindRegistry` polls registered key bindings and invokes callbacks, including simple `Ctrl`/`Alt`/`Shift` modifier combinations.
- `SaveRegistry` writes a `.dspcore` sidecar save file and imports handlers by `CoreLoadOrder`.
- `AchievementPolicyRegistry` aggregates each mod's achievement-disable declaration. Not declaring, or declaring `disableAchievements: false`, does not request disabling. If any mod declares true, DSPCore globally blocks achievement mutation, Milky Way / leaderboard uploads, and platform achievement/metadata calls. If no declaration is true, DSPCore blocks vanilla abnormality checks and keeps achievements available.
- `ErrorReporter` receives Unity fatal/error logs and fatal-window events.
- `ResourceRegistry.RegisterLocalization` is applied to DSP localization keys and language strings.
- `UiWindowManager` forwards DSPCore window lifecycle through `UIRoot` open, update, and destroy events; mods still create and open concrete windows themselves.

Current runtime limits:

- RebindBuildBar `BuildBarBinds` configuration is imported into DSPCore row-1 player overrides; DSPCore does not take over RebindBuildBar's rebinding UI, hotkeys, or later config writes.
- Tab projection currently covers vanilla `UIItemPicker`, `UIRecipePicker`, `UIReplicatorWindow`, `UISignalPicker`, and `UISignalTagPicker`. Blueprint icons, blueprint description icons, smart input icons, and other vanilla surfaces that use those pickers benefit from this. DSPCore does not skip injection based on GenesisBook, OrbitalRing, FE, or other plugin GUIDs. Truly rebuilt third-party picker surfaces that do not reuse vanilla pickers need dedicated adapters.
- Picker row and column counts are computed from runtime `GridIndex` data. DSPCore starts from the current UI surface's real dimensions, then scans the relevant item, recipe, or signal cells to find the largest required row/column count and expands arrays, materials, mouse hit testing, and visible size together. Mods do not declare picker width or height separately.
- The UI framework provides common scaffolding only. It does not register concrete pages, business navigation, unlock conditions, or save state.
- The proto phase hook is a conservative first bridge, not the final VFPreload mid-stage lifecycle.

P2/P3 blocks such as custom machine components, planet/star systems, network helpers, and player convenience modules are TODO and not implemented yet.

## Example: Proto Phase Registration

```csharp
using DSPCore;

ProtoRegistration.Data("com.example.my-mod", data =>
{
    data.RegisterItem(itemProto, "Declare base item");
});

ProtoRegistration.DataUpdates("com.example.my-mod", data =>
{
    data.RegisterRecipe(recipeProto, "Attach recipe after item declarations");
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

Not calling the API, or declaring `disableAchievements: false`, means that mod does not request achievement disabling. When multiple mods declare policies, any true wins. See `DSPCore/Achievements/README-EN.md` for the detailed cases.

## Example: Build Bar

```csharp
using DSPCore;

myItemProto.SetBuildBar(tab: 3, row: 2, index: 5);
BuildBar.BindQuickBar(tab: 3, row: 2, index: 4, itemId: 9554);
```

## Example: Tabs And GridIndex

```csharp
using DSPCore;

TabSlot machinesTab = Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "Example Machines",
    IconId: "example-machine-tab",
    Order: 100));

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
- Feature examples use paired `Examples/<Scenario>.md` + `Examples/<Scenario>Example.cs` files. `.cs` examples are documentation artifacts and are excluded from compilation.
- `DSPCore/Achievements/Examples/AchievementPolicyExample.cs`
- `DSPCore/Achievements/Examples/AchievementPolicy.md`
- `DSPCore/BuildBar/Examples/QuickBarBindingExample.cs`
- `DSPCore/BuildBar/Examples/QuickBarBinding.md`
- `DSPCore/Saves/Examples/SaveHandlerExample.cs`
- `DSPCore/Saves/Examples/SaveHandler.md`
- `DSPCore/Saves/Examples/SaveBlocksExample.cs`
- `DSPCore/Saves/Examples/SaveBlocks.md`
- `DSPCore/Icons/Examples/IconSetRegistrationExample.cs`
- `DSPCore/Icons/Examples/IconSetRegistration.md`
- `DSPCore/Tabs/Examples/TabRegistrationExample.cs`
- `DSPCore/Tabs/Examples/TabRegistration.md`
- `DSPCore/Pickers/Examples/PickerRequestExample.cs`
- `DSPCore/Pickers/Examples/PickerRequest.md`
- `DSPCore/RecipeTypes/Examples/RecipeTypeRegistrationExample.cs`
- `DSPCore/RecipeTypes/Examples/RecipeTypeRegistration.md`
- `DSPCore/ProtoRegistration/Examples/ProtoPhasesExample.cs`
- `DSPCore/ProtoRegistration/Examples/ProtoPhases.md`
- `DSPCore/Input/Examples/KeyBindRegistrationExample.cs`
- `DSPCore/Input/Examples/KeyBindRegistration.md`
- `DSPCore/UI/Examples/WindowScaffoldExample.cs`
- `DSPCore/UI/Examples/WindowScaffold.md`
