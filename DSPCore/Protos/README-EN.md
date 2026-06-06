# Protos

The Protos block lets a mod register DSP protos such as `ItemProto`, `RecipeProto`, `TechProto`, and `TutorialProto` into DSPCore data phases. DSPCore then writes them to LDB at a shared runtime point and rebuilds derived caches.

## What This Block Gives You

- You do not need every mod to find its own LDB insertion timing, mutate `ProtoSet` manually, or remember every cache rebuild.
- You can express ordering through `Data`, `DataUpdates`, and `DataFinalFixes`: declare first, adjust across mods, then apply final fixes.
- After applying protos, DSPCore rebuilds key item, model, recipe, signal, icon, and index caches, reducing cases where data exists but UI or derived indices are stale.
- Legacy LDBTool `PreAddProto` / `PostAddProto` calls bridge to Protos for migration.

## Capability: Register New Protos

Common typed entries:

```csharp
Protos.RegisterItem(itemProto, "com.example.my-mod");
Protos.RegisterRecipe(recipeProto, "com.example.my-mod", CoreDataPhase.DataUpdates);
Protos.RegisterTech(techProto, "com.example.my-mod");
Protos.RegisterTutorial(tutorialProto, "com.example.my-mod");
```

Use the generic entry when you need to specify the type or record a purpose:

```csharp
Protos.Register(typeof(ItemProto), itemProto, "com.example.my-mod", CoreDataPhase.Data, ProtoKind.Item, "new building item");
```

## Capability: Choose A Data Phase

- `CoreDataPhase.Data`: initial declarations, suitable for new protos and base fields.
- `CoreDataPhase.DataUpdates`: cross-mod data updates, suitable for changes that depend on other declarations.
- `CoreDataPhase.DataFinalFixes`: final fixes before derived caches are rebuilt.

Do not put everything into `DataFinalFixes`. Use it only for work that truly depends on earlier declarations being complete.

## What DSPCore Does After The Call

- Registration only records `ProtoType`, `Proto`, `ownerModGuid`, phase, kind, and purpose.
- Runtime reads registrations by phase and groups them by concrete Proto type before writing them to the matching LDB `ProtoSet`.
- If no matching LDB `ProtoSet` is found, that group is skipped and logged.
- Each phase is applied only once, so repeated runtime triggers do not reinsert the same phase.
- After final fixes, DSPCore rebuilds LDB indices, item derived caches, model derived caches, recipe derived caches, custom recipe types, signal indices, and icon caches.

## Capability: Describe Vanilla Data Reads

`DspCore.Vanilla.GetItem(...)`, `GetRecipe(...)`, and `GetTech(...)` currently return read descriptors that record who wants which vanilla data. They are not direct LDB object query APIs.

## What This Block Does Not Own

- It does not own build bar placement; call BuildBar after creating items.
- It does not own icon resource loading; call Icons when you need icon binding.
- It does not own localization strings; call Resources for text.
- It does not own custom recipe type restrictions; call RecipeTypes when you need machine-use guards.
- The current proto phase hook is a conservative first bridge, not the final VFPreload mid-stage lifecycle.

## Examples

- `Examples/ProtoPhases.md`
- `Examples/ProtoPhasesExample.cs`
