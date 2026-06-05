# BuildBar

## Responsibility

This block only binds an item id or `ItemProto` to a build bar shortcut slot.

## Slot Model

The new standard slot is `tab`, `row`, and `index`.

## Public API

- `BuildBarRegistry.BindItem(tab, row, index, itemId)`
- `BuildBarRegistry.BindItem(tab, row, index, item)`
- `BuildBarSlot`
- `BuildBarTier` for obsolete compatibility calls.

## Example

- `Examples/BuildBarExample.cs`

## Runtime

`Runtime/BuildBarRuntime.cs` writes row 1 into vanilla `UIBuildMenu.protos` and creates DSPCore extended buttons for row 2 and later.

## Boundaries

Item creation belongs to proto/item features. Those features may call `BindItem` after creating or modifying an item.
