# Star And Galaxy Systems

The Galaxy module provides star-level and galaxy-level lifecycle. Authors register `StarSystemDescriptor` or `GalaxySystemDescriptor`, and DSPCore creates systems after galaxy data exists, then forwards `SpaceSector.GameTick` updates and sidecar saves.

## What This Module Provides

- Mods do not need to maintain `starId -> system` maps and galaxy singleton state themselves.
- Star systems and galaxy systems use stable system IDs in saves.
- Suitable for galaxy-wide caches, interstellar events, star-level scans, and cross-planet scheduling.

## Boundaries

- The current update hook is space sector tick, not UIStarmap rendering.
- Planet factory logic belongs in the Planets module.
- System IDs must stay stable.

## Examples

- `Examples/GalaxyLifecycle.md`
- `Examples/GalaxyLifecycleExample.cs`
