# Tech Registration

Techs is the tech proto registration entry point. It only represents the author-facing capability of registering `TechProto`; phase timing is owned by DataPhases, and LDB insertion/cache rebuilds are handled by Systems/ProtoPipeline.

## What This Module Provides

- When code already has a `TechProto`, it can register directly from the object instead of going back through the large ProtoRegistration aggregate entry.
- Common tech fields have chainable entries: `SetIconTag(...)`, `SetHidden(...)`, `SetPreTechsImplicit(...)`, `AddPreTechsImplicit(...)`, `GrantItems(...)`, and `SetPropertyOverrideItems(...)`.
- `Techs.Register(TechProto, ...)` registers the tech and returns the same object, so it can be chained with nearby authoring code.
- `Techs.Register(object, ...)` remains as the low-level entry for compatibility or batch registration.

## Capability: Object-Centric Registration

```csharp
techProto
    .SetIconTag("example-tech")
    .SetHidden()
    .SetPreTechsImplicit(1001)
    .GrantItems(new[] { 9554 }, new[] { 1 })
    .SetPropertyOverrideItems(new[] { 6001 }, new[] { 200 })
    .RegisterTech(
        ownerModGuid: "com.example.my-mod",
        phase: CoreDataPhase.DataUpdates,
        purpose: "Unlock example machine");
```

`SetHidden(...)` writes vanilla `TechProto.IsHiddenTech`. `SetPreTechsImplicit(...)` replaces the implicit prerequisite array, while `AddPreTechsImplicit(...)` appends to it. `GrantItems(...)` writes `AddItems` / `AddItemCounts`, and `SetPropertyOverrideItems(...)` writes `PropertyOverrideItems` / `PropertyItemCounts`. `RegisterTech(...)` only registers the current tech with DSPCore's ProtoPipeline; fields that are not covered by helpers can still be set directly by the author.

## Capability: Low-Level Registration

```csharp
Techs.Register(techProto, "com.example.my-mod", CoreDataPhase.DataUpdates, "Unlock example machine");
```

When a data phase callback declares several protos together, `ProtoPhaseContext.RegisterTech(...)` also remains available.

## Boundaries

- This module does not create `TechProto` objects automatically.
- Items, recipes, tutorials, and UI pages belong to Items, Recipes, Tutorials, and UI.
- Cross-mod tech lookup or mutation belongs in DataUpdates / DataFinalFixes through `ProtoPhaseContext.Access`.

## Examples

- `Examples/TechAuthoring.md`
- `Examples/TechAuthoringExample.cs`
