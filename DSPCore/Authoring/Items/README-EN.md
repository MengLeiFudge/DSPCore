# Item Registration

Items is the item proto registration entry point. It only represents the author-facing capability of registering `ItemProto`; phase timing is owned by DataPhases, and LDB insertion/cache rebuilds are handled by Systems/ProtoPipeline.

## What This Block Gives You

- When you already have an `ItemProto`, configure `GridIndex`, bind an icon, and register it directly on the object instead of jumping through registry-style calls.
- Common vanilla fields have semantic entries: `SetIconTag(...)`, `UnlockAlways()`, `UnlockByEnemyDrop()`, `UnlockLike(...)`, `UnlockByRecipe()`, `SetPreTechOverride(...)`, and `SetProductive(...)`.
- Icon binding can use `ModResourcePack`, reusing owner and resource root metadata; normal paths, embedded PNGs, and AssetBundle icons all have object-centric entries.
- `Items.Register(...)` remains available as the lower-level entry for batch registration or code that does not use extension methods.

## Capability: Object-Centric Registration

```csharp
var pack = ModResources.Pack("com.example.my-mod", "assets/icons", typeof(MyPlugin).Assembly);

itemProto
    .SetIconTag("example-machine")
    .UnlockAlways()
    .SetProductive()
    .SetGridIndex(tab, row: 1, index: 5)
    .BindIcon(pack, "example-machine", "example-machine.png")
    .RegisterItem("com.example.my-mod", purpose: "Declare example machine");

itemProto.SetBuildBar(category: 3, row: 2, index: 5);
```

`SetIconTag(...)` writes the newer DSP `ItemProto.IconTag` field. `UnlockAlways()` and related unlock helpers write the vanilla `UnlockKey`: `-1` means always unlocked, `-2` means enemy-drop unlock, and a positive value follows another item; `SetUnlockKey(...)` remains available when the raw value is needed. `SetGridIndex(...)` writes the native `ItemProto.GridIndex`. `BindIcon(...)`, `BindEmbeddedIcon(...)`, and `BindAssetBundleIcon(...)` only register icon descriptors; the Icons runtime still applies them during cache rebuilds. `RegisterItem(...)` sends the current item to DSPCore's ProtoPipeline.

## Capability: Low-Level Registration

```csharp
Items.Register(itemProto, "com.example.my-mod", CoreDataPhase.Data, "Declare example machine");
```

`Items.Register(...)` remains a thin entry over `ProtoRegistration.RegisterItem(...)` so item registration is no longer hidden inside the broad ProtoRegistration bucket. Existing `ProtoRegistration.RegisterItem(...)` remains available.

## What This Block Does Not Own

- It does not create `ItemProto` automatically; authors still prepare item fields according to DSP semantics.
- It does not own recipes, techs, or unlock chains; those belong to Recipes, Techs, or the owning mod.
- It does not replace BuildBar; build bar placement still uses `ItemProto.SetBuildBar(...)` or `BuildBar.BindQuickBar(...)`.

## Examples

- `Examples/ItemAuthoringChain.md`
- `Examples/ItemAuthoringChainExample.cs`
