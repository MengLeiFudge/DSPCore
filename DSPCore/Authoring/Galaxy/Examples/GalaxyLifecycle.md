# Galaxy Lifecycle Example

Use this when a feature needs state above a single planet factory.

Use `GalaxySystems.RegisterStar<TSystem>(...)` or `GalaxySystems.RegisterGalaxy<TSystem>(...)` when the system has a parameterless constructor. Use descriptors only when construction needs `StarData` or `GalaxyData` immediately.
