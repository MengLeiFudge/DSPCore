# Proto Registration

The ProtoRegistration block lets a mod register DSP protos such as `ItemProto`, `RecipeProto`, `TechProto`, and `TutorialProto` into DSPCore data phases. DSPCore then writes them to LDB at a shared runtime point and rebuilds derived caches.

## What This Block Gives You

- You do not need every mod to find its own LDB insertion timing, mutate `ProtoSet` manually, or remember every cache rebuild.
- You can express ordering through `Data`, `DataUpdates`, and `DataFinalFixes` callbacks: declare first, adjust across mods, then apply final fixes. This is close to Factorio's data.lua / data-updates.lua / data-final-fixes.lua flow.
- After applying protos, DSPCore rebuilds key item, model, recipe, signal, icon, and index caches, reducing cases where data exists but UI or derived indices are stale.
- Legacy LDBTool `PreAddProto` / `PostAddProto` calls bridge to ProtoRegistration for migration.
- `Protos` remains as a compatibility/short alias; new documentation and examples prefer `ProtoRegistration`.

## Capability: Register Data Phase Callbacks

The recommended pattern is to register data phase callbacks during startup. DSPCore passes a `ProtoPhaseContext` when the phase runs, and you register items, recipes, techs, or final fixes inside that callback.

```csharp
ProtoRegistration.Data("com.example.my-mod", data =>
{
    data.RegisterItem(itemProto, "Declare base item");
});

ProtoRegistration.DataUpdates("com.example.my-mod", data =>
{
    data.RegisterRecipe(recipeProto, "Attach recipe after item declarations");
});

ProtoRegistration.DataFinalFixes("com.example.my-mod", data =>
{
    data.RegisterTutorial(tutorialProto, "Final tutorial chain fix");
});
```

Use `priority` to adjust order inside the same phase. Lower values run earlier; equal priorities keep registration order.

## Capability: Directly Register New Proto Objects

Typed direct entries remain available for compatibility and simple declarations. New code that needs data lifecycle semantics should prefer the callback pattern above.

```csharp
ProtoRegistration.RegisterItem(itemProto, "com.example.my-mod");
ProtoRegistration.RegisterRecipe(recipeProto, "com.example.my-mod", CoreDataPhase.DataUpdates);
ProtoRegistration.RegisterTech(techProto, "com.example.my-mod");
ProtoRegistration.RegisterTutorial(tutorialProto, "com.example.my-mod");
```

Use the generic direct entry when you need to specify the type or record a purpose:

```csharp
ProtoRegistration.Register(typeof(ItemProto), itemProto, "com.example.my-mod", CoreDataPhase.Data, ProtoKind.Item, "new building item");
```

## Capability: Set Item Or Recipe Cells

`GridIndex` is the native game field on `ItemProto` and `RecipeProto`. It decides the cell where an item or recipe appears inside its page.

If the item or recipe should appear on a DSPCore custom page, register the page through Tabs first, then generate the `GridIndex` from the returned `TabSlot`:

```csharp
TabSlot machinesTab = Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "ExampleMachines",
    IconId: "example-machines-icon"));

itemProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
recipeProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
```

If the item or recipe should stay on a vanilla page, use the vanilla tab category value directly:

```csharp
itemProto.GridIndex = ProtoRegistration.GetGridIndex(tab: 1, row: 2, index: 3);
```

`TabSlot` is the page slot. `GridIndex` is the item or recipe cell field. Do not treat them as the same concept.

## Capability: Choose A Data Phase

- `CoreDataPhase.Data` / `ProtoRegistration.Data(...)`: initial declarations, suitable for new protos and base fields.
- `CoreDataPhase.DataUpdates` / `ProtoRegistration.DataUpdates(...)`: cross-mod data updates, suitable for changes that depend on other declarations.
- `CoreDataPhase.DataFinalFixes` / `ProtoRegistration.DataFinalFixes(...)`: final fixes before derived caches are rebuilt.

Do not put everything into `DataFinalFixes`. Use it only for work that truly depends on earlier declarations being complete.

## What DSPCore Does After The Call

- Startup only records data phase callbacks and direct registrations; it does not write LDB immediately.
- When runtime enters a phase, it first executes callbacks for that phase by `priority` and registration order.
- Protos registered through `ProtoPhaseContext` are assigned to the currently executing phase.
- Runtime then reads registrations by phase and groups them by concrete Proto type before writing them to the matching LDB `ProtoSet`.
- If no matching LDB `ProtoSet` is found, that group is skipped and logged.
- Each phase is applied only once, so repeated runtime triggers do not reinsert the same phase.
- After final fixes, DSPCore rebuilds LDB indices, item derived caches, model derived caches, recipe derived caches, custom recipe types, signal indices, and icon caches.

## Capability: Describe Vanilla Data Reads

`DspCore.Vanilla.GetItem(...)`, `GetRecipe(...)`, and `GetTech(...)` currently return read descriptors that record who wants which vanilla data. They are not direct LDB object query APIs.

## What This Block Does Not Own

- It does not own build bar placement; call BuildBar after creating items.
- It does not own icon resource loading; call Icons when you need icon binding.
- It does not own localization strings; call Resources for text.
- It does not allocate page slots; call Tabs when you need a new page, then use the returned `TabSlot` to generate item or recipe `GridIndex` values.
- It does not own custom recipe type restrictions; call RecipeTypes when you need machine-use guards.
- The current proto phase hook is a conservative first bridge, not the final VFPreload mid-stage lifecycle.

## Examples

- `Examples/ProtoPhases.md`
- `Examples/ProtoPhasesExample.cs`
