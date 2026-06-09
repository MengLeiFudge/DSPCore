# 核心入口

Core 模块提供 DSPCore 的全局入口、内置功能块注册、模块/功能声明表、运行时启动装配，以及 CommonAPI 旧模块查询的兼容外壳。它是其他功能块的协调层，不承载具体建造栏、存档、图标、成就等业务行为。

## 这个模块带来什么便利

- 你可以通过 `using DSPCore;` 使用短入口，也可以通过 `DspCore` 访问所有全局 registry。
- DSPCore 会在 BepInEx `Awake` 中统一初始化内置功能块和模块声明，再装配各功能块 Harmony patch。
- `Features` 和 `Modules` 能让模组声明自己的功能/模块元数据，便于其他模组查询是否存在。
- `Patches` 能声明由 DSPCore 统一调度的条件补丁，并记录必需插件缺失、版本不足、条件禁用和应用失败。
- `Lifecycle` 能注册 DSPCore 运行时启动、每帧更新、销毁和常见存档链路回调，适合少量框架级初始化、清理或存档前后协调。
- 旧 CommonAPI 的 `CommonAPIPlugin.IsSubmoduleLoaded(...)` 会桥接到 `Modules.TryGet(...)`，方便迁移旧依赖查询。

## 功能：访问全局 registry

`DspCore` 是聚合入口，包含：

- `DspCore.ProtoRegistration`
- `DspCore.BuildBar`
- `DspCore.Saves`
- `DspCore.Resources`
- `DspCore.Icons`
- `DspCore.Tabs`
- `DspCore.GameEnums`
- `DspCore.RecipeTypes`（旧别名）
- `DspCore.KeyBinds`
- `DspCore.Achievements`
- `DspCore.Patches`

日常示例优先使用短入口，例如 `ProtoRegistration.RegisterItem(...)`、`Saves.Register(...)`、`BuildBar.BindQuickBar(...)`、`Patches.Register(...)`。需要访问 registry 快照或聚合服务时再使用 `DspCore`。

## 功能：声明功能块和模块

```csharp
Features.Register(
    id: "com.example.my-mod.machines",
    displayName: "Example Machines",
    initialize: InitializeMachines,
    priority: 100);

Modules.Register(
    id: "com.example.my-mod.compat.target-plugin",
    displayName: "Target Plugin Compatibility",
    initialize: InitializeCompatibility,
    dependencies: new[] { "com.example.my-mod.machines" });
```

`Features.Register(...)` 适合声明功能块级能力；`Modules.Register(...)` 适合声明模组内部或跨模组可查询模块。同一个 ID 后一次注册会覆盖前一次。

`FeatureDescriptor` 和 `ModuleDescriptor` 仍可直接传给对应 `Register(...)`，适合批量构造、配置驱动注册或需要先保存 descriptor 对象的高级路径。

初始化时，DSPCore 会按 priority 和 ID 顺序初始化 feature；module 当前按注册表枚举顺序初始化，不做依赖拓扑排序。

## 功能：声明条件补丁

```csharp
Patches.Register(
    id: "example.core-patch",
    ownerModGuid: "com.example.my-mod",
    apply: ApplyCorePatch,
    description: "Apply example runtime patch.",
    isEnabled: IsFeatureEnabled,
    getDisabledReason: () => "example feature is disabled");

Patches.RegisterForPlugin(
    id: "example.target-plugin-integration",
    ownerModGuid: "com.example.my-mod",
    requiredPluginGuid: "com.example.target-plugin",
    apply: ApplyTargetPluginIntegration,
    description: "Enable integration when the target plugin is loaded.",
    minimumPluginVersion: "1.2.0");
```

`Patches.Register(...)` 是普通作者入口；`Patches.RegisterForPlugin(...)` 适合依赖目标插件的集成补丁。DSPCore 会在 `DSPCorePlugin.Awake()` 装配内置运行时补丁后应用已注册声明，目标插件缺失、版本不足或 `isEnabled` 返回 false 时会记录禁用原因并跳过 `apply`。

`DspCore.Patches.Register(new PatchDescriptor(...))` 仍保留为低层 registry 路径，适合批量注册或已有 descriptor 流程。具体 Harmony patch 代码仍应留在所属系统或模组自己的运行时代码中。

## 功能：注册 DSPCore 生命周期回调

```csharp
Lifecycle.OnStarted(InitializeAfterDspCore);
Lifecycle.OnUpdate(PollSmallRuntimeState);
Lifecycle.OnDestroyed(DisposeState);
Lifecycle.OnBeforeSave(saveName => FlushRuntimeCache(saveName));
Lifecycle.OnAfterLoad(RebuildRuntimeCache);
```

`OnStarted` 会在 DSPCore 插件入口完成运行时桥接装配后触发；如果注册时 DSPCore 已经启动，会立即执行一次。`OnUpdate` 跟随 DSPCore 插件每帧更新；`OnDestroyed` 在 DSPCore 插件销毁时触发。

存档链路事件包括 `OnNewGame`、`OnBeforeSave(saveName)`、`OnBeforeLoad(saveName)`、`OnAfterLoad` 和 `OnSaveDeleted(saveName)`。这些事件来自 DSPCore 已经接入的 `GameData` / `GameSave` / 存档删除桥，适合刷新缓存、释放临时索引或同步非持久状态；复杂持久化仍应使用 `Saves`。

框架启动/销毁事件不表示某个具体游戏存档、星球、工厂或 UI surface 已经存在。

## 功能：CommonAPI 兼容查询

旧代码调用：

```csharp
CommonAPI.CommonAPIPlugin.IsSubmoduleLoaded("example.module")
```

会转到 `DSPCore.Modules.TryGet(...)`。`CommonAPISubmoduleDependencyAttribute` 也保留为 obsolete 兼容类型，但不会复刻 CommonAPI 的完整子模块扫描、版本约束和加载器行为。

## 这个模块不负责什么

- 不放具体功能行为；存档、图标、建造栏、成就等逻辑必须留在所属作者能力和系统集成目录。
- 不替模组自动解析模块依赖图；`Dependencies` 目前是描述数据。
- 不承载具体功能补丁实现；`PatchRegistry` 只保存声明，具体 Harmony patch 代码仍应由所属系统或模组自己的运行时代码提供。
- 不应新增集中式旧兼容目录；旧 API shim 必须放回所属作者能力的 `Compat/`。

## 运行时启动

`DSPCorePlugin.Awake()` 会初始化 DSPCore、注册旧 DSPModSave 处理器、创建 Harmony，并装配当前已实现的 Proto、BuildBar、Saves、Achievements、Errors、Localization、Tabs、GameEnums patch，然后触发 `Lifecycle.OnStarted`。`Update()` 会轮询 KeyBinds、picker surface 和 `Lifecycle.OnUpdate`。存档链路事件由 `SaveRuntime` 在对应 `GameSave` / `GameData` patch 中转发。

## 示例

- `Examples/Lifecycle.md`
- `Examples/LifecycleExample.cs`
- `Examples/ModuleDeclaration.md`
- `Examples/ModuleDeclarationExample.cs`
- `Examples/PatchPlatform.md`
- `Examples/PatchPlatformExample.cs`
