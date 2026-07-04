# Saves

The Saves block lets a mod store its own state in DSPCore `.dspcore` sidecar save files and follow vanilla new-game, load, save, delete-save, and autosave rotation flows. It also provides `Saves.Global...` entries for cross-save state.

## What This Block Gives You

- You do not need to patch `GameSave`, manage sidecar file names, handle autosave rotation, or delete matching sidecar files yourself.
- Each mod owns a save segment by GUID. Import/export exceptions are reported to Errors, so mods do not have to share one fragile binary layout.
- `IntoOtherSave()` is called for new games or saves that do not contain your mod data, reducing the risk of state leaking from one save into another.
- Covered legacy DSPModSave handlers bridge into the same SaveRegistry for migration.
- `Saves.Auto<TState>(...)` can create and save simple state objects marked with `[CoreSaveField]`; pass an existing instance when defaults or dependencies must be prepared first.
- `Saves.GlobalAuto<TState>(...)` / `GlobalRegister(...)` store cross-save data, suitable for achievement statistics, global unlock records, or player-profile-level state. It is not Config; DSPCore exposes only a read-only metadata page, not editable UI.
- Tagged block helpers let evolving fields be read and written by tag; unknown fields are skipped, lowering version-upgrade cost.

## Capability: Save Simple State With An Automatic Schema

```csharp
private sealed class ExampleState
{
    [CoreSaveField("counter")]
    public int Counter { get; set; }

    [CoreSaveField("enabled")]
    public bool Enabled = true;
}

private static readonly ExampleState State = Saves.Auto<ExampleState>("com.example.my-mod");
```

`Saves.Auto<TState>(...)` creates the state object with a parameterless constructor and registers it as a save handler. Export writes the schema version and each `[CoreSaveField]` member. Import restores the defaults captured during registration first, then reads existing fields by tag; missing fields keep their defaults. Pass `migrate: (version, state) => { ... }` when older versions need migration.

If the state object needs constructor arguments, dependency injection, or prepared defaults before registration, use the instance overload: `Saves.Auto("com.example.my-mod", state)`.

The current automatic schema supports only `bool`, `int`, `long`, `float`, `double`, `string`, and enums. Complex collections, dictionaries, nested objects, and Unity types should still use delegates, a full handler, or tagged blocks.

## Capability: Save Cross-Save Global Data

Global data uses the same `[CoreSaveField]` and `ICoreSaveHandler` rules, but writes to `DSPCore/GlobalSaves.dspcore` under the BepInEx config directory. It does not rotate with or get deleted alongside a specific game save.

```csharp
private sealed class GlobalState
{
    [CoreSaveField("total_runs")]
    public int TotalRuns { get; set; }
}

private static readonly GlobalState Global = Saves.GlobalAuto<GlobalState>(
    "com.example.my-mod",
    initialize: state => state.TotalRuns = 0);
```

Delegate style:

```csharp
Saves.GlobalRegister(
    modGuid: "com.example.my-mod",
    export: writer => writer.Write(totalRuns),
    import: reader => totalRuns = reader.ReadInt32(),
    initialize: () => totalRuns = 0);
```

Global data is imported when DSPCore starts. If a mod registers a global handler later, DSPCore immediately imports existing data for that handler or calls its initializer. DSPCore saves global data once during plugin shutdown; call `Saves.SaveGlobal()` when data must be flushed immediately. Players can open the read-only Global Data page from the DSPCore unified settings window to inspect registered handlers, persisted file-only blocks, and byte counts, but cannot edit content through the UI.

## Capability: Register A Save Handler

Simple state can register delegates directly without declaring a handler class:

```csharp
Saves.Register(
    modGuid: "com.example.my-mod",
    export: writer => writer.Write(counter),
    import: reader => counter = reader.ReadInt32(),
    intoOtherSave: () => counter = 0);
```

For complex state, implement `ICoreSaveHandler`, then call during startup or feature registration:

```csharp
Saves.Register("com.example.my-mod", handler, CoreLoadOrder.Postload);
```

`CoreLoadOrder.Preload` imports before the main game load flow, and `CoreLoadOrder.Postload` imports after it. Most ordinary mod state should use `Postload`.

If the same `modGuid` registers more than once, the later registration replaces the earlier one. `modGuid` cannot be empty.

## What DSPCore Does After The Call

- After a successful save, DSPCore writes a matching `.dspcore` file and stores each handler's data range by registered `modGuid`.
- Global saves use `DSPCore/GlobalSaves.dspcore` and store one data block per registered global handler `modGuid`; the unified settings window shows only block metadata and does not display raw binary content.
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
- `Saves.Global...` is not Config: it does not expose editable UI, and players should not hand-edit the global binary file.

## Examples

- `Examples/SaveHandler.md`
- `Examples/SaveHandlerExample.cs`
- `Examples/SaveBlocks.md`
- `Examples/SaveBlocksExample.cs`
