# Tech Registration

Techs is the tech proto registration entry point. It only represents the author-facing capability of registering `TechProto`; phase timing is owned by DataPhases, and LDB insertion/cache rebuilds are handled by Systems/ProtoPipeline.

## What This Module Provides

- When code already has a `TechProto`, it can register directly from the object instead of going back through the large ProtoRegistration aggregate entry.
- `Techs.Register(TechProto, ...)` registers the tech and returns the same object, so it can be chained with nearby authoring code.
- `Techs.Register(object, ...)` remains as the low-level entry for compatibility or batch registration.

## Capability: Object-Centric Registration

```csharp
techProto.RegisterTech(
    ownerModGuid: "com.example.my-mod",
    phase: CoreDataPhase.DataUpdates,
    purpose: "Unlock example machine");
```

`RegisterTech(...)` only registers the current tech with DSPCore's ProtoPipeline. Tech fields, unlock tree data, prerequisite techs, and recipe references still need to be prepared by the author according to DSP semantics.

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
