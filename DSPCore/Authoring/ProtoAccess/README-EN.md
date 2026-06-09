# Proto Access

ProtoAccess owns the author-facing capability of inspecting and modifying data registered by other mods during the second and third data phases.

## What This Module Provides

- `ProtoPhaseContext` exposes lookup access to protos visible in the current phase.
- Common lookups can use `data.FindItem(...)`, `data.FindRecipe(...)`, `data.FindTech(...)`, and `data.FindTutorial(...)`.
- Use `data.Access` when you need enumeration or phase-boundary checks, such as `data.Access.Items()` and `data.Access.CanMutate`.
- Returned values are the proto object references used by the current phase. Mutating them in `DataUpdates` and `DataFinalFixes` takes effect before DSPCore writes to LDB or rebuilds derived caches.

## Capability: Adjust Data In DataUpdates / DataFinalFixes

```csharp
ProtoRegistration.DataUpdates("com.example.my-mod", data =>
{
    ItemProto item = data.FindItem(1001);
    if (item != null)
    {
        item.GridIndex = GridIndexes.From(tab: 3, row: 1, index: 5);
    }
});
```

`Find*` first checks DSPCore registrations visible to the current phase, then falls back to the current LDB object. Use it for cross-mod adjustments to items, recipes, techs, or tutorials declared by earlier phases.

## Boundaries

- `Data` is still mainly for declaring new protos. Cross-mod mutation belongs in `DataUpdates` or `DataFinalFixes`.
- `CanMutate` expresses phase semantics only; it does not freeze returned objects. Authors still need to follow phase rules.
- `Items()` / `Recipes()` / `Techs()` / `Tutorials()` merge current LDB objects and visible registrations by ID; later registered objects with the same ID win.
- `VanillaDataView` remains a descriptive read request model and is not the recommended direct LDB query API.

## Examples

- `Examples/ProtoAccess.md`
- `Examples/ProtoAccessExample.cs`
