# Runtime Host

## Responsibility

This directory only keeps the BepInEx plugin entry point and cross-feature runtime assembly.

## Public API

`Runtime/DSPCorePlugin.cs` is the BepInEx plugin entry point.

## Runtime

`DSPCorePlugin` initializes `DspCore`, registers legacy DSPModSave handlers, applies Harmony patches owned by feature blocks, and drives per-frame feature polling from Unity `Update`.

## Boundaries

Runtime does not define public feature semantics and does not own concrete feature runtime implementations. Concrete bridge code lives in the matching feature block directory, such as `../BuildBar/Runtime/BuildBarRuntime.cs`, `../Protos/Runtime/ProtoRuntime.cs`, and `../Saves/Runtime/SaveRuntime.cs`.
