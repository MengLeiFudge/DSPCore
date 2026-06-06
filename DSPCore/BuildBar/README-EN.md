# BuildBar

The BuildBar block lets a mod place an existing item on a quick build bar slot. It does not create items; it maps an existing `ItemProto` or item id to a `tab`, `row`, and `index` slot, then projects those bindings to the game build menu.

## What This Block Gives You

- You do not need to patch `UIBuildMenu.protos` or clone build bar buttons yourself.
- You can describe vanilla row 1 and DSPCore extended rows with the same slot model.
- Legacy BuildBarTool / LDBTool style `SetBuildBar` calls bridge to the same binding model, reducing the risk of multiple build bar patches overwriting each other.
- Extended row buttons reuse vanilla click behavior, icons, item counts, and unlock checks instead of forcing each mod to rebuild that UI behavior.
- DSPCore-owned player overrides can replace author defaults and are saved in the `.dspcore` sidecar.

## Capability: Bind Existing Items To Build Bar Slots

If you already have an `ItemProto`, prefer the extension method:

```csharp
itemProto.SetBuildBar(tab: 3, row: 2, index: 5);
```

If you only have an item id, use the short entry point:

```csharp
BuildBar.BindQuickBar(tab: 3, row: 2, index: 5, itemId: 9554);
```

`tab`, `row`, and `index` all start from 1. `row = 1` is the vanilla build bar row; `row > 1` is a DSPCore extended row.

## Capability: Player Slot Overrides

If your UI lets players rebind build bar slots, write to the player override layer:

```csharp
BuildBar.SetPlayerOverride(tab: 3, row: 2, index: 5, itemId: 9555);
BuildBar.ClearPlayerOverride(new BuildBarSlot(3, 2, 5));
```

Overrides take precedence over author defaults and are saved through DSPCore's `.dspcore` sidecar. Passing `itemId = 0` is equivalent to clearing that slot override.

## What DSPCore Does After The Call

- Registration only records the slot-to-item binding; if the same slot is bound more than once, the later binding replaces the earlier one.
- If `tab`, `row`, or `index` is less than 1, or item id is less than or equal to 0, the binding is rejected and returns false.
- After `UIBuildMenu.StaticLoad`, `row = 1` bindings are written to vanilla `UIBuildMenu.protos`, and the item's `BuildIndex` is updated.
- When the build menu opens and refreshes, `row > 1` bindings create or refresh DSPCore extended buttons.
- When an extended button is clicked, DSPCore temporarily injects the item into the current category's vanilla slot and invokes the vanilla build bar click logic.
- Runtime reads effective bindings after applying player overrides on top of author defaults; player overrides are imported and exported with DSPCore saves.

## Capability: Legacy BuildBarTool / LDBTool Compatibility

Legacy entries remain under `Compat/`:

- `BuildBarTool.BuildBarTool.SetBuildBar(category, index, itemId, isTopRow)`
- `DSPCore.LegacyBuildBarCompatibility.SetBuildBar(category, index, itemId, layer)`
- `xiaoye97.LDBTool.SetBuildBar(category, index, itemId)`

These entries map to `tab`, `row`, and `index`. New code should use `ItemProto.SetBuildBar(...)` or `BuildBar.BindQuickBar(...)`; legacy entries are kept for migration and source compatibility.

## What This Block Does Not Own

- It does not create `ItemProto`, recipes, icons, or localization entries; those belong to ProtoRegistration, Icons, and Resources.
- It does not decide whether an item is unlocked; extended buttons use vanilla history unlock state and sandbox instant-item state for interactivity.
- It does not read external RebindBuildBar player configuration; the current player override layer is DSPCore-owned data.
- `row = 1` is limited by vanilla `UIBuildMenu.protos` dimensions; current runtime skips row-1 bindings outside vanilla tab/index bounds.

## Examples

- `Examples/QuickBarBinding.md`
- `Examples/QuickBarBindingExample.cs`
