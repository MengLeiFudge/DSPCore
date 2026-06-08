# Blueprint Parameters

The Blueprints module provides tagged parameter blocks for building copy-paste, blueprints, and prebuild restore. Mods do not compete for fixed slots inside vanilla `int[] parameters`; they register stable `BlockId` values, and DSPCore encodes blocks at the end of the parameter array.

## What This Module Provides

- Multiple mods can share one `BuildingParameters.parameters` array without overwriting each other.
- Copy, blueprint export, blueprint import, paste, and prebuild apply use the same block ID.
- `CanPaste` can reject incompatible parameter blocks.

## Boundaries

- Block payloads are currently `int[]`; mods should encode complex binary state themselves.
- `BlockId` must stay stable.
- DSPCore moves and dispatches parameter blocks, but it does not understand mod-specific business semantics.

## Examples

- `Examples/BuildingParameters.md`
- `Examples/BuildingParametersExample.cs`
