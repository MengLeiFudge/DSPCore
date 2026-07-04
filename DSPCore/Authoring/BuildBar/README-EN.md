# BuildBar

The BuildBar block lets a mod place an existing item on a quick build bar slot. It does not create items; it maps an existing `ItemProto` or item id to a `category`, `row`, and `index` slot, then projects those bindings to the game build menu.

## What This Block Gives You

- You do not need to patch `UIBuildMenu.protos` or clone build bar buttons yourself.
- You can describe vanilla row 1 and DSPCore extended rows with the same slot model.
- Legacy BuildBarTool / LDBTool style `SetBuildBar` calls bridge to the same binding model, reducing the risk of multiple build bar patches overwriting each other.
- Extended row buttons reuse vanilla click behavior, icons, item counts, and unlock checks instead of forcing each mod to rebuild that UI behavior.
- DSPCore-owned player overrides can replace author defaults and are saved in the `.dspcore` sidecar.
- Players can hold the build-bar reassign key and click a slot to bind an item through the vanilla item picker, or hold the clear key over a slot to explicitly empty it.

## Capability: Bind Existing Items To Build Bar Slots

If you already have an `ItemProto`, prefer the extension method:

```csharp
itemProto.SetBuildBar(category: 3, row: 2, index: 5);
```

If you only have an item id, use the short entry point:

```csharp
BuildBar.BindQuickBar(category: 3, row: 2, index: 5, itemId: 9554);
```

`category`, `row`, and `index` all start from 1. `row = 1` is the vanilla build bar row; `row > 1` is a DSPCore extended row.

Default bindings do not silently replace existing author defaults. If you need to know whether a target slot is already occupied, use the structured result entry:

```csharp
BuildBarBindResult result = itemProto.SetBuildBarWithResult(category: 3, row: 2, index: 5);
if (result.Status == BuildBarBindStatus.Occupied)
{
    // ExistingItemId is the current author-default item occupying this slot.
}
```

To explicitly replace an existing author default, pass the replacement policy:

```csharp
itemProto.SetBuildBarWithResult(
    category: 3,
    row: 2,
    index: 5,
    conflictPolicy: BuildBarConflictPolicy.ReplaceExisting);
```

`BuildBarBindStatus.Applied` means an empty slot was written; `AlreadyBound` means the slot already contains the same item; `Occupied` means another author default exists and was kept; `Replaced` means the explicit policy replaced it; `Invalid` means invalid arguments. The old bool entry only reports whether the requested item became the current author default.

## Capability: Player Slot Overrides

DSPCore reuses the build bar itself as the player rebinding entry. Holding the `DSPCore Build Bar Reassign` key and clicking a slot opens the vanilla item picker. Holding the `DSPCore Build Bar Clear` key over a slot explicitly clears that slot. Both keys are injected into the vanilla key-binding page.

If your own UI also lets players rebind build bar slots, write to the same player override layer:

```csharp
BuildBar.SetPlayerOverride(category: 3, row: 2, index: 5, itemId: 9555);
BuildBar.ClearPlayerOverride(new BuildBarSlot(3, 2, 5));
```

Overrides take precedence over author defaults and are saved through DSPCore's `.dspcore` sidecar. Passing `itemId = 0` explicitly empties the slot; call `ClearPlayerOverride(...)` to remove the override and return to the author default.
`BuildBar.OpenEditor()` remains only as an obsolete source-compatibility entry and no longer opens a separate window.

## What DSPCore Does After The Call

- Registration records author default slot-to-item bindings. When another author default already occupies the same slot, the default policy keeps the existing binding and returns `Occupied` instead of silently replacing it.
- Authors that need to replace an existing default must use `BuildBarConflictPolicy.ReplaceExisting`; the result's `ExistingItemId` tells you which old item was replaced or kept.
- If `category`, `row`, or `index` is less than 1, or item id is less than or equal to 0, the binding is rejected and returns false.
- After `UIBuildMenu.StaticLoad`, `row = 1` bindings are written to vanilla `UIBuildMenu.protos`, and the item's `BuildIndex` is updated.
- When the build menu opens and refreshes, `row > 1` bindings create or refresh DSPCore extended buttons.
- When an extended button is clicked, DSPCore temporarily injects the item into the current category's vanilla slot and invokes the vanilla build bar click logic.
- Runtime reads effective bindings after applying player overrides on top of author defaults; player overrides are imported and exported with DSPCore saves.
- After a player changes a slot through build-bar hotkeys, DSPCore immediately refreshes the current build bar projection.
- If the current save has no DSPCore BuildBar data yet, DSPCore reads RebindBuildBar's `RebindBuildBar/CustomBarBind.cfg` and imports `[BuildBarBinds]` entries as vanilla row-1 player overrides.

## Capability: Legacy BuildBarTool / LDBTool Compatibility

Legacy entries remain under `Compat/`:

- `BuildBarTool.BuildBarTool.SetBuildBar(category, index, itemId, isTopRow)`
- `DSPCore.LegacyBuildBarCompatibility.SetBuildBar(category, index, itemId, layer)`
- `xiaoye97.LDBTool.SetBuildBar(category, index, itemId)`

These entries map to `category`, `row`, and `index`. New code should use `ItemProto.SetBuildBar(...)` or `BuildBar.BindQuickBar(...)`; legacy entries are kept for migration and source compatibility.

## What This Block Does Not Own

- It does not create `ItemProto`, recipes, icons, or localization entries; those belong to ProtoRegistration, Icons, and Resources.
- It does not decide whether an item is unlocked; extended buttons use vanilla history unlock state and sandbox instant-item state for interactivity.
- It does not take over RebindBuildBar's rebinding UI, hotkeys, or later config writes; DSPCore only imports existing `CustomBarBind.cfg` entries into its own player override layer. Later DSPCore player edits are written by the build-bar hotkey interaction to the `.dspcore` save.
- `row = 1` is limited by vanilla `UIBuildMenu.protos` dimensions; current runtime skips row-1 bindings outside vanilla tab/index bounds.

## Examples

- `Examples/QuickBarBinding.md`
- `Examples/QuickBarBindingExample.cs`
