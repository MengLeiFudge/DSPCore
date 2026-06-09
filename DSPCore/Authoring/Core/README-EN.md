# Core

The Core block provides the DSPCore global entry point, built-in feature registration, module/feature declaration registries, runtime startup assembly, and a CommonAPI legacy module-query shim. It coordinates other authoring capabilities and systems; it does not own concrete behavior such as build bar, saves, icons, or achievements.

## What This Block Gives You

- You can use short entries through `using DSPCore;`, or use `DspCore` to access all global registries.
- DSPCore initializes built-in features and module declarations from BepInEx `Awake`, then assembles Harmony patches for systems.
- `Features` and `Modules` let mods declare feature/module metadata that other mods can query.
- `Patches` can declare conditional patches that DSPCore schedules and logs when required plugins are missing, versions are too old, conditions disable the patch, or apply fails.
- `Lifecycle` can register DSPCore runtime started, update, destroyed, and common save-chain callbacks for small framework-level initialization, cleanup, or save-boundary coordination.
- Legacy CommonAPI `CommonAPIPlugin.IsSubmoduleLoaded(...)` bridges to `Modules.TryGet(...)` for migration.

## Capability: Access Global Registries

`DspCore` is the aggregate entry point and contains:

- `DspCore.ProtoRegistration`
- `DspCore.BuildBar`
- `DspCore.Saves`
- `DspCore.Resources`
- `DspCore.Icons`
- `DspCore.Tabs`
- `DspCore.GameEnums`
- `DspCore.RecipeTypes` (legacy alias)
- `DspCore.KeyBinds`
- `DspCore.Achievements`
- `DspCore.Patches`

Examples should usually use short entries such as `ProtoRegistration.RegisterItem(...)`, `Saves.Register(...)`, `BuildBar.BindQuickBar(...)`, and `Patches.Register(...)`. Use `DspCore` when you need registry snapshots or aggregate services.

## Capability: Declare Features And Modules

```csharp
Modules.Register(new ModuleDescriptor(
    Id: "example.module",
    DisplayName: "Example Module",
    Initialize: InitializeModule,
    Dependencies: null));
```

`Features.Register(...)` is for feature-block-level capabilities. `Modules.Register(...)` is for internal or cross-mod module metadata. If the same ID is registered more than once, the later registration replaces the earlier one.

During initialization, DSPCore initializes features by priority and ID. Modules currently initialize in registry enumeration order and do not use dependency topological sorting.

## Capability: Declare Conditional Patches

```csharp
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

`Patches.Register(...)` is the normal author entry. `Patches.RegisterForPlugin(...)` is for integration patches that depend on another plugin. DSPCore applies registered declarations after `DSPCorePlugin.Awake()` assembles built-in runtime patches. If the target plugin is missing, the version is too old, or `isEnabled` returns false, DSPCore logs the disabled reason and skips `apply`.

`DspCore.Patches.Register(new PatchDescriptor(...))` remains as the low-level registry path for batch registration or existing descriptor flows. Concrete Harmony patch code should still live in the owning system or your mod runtime code.

## Capability: Register DSPCore Lifecycle Callbacks

```csharp
Lifecycle.OnStarted(InitializeAfterDspCore);
Lifecycle.OnUpdate(PollSmallRuntimeState);
Lifecycle.OnDestroyed(DisposeState);
Lifecycle.OnBeforeSave(saveName => FlushRuntimeCache(saveName));
Lifecycle.OnAfterLoad(RebuildRuntimeCache);
```

`OnStarted` runs after the DSPCore plugin entry finishes runtime bridge assembly; if registered after DSPCore has already started, it runs immediately once. `OnUpdate` follows the DSPCore plugin update loop, and `OnDestroyed` runs when the DSPCore plugin is destroyed.

Save-chain events include `OnNewGame`, `OnBeforeSave(saveName)`, `OnBeforeLoad(saveName)`, `OnAfterLoad`, and `OnSaveDeleted(saveName)`. They are forwarded from DSPCore's existing `GameData` / `GameSave` / save-delete bridge and are intended for cache refresh, temporary index cleanup, or non-persistent state coordination. Complex persistence should still use `Saves`.

Framework started/destroyed events do not mean a specific game save, planet, factory, or UI surface exists.

## Capability: CommonAPI Compatibility Query

Legacy code calling:

```csharp
CommonAPI.CommonAPIPlugin.IsSubmoduleLoaded("example.module")
```

is redirected to `DSPCore.Modules.TryGet(...)`. `CommonAPISubmoduleDependencyAttribute` remains as an obsolete compatibility type, but it does not reproduce CommonAPI's full submodule scanning, version constraint, or loader behavior.

## What This Block Does Not Own

- It does not contain concrete feature behavior; saves, icons, build bar, achievements, and similar logic must stay in the owning authoring capability and system directories.
- It does not automatically resolve a module dependency graph; `Dependencies` is currently descriptor data.
- It does not own concrete feature patch implementations; `PatchRegistry` stores declarations, while concrete Harmony patch code should be supplied by the owning system or mod runtime code.
- It should not grow a centralized legacy compatibility directory; old API shims must live in the owning authoring capability's `Compat/`.

## Runtime Startup

`DSPCorePlugin.Awake()` initializes DSPCore, registers legacy DSPModSave handlers, creates Harmony, assembles the currently implemented Proto, BuildBar, Saves, Achievements, Errors, Localization, Tabs, and GameEnums patches, then raises `Lifecycle.OnStarted`. `Update()` polls KeyBinds, picker surfaces, and `Lifecycle.OnUpdate`. Save-chain events are forwarded by `SaveRuntime` from the corresponding `GameSave` / `GameData` patches.

## Examples

- `Examples/Lifecycle.md`
- `Examples/LifecycleExample.cs`
- `Examples/PatchPlatform.md`
- `Examples/PatchPlatformExample.cs`
