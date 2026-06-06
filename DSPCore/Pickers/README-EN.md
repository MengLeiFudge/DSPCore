# Picker Requests

The Pickers block lets a mod request a vanilla item, recipe, or signal picker from its own UI or flow, then receive the player's selection through a callback.

It does not own the item or recipe registration-position model. Authors still set `GridIndex` when registering items or recipes; when they need a new page, they get a `TabSlot` from Tabs first.

## What This Block Gives You

- You do not need to handle the different popup APIs for `UIItemPicker`, `UIRecipePicker`, and `UISignalPicker` yourself.
- Picker requests enter a DSPCore queue and are consumed during UI update, reducing the risk of opening pickers at the wrong UI timing.
- DSPCore centralizes result filtering, exception reporting, and `null` callbacks on failure.
- One `PickerRequest` model covers item, recipe, and signal selection.
- Live grids for item, recipe, and signal pickers apply request filters and move duplicate `GridIndex` entries to later empty cells so later registrations do not overwrite earlier visible entries.

## Capability: Open One Picker

```csharp
Pickers.Open(new PickerRequest(
    Kind: PickerKind.Item,
    OwnerModGuid: "com.example.my-mod",
    Filter: value => value is ItemProto item && item.ID > 0,
    ShowLocked: false,
    ShowAll: false,
    OnReturn: value => { /* handle item, recipe, signal id, or null */ }));
```

`Kind` chooses the item, recipe, or signal picker. `OnReturn` may receive an `ItemProto`, a `RecipeProto`, a signal id, or `null`.

## What DSPCore Does After The Call

- `Pickers.Open(...)` enqueues the request instead of opening UI immediately.
- Runtime update consumes the current queue and opens pickers one by one.
- Item picker requests set `UIItemPicker.showAll` from `ShowAll` or `ShowLocked`.
- Item, recipe, and signal picker grids apply `Filter` while refreshing. If the returned value still fails `Filter`, DSPCore calls `OnReturn(null)`.
- Vanilla `UIItemPicker`, `UIRecipePicker`, `UISignalPicker`, and `UISignalTagPicker` receive duplicate `GridIndex` fallbacks. Blueprint icons, description icons, smart-input icons, and other vanilla surfaces that reuse signal/tag pickers benefit from this.
- If opening or callback handling throws, DSPCore reports the exception to Errors and calls `OnReturn(null)`.

## What This Block Does Not Own

- It does not allocate `TabSlot` values; page registration belongs to Tabs.
- It does not set `GridIndex`; item and recipe cells belong to ProtoRegistration and the proto objects themselves.
- `OnReturn` is not guaranteed to be non-null. Cancel, filter failure, or exceptions all return null.
- It does not provide a custom picker UI; current runtime uses vanilla picker popups.
- It does not directly adapt picker UIs rebuilt by GenesisBook, OrbitalRing, FE, or similar UI takeover mods; those third-party surfaces need dedicated runtime adapters.

## Examples

- `Examples/PickerRequest.md`
- `Examples/PickerRequestExample.cs`
