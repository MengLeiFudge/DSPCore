# Planet System Example

Use this when a mod needs one state object per loaded planet factory. The system ID is stable save identity and should not be renamed after release.

Use `PlanetSystems.Register<TSystem>(...)` when the system has a parameterless constructor. Use `PlanetSystemDescriptor` only when construction needs the `PlanetFactory` argument immediately.
