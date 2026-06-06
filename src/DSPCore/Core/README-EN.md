# Core

## Responsibility

This block owns the `DspCore` entry point and framework-level registries.

Compatibility patch declarations belong to Core as a framework-level patch support mechanism, not as a separate feature block. Concrete tutorial, build bar, save, UI, and other compatibility logic still belongs to the owning feature block.

## Public API

- `Api/DspCore.cs`
- `Api/Features.cs`: author-facing feature block short entry point.
- `Api/Modules.cs`: author-facing module short entry point.
- `Api/FeatureRegistry.cs`
- `Api/ModuleRegistry.cs`
- `Api/PatchRegistry.cs`
- `Api/Compatibility.cs`: author-facing short entry point for compatibility patch declarations.
- `Api/CompatibilityPatchRegistry.cs`
- `Api/FeatureDescriptor.cs`
- `Api/ModuleDescriptor.cs`
- `Api/PatchDescriptor.cs`
- `Api/CompatibilityPatchDescriptor.cs`

## Examples

- `Examples/CompatibilityPatchExample.cs`
- `Examples/CompatibilityPatch.md`

## Runtime

`../Runtime/Runtime/DSPCorePlugin.cs` initializes this block from BepInEx and applies Harmony patches.

## Boundaries

Core coordinates features but should not own feature-specific behavior such as saves, icons, build bar, or achievements. The compatibility patch registry only records declaration and reporting metadata; it does not own concrete fixes.
