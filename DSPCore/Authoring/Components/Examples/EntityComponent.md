# Entity Component Example

Use this when a building needs per-entity state and repeated tick callbacks. Register a stable component ID and match by item id, model index, or `PrefabDesc`.

Use `Components.Register<TComponent>(...)` when the component has a parameterless constructor. Use `ComponentDescriptor` only when construction needs factory/entity/prebuild context or a custom predicate that does not fit the short parameters.

The component ID is written into `.dspcore` saves. Do not rename it after release unless you intentionally migrate old data.
