# Saves

The Saves block lets a mod store its own state in DSPCore `.dspcore` sidecar save files and follow vanilla new-game, load, save, delete-save, and autosave rotation flows.

## What This Block Gives You

- You do not need to patch `GameSave`, manage sidecar file names, handle autosave rotation, or delete matching sidecar files yourself.
- Each mod owns a save segment by GUID. Import/export exceptions are reported to Errors, so mods do not have to share one fragile binary layout.
- `IntoOtherSave()` is called for new games or saves that do not contain your mod data, reducing the risk of state leaking from one save into another.
- Covered legacy DSPModSave handlers bridge into the same SaveRegistry for migration.
- Tagged block helpers let evolving fields be read and written by tag; unknown fields are skipped, lowering version-upgrade cost.

## Capability: Register A Save Handler

Implement `ICoreSaveHandler`, then call during startup or feature registration:

```csharp
Saves.Register("com.example.my-mod", handler, CoreLoadOrder.Postload);
```

`CoreLoadOrder.Preload` imports before the main game load flow, and `CoreLoadOrder.Postload` imports after it. Most ordinary mod state should use `Postload`.

If the same `modGuid` registers more than once, the later registration replaces the earlier one. `modGuid` cannot be empty.

## What DSPCore Does After The Call

- After a successful save, DSPCore writes a matching `.dspcore` file and stores each handler's data range by registered `modGuid`.
- Before loading a save, DSPCore opens the `.dspcore` file and reads its header, then calls `Import` by `CoreLoadOrder`.
- If the current save has no data for a registered mod, DSPCore calls that handler's `IntoOtherSave()`.
- New games call `IntoOtherSave()` for every handler.
- Autosaves rotate matching `.dspcore` files for `_autosave_0`, `_autosave_1`, `_autosave_2`, and `_autosave_3`.
- Deleting a vanilla save deletes the matching `.dspcore` file.

## Capability: Use Tagged Blocks For Evolving Fields

If your save fields may be added, removed, or made optional later, prefer these inside `Export` / `Import`:

```csharp
SaveBlockFormat.WriteBlocks(writer, blocks);
SaveBlockFormat.ReadBlocks(reader, blocks, onBlockError);
```

Each `SaveBlock` has a stable `Tag`. Unknown tags are skipped while reading, and single-block read failures can be handled through `onBlockError`.

## Capability: Legacy DSPModSave Compatibility

Legacy `crecheng.DSPModSave.IModCanSave`, `ModSaveSettingsAttribute`, and `DSPModSavePlugin.AddModSaveManually(...)` remain as obsolete compatibility entries. DSPCore adapts covered legacy handlers to `ICoreSaveHandler`.

New code should use `DSPCore.ICoreSaveHandler` and `Saves.Register(...)` directly.

## What This Block Does Not Own

- It does not design your binary format version for you; complex data should still write a version or use tagged blocks.
- It does not guarantee every legacy DSPModSave edge behavior is reproduced; the compatibility target is to bridge covered handlers into the DSPCore lifecycle.
- It does not automatically restore business state after an `Import` failure; exceptions are reported, but recovery remains the handler's responsibility.
- It does not write `.dspcore` content into the vanilla `.dsv` file; it uses an independent sidecar file.

## Examples

- `Examples/SaveHandler.md`
- `Examples/SaveHandlerExample.cs`
- `Examples/SaveBlocks.md`
- `Examples/SaveBlocksExample.cs`
