# Pickers

The Pickers block lets a mod request a vanilla item, recipe, or signal picker from its own UI or flow, then receive the player's selection through a callback.

## What This Block Gives You

- You do not need to handle the different popup APIs for `UIItemPicker`, `UIRecipePicker`, and `UISignalPicker` yourself.
- Picker requests enter a DSPCore queue and are consumed during UI update, reducing the risk of opening pickers at the wrong UI timing.
- DSPCore centralizes result filtering, exception reporting, and `null` callbacks on failure.
- One `PickerRequest` model covers item, recipe, and signal selection.

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
- If the returned value fails `Filter`, DSPCore calls `OnReturn(null)`.
- If opening or callback handling throws, DSPCore reports the exception to Errors and calls `OnReturn(null)`.

## What This Block Does Not Own

- Current filters validate only the returned value; they do not hide invalid entries inside the live picker grid.
- `OnReturn` is not guaranteed to be non-null. Cancel, filter failure, or exceptions all return null.
- It does not provide a custom picker UI; current runtime uses vanilla picker popups.

## Examples

- `Examples/PickerRequest.md`
- `Examples/PickerRequestExample.cs`
