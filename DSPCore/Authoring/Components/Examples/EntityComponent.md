# Entity Component Example

Use this when a building needs per-entity state and repeated tick callbacks. Register a stable component ID and match by item id, model index, or `PrefabDesc`.

The component ID is written into `.dspcore` saves. Do not rename it after release unless you intentionally migrate old data.
