# BuildBar

## Responsibility

This block only binds an item id or `ItemProto` to a quick build bar slot.

## Slot Model

The new standard slot is `tab`, `row`, and `index`.

## Public API

- `ItemProto.BindQuickBar(tab, row, index)`: preferred author-facing style.
- `ItemProto.BindQuickBar(buildIndex, row)`: for migrating BuildIndex-style code.
- `BuildBar.BindQuickBar(tab, row, index, itemId)`: use this when you only have an item id.
- `BuildBarRegistry.BindQuickBar(tab, row, index, itemId)`
- `BuildBarSlot`
- `BuildBarTier` for obsolete compatibility calls.

## Example

- `Examples/BuildBarExample.cs`

## Runtime

`BuildBarRuntime.cs` writes row 1 into vanilla `UIBuildMenu.protos` and creates DSPCore extended buttons for row 2 and later.

## Boundaries

Item creation belongs to proto/item features. Those features may call `ItemProto.BindQuickBar(...)` after creating or modifying an item. Legacy `SetBuildBar` is kept only for BuildBarTool and LDBTool compatibility.
