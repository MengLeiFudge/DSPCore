# Models And Prefabs

The Models module lets mods declare a new `ModelProto` cloned from an existing model and configure both `ModelProto` and `PrefabDesc` before final proto cache rebuilds.

## What This Module Provides

- Mods do not need to expand `LDB.models.dataArray` manually.
- DSPCore applies model clones after `DataFinalFixes`, then rebuilds `ModelProto` derived caches and `PlanetFactory.PrefabDescByModelIndex`.
- `PrefabDesc` is separated from the source model, so configuration callbacks do not directly mutate the source model.
- When the caller already has a `ModelProto`, use `sourceModel.CloneAsModel(...)` directly. Use `Models.CloneModel(...)` only when the caller only has a model index.

## Capability: Declare Model Clones

```csharp
sourceModel.CloneAsModel(
    modelIndex: 9554,
    ownerModGuid: "com.example.my-mod",
    configureModel: static model => model.Name = "Example Cloned Model",
    configurePrefab: static prefab => prefab.modelIndex = 9554);
```

`CloneAsModel(...)` uses the current `ModelProto.ID` as the source model index. Runtime re-reads the source model from LDB and creates the clone later, so this call only records a declaration and does not immediately return a new model instance.

If you only have the source model index, use:

```csharp
Models.CloneModel(
    sourceModelIndex: 230,
    modelIndex: 9554,
    ownerModGuid: "com.example.my-mod");
```

`Models.Register(new ModelDescriptor(...))` remains the advanced path for configuration-driven or batch construction.

## Boundaries

- The current implementation shallow-copies public fields. Mesh, material, and prefab references still point to the source model unless the author replaces them.
- Authors must choose a `modelIndex` that does not conflict with other mods.
- This module does not create items or recipes; it is usually used together with Items/Recipes.

## Examples

- `Examples/CloneModel.md`
- `Examples/CloneModelExample.cs`
