# 实体组件

Components 模块让模组把自定义逻辑挂到星球工厂里的具体实体上。作者可用 `Components.Register<TComponent>(...)` 注册带无参构造函数的组件；需要完整上下文构造时再声明 `ComponentDescriptor`。DSPCore 会在实体创建时按 item id、model index 或 `PrefabDesc` 条件创建组件，并在实体移除、工厂 tick 和存档导入导出时转发生命周期。

## 这个模块带来什么便利

- 不需要每个模组都 patch `PlanetFactory.CreateEntityLogicComponents` 和 `RemoveEntityWithComponents`。
- 同一实体可以承载多个稳定 ID 的 DSPCore 组件，组件状态写入 `.dspcore` sidecar。
- 存档导入时如果星球工厂尚未加载，DSPCore 会先保留 pending 数据，并在 `GameData.GetOrCreateFactory` 后恢复。
- Preloader 注入的 `EntityData.customId/customType/customData` 只作为辅助 marker；组件状态仍以 DSPCore sidecar 为事实源。

## 功能：短入口注册组件

```csharp
Components.Register<ExampleCounterComponent>(
    componentId: "com.example.counter",
    ownerModGuid: "com.example.my-mod",
    itemId: 9554);
```

`Register<TComponent>(...)` 会用无参构造函数创建组件，并把 `componentId`、`PlanetFactory` 和实体 ID 写入组件上下文。需要根据 `PlanetFactory`、entity id、`PrefabDesc` 或 prebuild id 自定义构造对象时，使用 `Components.Register(new ComponentDescriptor(...))`。

## 边界

- 组件不会自动创建游戏原生 `AssemblerComponent`、`StationComponent` 等原版组件。
- 组件 ID 必须长期稳定；改 ID 等于创建全新存档块。
- 当前生命周期覆盖电力阶段、工厂逻辑阶段和后置阶段，不承诺替代所有原版组件内部 tick。

## 示例

- `Examples/EntityComponent.md`
- `Examples/EntityComponentExample.cs`
