# Runtime

## Responsibility

This block connects DSPCore registries to BepInEx, Harmony, Unity, and DSP runtime objects.

## Public API

`DSPCorePlugin` is the BepInEx plugin entry point. Other runtime classes are internal adapters.

## Runtime

Runtime adapters patch game methods, apply queued registrations, rebuild caches, save sidecar data, and update UI helpers.

## Boundaries

Runtime should not define new public feature semantics. Public semantics belong to the feature block registries.
