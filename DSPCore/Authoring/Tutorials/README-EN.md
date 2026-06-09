# Tutorial Registration

Tutorials is the guide/tutorial proto registration entry point. It only represents the author-facing capability of registering `TutorialProto` or equivalent guide protos; phase timing is owned by DataPhases, and LDB insertion/cache rebuilds are handled by Systems/ProtoPipeline.

## What This Module Provides

- When code already has a `TutorialProto`, it can register directly from the object instead of going back through the ProtoRegistration aggregate entry.
- `Tutorials.Register(TutorialProto, ...)` registers the guide and returns the same object, so it can be chained with nearby field configuration.
- `Tutorials.Register(object, ...)` remains as the low-level entry for compatibility or batch registration.

## Capability: Object-Centric Registration

```csharp
tutorialProto.RegisterTutorial(
    ownerModGuid: "com.example.my-mod",
    phase: CoreDataPhase.DataFinalFixes,
    purpose: "Show example machine guide");
```

`RegisterTutorial(...)` only registers the current guide with DSPCore's ProtoPipeline. Guide content, trigger conditions, chain relationships, and localization keys still need to be prepared by the author according to DSP semantics.

## Capability: Low-Level Registration

```csharp
Tutorials.Register(tutorialProto, "com.example.my-mod", CoreDataPhase.DataFinalFixes, "Show example machine guide");
```

When a data phase callback declares several protos together, `ProtoPhaseContext.RegisterTutorial(...)` also remains available.

## Boundaries

- This module does not create `TutorialProto` objects automatically.
- Items, recipes, techs, and localization text belong to Items, Recipes, Techs, and Resources.
- Runtime business UI and player navigation logic are outside this module.

## Examples

- `Examples/TutorialAuthoring.md`
- `Examples/TutorialAuthoringExample.cs`
