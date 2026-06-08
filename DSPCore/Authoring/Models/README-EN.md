# Models And Prefabs

The Models module lets mods declare a new `ModelProto` cloned from an existing model and configure both `ModelProto` and `PrefabDesc` before final proto cache rebuilds.

## What This Module Provides

- Mods do not need to expand `LDB.models.dataArray` manually.
- DSPCore applies model clones after `DataFinalFixes`, then rebuilds `ModelProto` derived caches and `PlanetFactory.PrefabDescByModelIndex`.
- `PrefabDesc` is separated from the source model, so configuration callbacks do not directly mutate the source model.

## Boundaries

- The current implementation shallow-copies public fields. Mesh, material, and prefab references still point to the source model unless the author replaces them.
- Authors must choose a `modelIndex` that does not conflict with other mods.
- This module does not create items or recipes; it is usually used together with Items/Recipes.

## Examples

- `Examples/CloneModel.md`
- `Examples/CloneModelExample.cs`
