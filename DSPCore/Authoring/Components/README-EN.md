# Entity Components

The Components module lets mods attach custom logic to concrete entities inside a planet factory. Authors can use `Components.Register<TComponent>(...)` for components with parameterless constructors, and use `ComponentDescriptor` when construction needs full context. DSPCore creates components when matching entities are built, then forwards removal, factory ticks, and save import/export lifecycle calls.

## What This Module Provides

- Mods do not need to patch `PlanetFactory.CreateEntityLogicComponents` and `RemoveEntityWithComponents` individually.
- One entity can host multiple DSPCore components identified by stable IDs, and component state is stored in the `.dspcore` sidecar.
- If a planet factory is not loaded during save import, DSPCore keeps pending data and restores it after `GameData.GetOrCreateFactory`.
- Preloader-injected `EntityData.customId/customType/customData` fields are auxiliary markers only; the sidecar remains the state source of truth.

## Capability: Short Component Registration

```csharp
Components.Register<ExampleCounterComponent>(
    componentId: "com.example.counter",
    ownerModGuid: "com.example.my-mod",
    itemId: 9554);
```

`Register<TComponent>(...)` creates the component with a parameterless constructor and writes `componentId`, `PlanetFactory`, and entity id into the component context. Use `Components.Register(new ComponentDescriptor(...))` when construction needs `PlanetFactory`, entity id, `PrefabDesc`, or prebuild id directly.

## Boundaries

- Components do not create vanilla `AssemblerComponent`, `StationComponent`, or other native components automatically.
- Component IDs must stay stable. Changing an ID creates a new save block.
- The current lifecycle covers power, factory logic, and post phases; it is not a replacement for every vanilla component internal tick.

## Examples

- `Examples/EntityComponent.md`
- `Examples/EntityComponentExample.cs`
