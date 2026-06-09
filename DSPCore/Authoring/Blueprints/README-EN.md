# Blueprint Parameters

The Blueprints module provides tagged parameter blocks for building copy-paste, blueprints, and prebuild restore. Mods do not compete for fixed slots inside vanilla `int[] parameters`; they register stable `BlockId` values, and DSPCore encodes blocks at the end of the parameter array. Simple cases use `Blueprints.Register(blockId, ownerModGuid, copy, paste, ...)` with direct `int[]` payloads; use `BuildingParameterDescriptor` when full block objects are needed.

## What This Module Provides

- Multiple mods can share one `BuildingParameters.parameters` array without overwriting each other.
- Copy, blueprint export, blueprint import, paste, and prebuild apply use the same block ID.
- `CanPaste` can reject incompatible parameter blocks.

## Capability: Short Integer Parameter Blocks

```csharp
Blueprints.Register(
    blockId: "com.example.mode",
    ownerModGuid: "com.example.my-mod",
    copy: static (factory, objectId) => new[] { 1 },
    paste: static (factory, entityId, data) =>
    {
        int mode = data.Length > 0 ? data[0] : 0;
    });
```

The short entry wraps the `int[]` returned by `copy` into a `BuildingParameterBlock` with `blockId`, and paste/prebuild callbacks receive only the payload array. Use `Blueprints.Register(new BuildingParameterDescriptor(...))` when you need to construct blocks manually, reuse advanced checks, or extend non-standard transfer semantics.

## Boundaries

- Block payloads are currently `int[]`; mods should encode complex binary state themselves.
- `BlockId` must stay stable.
- DSPCore moves and dispatches parameter blocks, but it does not understand mod-specific business semantics.

## Examples

- `Examples/BuildingParameters.md`
- `Examples/BuildingParametersExample.cs`
