# DSPCore

DSPCore is a new common framework standard for Dyson Sphere Program mods.

## Goals

- Provide one shared API for common mod infrastructure.
- Replace scattered requirements such as LDBTool, DSPModSave, CommonAPI, BuildBarTool, ErrorAnalyzer-style diagnostics, achievement policy aggregation, icon registration, and compatibility patches.
- Keep legacy API namespaces available as `[Obsolete]` compatibility shims, so existing mods can run first and migrate later.

## Package

- Thunderstore package name: `MengLei-DSPCore`
- DLL name: `DSPCore.dll`
- New namespace: `DSPCore`
- Chinese documentation: `README.md`

## First Version Scope

- P0/P1 author-facing feature blocks: feature lifecycle, data phases, item/recipe/tech/tutorial registration, build bar placement, resources, icons, localization, tabs, pickers, recipe types, key binds, saves, achievements, and error reports.
- Legacy compatibility shims for `xiaoye97.LDBTool`, `crecheng.DSPModSave`, `CommonAPI`, and `BuildBarTool`; compatibility code lives under the owning feature block's `Compat/` directory instead of a centralized `Legacy/` directory.
- Bilingual XML summaries for public APIs.

The current version includes P0/P1 runtime bridges: BepInEx/Harmony startup, proto insertion, multi-row build bar binding, resource/icon loading, tabs for item/recipe/replicator surfaces, picker popups, custom recipe type guards, key callbacks, DSPCore sidecar saves, legacy DSPModSave handler bridging, achievement/abnormality/platform policy patches, error reporting, fatal-window copy/close buttons, and localization entries.

## Feature Blocks

P0/P1 blocks are the current implementation target.

- Feature lifecycle: declare feature blocks, dependencies, priority, and initialization.
- Data phases: `Data`, `DataUpdates`, and `DataFinalFixes`.
- Proto features: item, recipe, tech, tutorial, model/building binding, and vanilla data query descriptors.
- Build bar placement: bind an `ItemProto` or item id to a tab/row/index slot, and own two or more build bar rows, player-defined or dynamically overridden slots, related UI projection and refresh handling, RebindBuildBar compatibility, and BuildBarTool compatibility. Other feature blocks, such as item registration, should prefer `ItemProto.BindQuickBar(...)` after they have the item proto; BuildBar does not own proto creation.
- Resources, icons, and localization: resource roots, icon descriptors, and translation entries.
- Tabs and pickers: authors can declare custom tabs for item, recipe, and replicator surfaces, and can open item/recipe/signal picker requests.
- Saves: raw `BinaryReader`/`BinaryWriter` handlers and tagged block helpers.
- Achievements and errors: achievement policy aggregation and structured error reports.

## Runtime Status

Implemented runtime bridges:

- `DSPCorePlugin` starts from BepInEx and applies Harmony patches.
- Proto registrations are applied around `VFPreload.InvokeOnLoadWorkEnded`; DSPCore rebuilds `ProtoSet` indices and key derived caches after final fixes.
- `BuildBarRegistry.BindQuickBar` maps item ids or `ItemProto` instances to build bar tab/row/index slots; row 1 writes vanilla `UIBuildMenu.protos`, and row 2+ uses DSPCore extended buttons. Player-defined or dynamically overridden slots and RebindBuildBar integration belong to the same BuildBar feature block, but are not implemented yet.
- `IconSetRegistry` can load Unity `Resources` sprites or local PNG files, cache them, and apply them to target protos.
- `TabRegistry` projects custom tabs to item picker, recipe picker, and replicator surfaces through the existing GridIndex category flow.
- `Pickers.Open` requests item, recipe, and signal picker popups and invokes the request callback.
- `RecipeTypeRegistry` marks declared recipes as custom recipe types and blocks unsupported assembler machines from selecting them.
- `KeyBindRegistry` polls registered key bindings and invokes callbacks, including simple `Ctrl`/`Alt`/`Shift` modifier combinations.
- `SaveRegistry` writes a `.dspcore` sidecar save file and imports handlers by `CoreLoadOrder`.
- `AchievementPolicyRegistry` only aggregates each mod's GUID and achievement-disable flag; when no mod disables achievements, it blocks vanilla abnormality checks and allows achievement access, leaderboard upload, and platform metadata access; when any mod disables achievements, it blocks those achievement-related capabilities.
- `ErrorReporter` receives Unity fatal/error logs and fatal-window events.
- `ResourceRegistry.RegisterLocalization` is applied to DSP localization keys and language strings.

Current runtime limits:

- Player-defined build bar positions and RebindBuildBar compatibility are not implemented yet.
- Tab projection currently covers item picker, recipe picker, and replicator surfaces. Signal picker, beacon, blueprint, and other surfaces need a richer tab-content model before they can be supported correctly.
- Picker filters are applied on return as a safety check; they do not yet hide invalid entries inside the live picker grid.
- Recipe type runtime blocks unsupported assembler selection, but the assembler recipe picker list is not yet filtered before selection.
- The proto phase hook is a conservative first bridge, not the final VFPreload mid-stage lifecycle.

P2/P3 blocks such as custom machine components, planet/star systems, network helpers, and player convenience modules are TODO and not implemented yet.

## Example: Achievement Policy

```csharp
using DSPCore;

Achievements.Declare("com.example.my-mod", disableAchievements: true);

bool disabled = Achievements.ShouldDisableAchievements();
```

## Example: Build Bar

```csharp
using DSPCore;

myItemProto.BindQuickBar(tab: 3, row: 2, index: 5);
BuildBar.BindQuickBar(tab: 3, row: 2, index: 4, itemId: 9554);
```

## Example: Tabs

```csharp
using DSPCore;

Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "Example Machines",
    IconId: "example-machine-tab",
    Order: 100));
```

## Example: Legacy BuildBarTool Compatibility

```csharp
#pragma warning disable CS0618
BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
#pragma warning restore CS0618
```

The old call is accepted, but it is marked obsolete. New mods should prefer `ItemProto.BindQuickBar(...)`, and use `DSPCore.BuildBar.BindQuickBar(...)` only when they only have an item id.

## Documentation

- `README.md`
- `docs/getting-started.md`
- `docs/api-migration.md`
- Feature examples use paired `Examples/<Scenario>.md` + `Examples/<Scenario>Example.cs` files. `.cs` examples are documentation artifacts and are excluded from compilation.
- `src/DSPCore/Achievements/Examples/AchievementPolicyExample.cs`
- `src/DSPCore/Achievements/Examples/AchievementPolicy.md`
- `src/DSPCore/BuildBar/Examples/QuickBarBindingExample.cs`
- `src/DSPCore/BuildBar/Examples/QuickBarBinding.md`
- `src/DSPCore/Saves/Examples/SaveHandlerExample.cs`
- `src/DSPCore/Saves/Examples/SaveHandler.md`
- `src/DSPCore/Saves/Examples/SaveBlocksExample.cs`
- `src/DSPCore/Saves/Examples/SaveBlocks.md`
- `src/DSPCore/Icons/Examples/IconSetRegistrationExample.cs`
- `src/DSPCore/Icons/Examples/IconSetRegistration.md`
- `src/DSPCore/Tabs/Examples/TabRegistrationExample.cs`
- `src/DSPCore/Tabs/Examples/TabRegistration.md`
- `src/DSPCore/Pickers/Examples/PickerRequestExample.cs`
- `src/DSPCore/Pickers/Examples/PickerRequest.md`
- `src/DSPCore/Recipes/Examples/RecipeTypeRegistrationExample.cs`
- `src/DSPCore/Recipes/Examples/RecipeTypeRegistration.md`
- `src/DSPCore/Protos/Examples/ProtoPhasesExample.cs`
- `src/DSPCore/Protos/Examples/ProtoPhases.md`
- `src/DSPCore/Input/Examples/KeyBindRegistrationExample.cs`
- `src/DSPCore/Input/Examples/KeyBindRegistration.md`
- `src/DSPCore/Core/Examples/CompatibilityPatchExample.cs`
- `src/DSPCore/Core/Examples/CompatibilityPatch.md`
