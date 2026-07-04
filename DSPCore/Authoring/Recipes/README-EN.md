# Recipe Registration

Recipes is the recipe proto registration entry point. It only represents the author-facing capability of registering `RecipeProto`; phase timing is owned by DataPhases, and LDB insertion/cache rebuilds are handled by Systems/ProtoPipeline.

## What This Block Gives You

- When you already have a `RecipeProto`, configure `GridIndex`, bind an icon, and register it directly on the object; normal paths, embedded PNGs, and AssetBundle icons all have object-centric entries.
- Common recipe fields have chainable entries: `SetIconTag(...)` and `SetNonProductive(...)`.
- If the recipe depends on item ids, use `CoreDataPhase.DataUpdates` to express "declare items first, attach recipes later".
- `Recipes.Register(...)` remains available as the lower-level entry.

## Capability: Object-Centric Registration

```csharp
var pack = ModResources.Pack("com.example.my-mod", "assets/icons", typeof(MyPlugin).Assembly);

recipeProto
    .SetIconTag("example-recipe")
    .SetNonProductive()
    .SetGridIndex(tab, row: 1, index: 6)
    .BindIcon(pack, "example-recipe", "example-recipe.png")
    .RegisterRecipe("com.example.my-mod", CoreDataPhase.DataUpdates, "Attach example recipe");
```

`SetIconTag(...)` writes the newer DSP `RecipeProto.IconTag` field, and `SetNonProductive(...)` writes vanilla `RecipeProto.NonProductive`. `SetGridIndex(...)` writes the native `RecipeProto.GridIndex`. `BindIcon(...)`, `BindEmbeddedIcon(...)`, and `BindAssetBundleIcon(...)` only register icon descriptors; the Icons runtime still applies them during cache rebuilds. `RegisterRecipe(...)` sends the current recipe to DSPCore's ProtoPipeline.

## Capability: Low-Level Registration

```csharp
Recipes.Register(recipeProto, "com.example.my-mod", CoreDataPhase.DataUpdates, "Attach example recipe");
```

`Recipes.Register(...)` remains a thin entry over `ProtoRegistration.RegisterRecipe(...)`. Recipe type or machine-use restrictions are not owned here; they belong to GameEnums.

## What This Block Does Not Own

- It does not create `RecipeProto` automatically; authors still prepare recipe fields according to DSP semantics.
- It does not own custom recipe types or machine-use restrictions; those belong to GameEnums.
- It does not own tech unlocks, tutorial chains, or player convenience pages.

## Examples

- `Examples/RecipeAuthoringChain.md`
- `Examples/RecipeAuthoringChainExample.cs`
