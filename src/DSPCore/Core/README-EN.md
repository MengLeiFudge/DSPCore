# Core

## Responsibility

This block owns the `DspCore` entry point and framework-level registries.

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

## Runtime

`../Runtime/Runtime/DSPCorePlugin.cs` initializes this block from BepInEx and applies Harmony patches.

## Boundaries

Core coordinates features but should not own feature-specific behavior such as saves, icons, build bar, or achievements.
