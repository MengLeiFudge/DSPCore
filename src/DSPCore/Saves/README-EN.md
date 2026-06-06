# Saves

## Responsibility

This block declares mod save handlers and tagged block save helpers.

## Public API

- `Api/Saves.cs`: author-facing short entry point.
- `Api/ICoreSaveHandler.cs`
- `Api/SaveRegistry.cs`
- `Api/SaveRegistration.cs`
- `Api/CoreLoadOrder.cs`
- `Api/SaveBlock.cs`
- `Api/SaveBlockFormat.cs`

## Compatibility API

- `Compat/DSPModSaveShim.cs`: old namespace shell for `crecheng.DSPModSave` save interfaces, load order, and manual registration.

## Examples

- `Examples/SaveHandlerExample.cs`
- `Examples/SaveHandler.md`
- `Examples/SaveBlocksExample.cs`
- `Examples/SaveBlocks.md`

## Runtime

`Runtime/SaveRuntime.cs` writes and reads `.dspcore` sidecar save files, calls handlers by load order, and bridges covered legacy DSPModSave handlers.

## Boundaries

Handlers may use raw `BinaryReader`/`BinaryWriter` APIs or tagged blocks. Tagged blocks are preferred for fields that may be added or removed over time.
