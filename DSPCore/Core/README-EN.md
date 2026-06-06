# Core

## Responsibility

This block owns the `DspCore` entry point, framework-level registries, and BepInEx startup assembly.

## Public API

- `Api/DspCore.cs`
- `Api/Features.cs`: author-facing feature block short entry point.
- `Api/Modules.cs`: author-facing module short entry point.
- `Api/FeatureRegistry.cs`
- `Api/ModuleRegistry.cs`
- `Api/PatchRegistry.cs`
- `Api/FeatureDescriptor.cs`
- `Api/ModuleDescriptor.cs`
- `Api/PatchDescriptor.cs`

## Compatibility API

- `Compat/CommonApiShim.cs`: old namespace shell for `CommonAPI` module queries and submodule dependency attributes.
- `Compat/IsExternalInit.cs`: compile-time polyfill required for record/init-only syntax on net472.

## Runtime

`Runtime/DSPCorePlugin.cs` initializes this block from BepInEx and applies Harmony patches.

## Boundaries

Core coordinates feature blocks and startup assembly, but should not own feature-specific behavior such as saves, icons, build bar, or achievements. Legacy API shims and third-party compatibility adapters must live in the owning feature block's `Compat/`.
