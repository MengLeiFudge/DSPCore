# Saves

## Responsibility

This block declares mod save handlers and tagged block save helpers.

## Public API

- `Saves`: author-facing short entry point.
- `ICoreSaveHandler`
- `SaveRegistry`
- `SaveRegistration`
- `CoreLoadOrder`
- `SaveBlock`
- `SaveBlockFormat`

## Examples

- `Examples/SaveHandlerExample.cs`
- `Examples/SaveBlocksExample.cs`

## Runtime

`Runtime/SaveRuntime.cs` writes and reads `.dspcore` sidecar save files, calls handlers by load order, and bridges covered legacy DSPModSave handlers.

## Boundaries

Handlers may use raw `BinaryReader`/`BinaryWriter` APIs or tagged blocks. Tagged blocks are preferred for fields that may be added or removed over time.
