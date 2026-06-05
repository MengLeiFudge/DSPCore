# Core

## Responsibility

This block owns the `DspCore` entry point and framework-level registries.

## Public API

- `DspCore`
- `Features`: author-facing feature block short entry point.
- `Modules`: author-facing module short entry point.
- `FeatureRegistry`
- `ModuleRegistry`
- `PatchRegistry`
- `FeatureDescriptor`
- `ModuleDescriptor`
- `PatchDescriptor`

## Runtime

`Runtime/DSPCorePlugin.cs` initializes this block from BepInEx and applies Harmony patches.

## Boundaries

Core coordinates features but should not own feature-specific behavior such as saves, icons, build bar, or achievements.
