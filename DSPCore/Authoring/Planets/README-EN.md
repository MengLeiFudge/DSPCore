# Planet Systems

The Planets module lets mods create one system per loaded `PlanetFactory`. Authors can use `PlanetSystems.Register<TSystem>(...)` for systems with parameterless constructors, and use `PlanetSystemDescriptor` when construction needs the `PlanetFactory` immediately. DSPCore forwards factory creation, local planet rendering, factory ticks, and sidecar save lifecycle calls.

## What This Module Provides

- Mods do not need to maintain their own `planetId -> system` dictionaries and delayed load restore logic.
- Data for unloaded planet factories is kept pending during import and restored when the factory loads.
- Planet systems are suitable for planet-wide caches, projection state, building scan indices, and cross-entity scheduling.

## Capability: Short Planet System Registration

```csharp
PlanetSystems.Register<ExamplePlanetSystem>(
    systemId: "com.example.planet-cache",
    ownerModGuid: "com.example.my-mod");
```

`Register<TSystem>(...)` creates the system with a parameterless constructor and writes `systemId` and `PlanetFactory` into the system context. Use `PlanetSystems.Register(new PlanetSystemDescriptor(...))` when construction needs to read `PlanetFactory` immediately.

## Boundaries

- A planet system is initialized only after a `PlanetFactory` exists.
- `DrawUpdate` is forwarded after local planet `FactoryModel.OnCameraPostRender`.
- Planet systems do not replace star or galaxy state; use the Galaxy module for cross-planet state.

## Examples

- `Examples/PlanetSystem.md`
- `Examples/PlanetSystemExample.cs`
