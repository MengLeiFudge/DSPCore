# BuildBar

## Responsibility

This block only binds an item id or `ItemProto` to a quick build bar slot.

## Slot Model

The new standard slot is `tab`, `row`, and `index`.

## Public API

- `Api/ItemProtoQuickBarExtensions.cs`: preferred `ItemProto.BindQuickBar(...)` author-facing style.
- `Api/BuildBar.cs`: short entry point when you only have an item id.
- `Api/BuildBarRegistry.cs`: slot binding registry.
- `Api/BuildBarSlot.cs`
- `Api/BuildBarTier.cs`: obsolete compatibility calls.

## Example

- `Examples/QuickBarBinding.md`
- `Examples/QuickBarBindingExample.cs`

## Runtime

`Runtime/BuildBarRuntime.cs` writes row 1 into vanilla `UIBuildMenu.protos` and creates DSPCore extended buttons for row 2 and later.

## Boundaries

Item creation belongs to proto/item features. Those features may call `ItemProto.BindQuickBar(...)` after creating or modifying an item. Legacy `SetBuildBar` is kept only for BuildBarTool and LDBTool compatibility.
