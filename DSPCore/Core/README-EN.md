# Core

The Core block provides the DSPCore global entry point, built-in feature registration, module/feature declaration registries, runtime startup assembly, and a CommonAPI legacy module-query shim. It coordinates other feature blocks; it does not own concrete behavior such as build bar, saves, icons, or achievements.

## What This Block Gives You

- You can use short entries through `using DSPCore;`, or use `DspCore` to access all global registries.
- DSPCore initializes built-in features and module declarations from BepInEx `Awake`, then assembles Harmony patches for feature blocks.
- `Features` and `Modules` let mods declare feature/module metadata that other mods can query.
- Legacy CommonAPI `CommonAPIPlugin.IsSubmoduleLoaded(...)` bridges to `Modules.TryGet(...)` for migration.

## Capability: Access Global Registries

`DspCore` is the aggregate entry point and contains:

- `DspCore.ProtoRegistration`
- `DspCore.BuildBar`
- `DspCore.Saves`
- `DspCore.Resources`
- `DspCore.Icons`
- `DspCore.Tabs`
- `DspCore.Pickers`
- `DspCore.RecipeTypes`
- `DspCore.KeyBinds`
- `DspCore.Achievements`
- `DspCore.Errors`

Examples should usually use short entries such as `ProtoRegistration.RegisterItem(...)`, `Saves.Register(...)`, and `BuildBar.BindQuickBar(...)`. Use `DspCore` when you need registry snapshots or aggregate services.

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

## Capability: Declare Patch Metadata

`DspCore.Patches.Register(new PatchDescriptor(...))` currently stores patch metadata only. It does not automatically apply Harmony patches for you. Concrete patches still belong in the owning feature block or your mod runtime code.

## Capability: CommonAPI Compatibility Query

Legacy code calling:

```csharp
CommonAPI.CommonAPIPlugin.IsSubmoduleLoaded("example.module")
```

is redirected to `DSPCore.Modules.TryGet(...)`. `CommonAPISubmoduleDependencyAttribute` remains as an obsolete compatibility type, but it does not reproduce CommonAPI's full submodule scanning, version constraint, or loader behavior.

## What This Block Does Not Own

- It does not contain concrete feature behavior; saves, icons, build bar, achievements, and similar logic must stay in the owning feature block.
- It does not automatically resolve a module dependency graph; `Dependencies` is currently descriptor data.
- It does not automatically apply patch descriptors registered in `PatchRegistry`.
- It should not grow a centralized legacy compatibility directory; old API shims must live in the owning feature block's `Compat/`.

## Runtime Startup

`DSPCorePlugin.Awake()` initializes DSPCore, registers legacy DSPModSave handlers, creates Harmony, and assembles the currently implemented Proto, BuildBar, Saves, Achievements, Errors, Localization, Tabs, and RecipeTypes patches. `Update()` polls KeyBinds and Pickers.
