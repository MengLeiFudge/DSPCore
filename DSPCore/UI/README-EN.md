# UI

The UI block is currently a lightweight description layer for reusable UI metadata such as button descriptors. It is not a full UI framework and does not automatically attach buttons to game windows.

## What This Block Gives You

- Feature blocks can share simple UI descriptor objects instead of passing Unity `GameObject` instances around.
- If multiple feature blocks later need shared buttons, panels, or node factories, this is the place to extend the common descriptor model.
- The current implementation stays small and avoids putting concrete window logic into Core too early.

## Capability: Create Button Descriptors

```csharp
var button = new UiNodeFactory().Button(
    id: "example.copy",
    title: "Copy",
    tooltip: "Copy error text");
```

The return value is a `UiButtonDescriptor` containing only `Id`, `Title`, and `Tooltip`.

## What DSPCore Does After The Call

There is currently no generic UI runtime that consumes these descriptors. Concrete window behavior remains in the owning feature block; Errors handles fatal-window buttons, and Tabs handles tab buttons itself.

## What This Block Does Not Own

- It does not automatically create Unity `GameObject` instances.
- It does not provide layout, style, event binding, or lifecycle management.
- It does not own concrete feature UI; concrete behavior must stay in the owning feature block.
- It should not absorb UI behavior already owned by Tabs, Pickers, Errors, or other concrete feature blocks.
