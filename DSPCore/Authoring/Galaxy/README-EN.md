# Star And Galaxy Systems

The Galaxy module provides star-level and galaxy-level lifecycle. Authors can use `GalaxySystems.RegisterStar<TSystem>(...)` or `GalaxySystems.RegisterGalaxy<TSystem>(...)` for systems with parameterless constructors, and use descriptors when construction needs `StarData` or `GalaxyData` immediately. DSPCore creates systems after galaxy data exists, then forwards `SpaceSector.GameTick` updates and sidecar saves.

## What This Module Provides

- Mods do not need to maintain `starId -> system` maps and galaxy singleton state themselves.
- Star systems and galaxy systems use stable system IDs in saves.
- Suitable for galaxy-wide caches, interstellar events, star-level scans, and cross-planet scheduling.

## Capability: Short Star / Galaxy System Registration

```csharp
GalaxySystems.RegisterGalaxy<ExampleGalaxySystem>(
    systemId: "com.example.galaxy",
    ownerModGuid: "com.example.my-mod");
```

`RegisterStar<TSystem>(...)` and `RegisterGalaxy<TSystem>(...)` create systems with parameterless constructors and write `systemId` plus `StarData` / `GalaxyData` into the system context. Use the matching descriptor when construction needs to read `StarData` or `GalaxyData` immediately.

## Boundaries

- The current update hook is space sector tick, not UIStarmap rendering.
- Planet factory logic belongs in the Planets module.
- System IDs must stay stable.

## Examples

- `Examples/GalaxyLifecycle.md`
- `Examples/GalaxyLifecycleExample.cs`
