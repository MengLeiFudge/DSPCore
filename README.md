# DSPCore

DSPCore is a new common framework standard for Dyson Sphere Program mods.

DSPCore 是戴森球计划模组的新通用底层标准。

## Goals / 目标

- Provide one shared API for common mod infrastructure.
- 为常见模组基础设施提供统一 API。

- Replace scattered requirements such as LDBTool, DSPModSave, CommonAPI, BuildBarTool, ErrorAnalyzer-style diagnostics, achievement policy aggregation, icon registration, and compatibility patches.
- 逐步替代分散的 LDBTool、DSPModSave、CommonAPI、BuildBarTool、ErrorAnalyzer 式诊断、成就策略聚合、图标注册和兼容补丁需求。

- Keep legacy API namespaces available as `[Obsolete]` compatibility shims, so existing mods can run first and migrate later.
- 保留旧 API 命名空间作为 `[Obsolete]` 兼容层，让已有模组可以先运行，再逐步迁移。

## Package / 包

- Thunderstore package name: `MengLei-DSPCore`
- Thunderstore 包名：`MengLei-DSPCore`

- DLL name: `DSPCore.dll`
- DLL 名称：`DSPCore.dll`

- New namespace: `DSPCore`
- 新命名空间：`DSPCore`

## First Version Scope / 初版范围

- P0/P1 author-facing feature blocks: feature lifecycle, data phases, item/recipe/tech/tutorial registration, build bar placement, resources, icons, localization, tabs, pickers, recipe types, key binds, saves, achievements, and error reports.
- 提供 P0/P1 的作者可见功能块：功能生命周期、数据阶段、物品/配方/科技/指引注册、建造栏位置、资源、图标、本地化、分页、选择器、配方类型、按键、存档、成就和错误报告。

- Legacy compatibility shims for `xiaoye97.LDBTool`, `crecheng.DSPModSave`, `CommonAPI`, and `BuildBarTool`.
- 提供 `xiaoye97.LDBTool`、`crecheng.DSPModSave`、`CommonAPI` 和 `BuildBarTool` 的旧 API 兼容层。

- Bilingual XML summaries for public APIs.
- 公开 API 提供中英文 XML summary。

The current version is still an API and documentation preview. Runtime BepInEx/Harmony wiring and full behavior migration will be implemented feature by feature.

当前版本仍是 API 和文档预览。BepInEx/Harmony 运行时接入和完整行为迁移会按功能块逐步实现。

## Feature Blocks / 功能块

P0/P1 blocks are the current implementation target.

P0/P1 是当前实现目标。

- Feature lifecycle: declare feature blocks, dependencies, priority, and initialization.
- 功能生命周期：声明功能块、依赖、优先级和初始化。

- Data phases: `Data`, `DataUpdates`, and `DataFinalFixes`.
- 数据阶段：`Data`、`DataUpdates` 和 `DataFinalFixes`。

- Proto features: item, recipe, tech, tutorial, model/building binding, and vanilla data query descriptors.
- 原型功能：物品、配方、科技、指引、模型/建筑绑定和原版数据查询描述。

- Build bar placement: multi-layer placement and legacy BuildBarTool tier bridging.
- 建造栏位置：多层位置声明和旧 BuildBarTool 层级桥接。

- Resources, icons, and localization: resource roots, icon descriptors, and translation entries.
- 资源、图标和本地化：资源根、图标描述和翻译条目。

- Tabs and pickers: authors declare tabs and picker requests; runtime adapters will project them to item, recipe, replicator, signal, beacon, blueprint, and other UI surfaces.
- 分页和选择器：作者声明分页和选择器请求；运行时适配层会投射到物品、配方、制造器、信号、信标、蓝图等 UI 表面。

- Saves: raw `BinaryReader`/`BinaryWriter` handlers and tagged block helpers.
- 存档：原始 `BinaryReader`/`BinaryWriter` 处理器和 tagged block 工具。

- Achievements and errors: achievement policy aggregation and structured error reports.
- 成就和错误：成就策略聚合和结构化错误报告。

P2/P3 blocks such as custom machine components, planet/star systems, network helpers, and player convenience modules are TODO and not implemented yet.

P2/P3 的自定义机器组件、星球/恒星系统、网络工具和玩家便利模块仍是 TODO，尚未实现。

## Example: Achievement Policy / 示例：成就策略

```csharp
using DSPCore;

DspCore.Achievements.Declare(new AchievementPolicyDeclaration(
    ModGuid: "com.example.my-mod",
    DisableAchievements: true,
    Reason: "Changes production balance",
    SourceVersion: "1.0.0"));

bool disabled = DspCore.Achievements.ShouldDisableAchievements();
```

```csharp
using DSPCore;

DspCore.Achievements.Declare(new AchievementPolicyDeclaration(
    ModGuid: "com.example.my-mod",
    DisableAchievements: true,
    Reason: "改变生产平衡",
    SourceVersion: "1.0.0"));

bool disabled = DspCore.Achievements.ShouldDisableAchievements();
```

## Example: Build Bar / 示例：建造栏

```csharp
using DSPCore;

DspCore.BuildBar.SetBuildBar(
    category: 3,
    index: 4,
    itemId: 9554,
    layer: 2);
```

## Example: Tabs / 示例：分页

```csharp
using DSPCore;

DspCore.Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "Example Machines",
    IconId: "example-machine-tab",
    Order: 100));
```

## Example: Legacy BuildBarTool Compatibility / 示例：旧 BuildBarTool 兼容

```csharp
#pragma warning disable CS0618
BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
#pragma warning restore CS0618
```

The old call is accepted, but it is marked obsolete. New mods should use `DSPCore.DspCore.BuildBar`.

旧调用会被接受，但会标记为 obsolete。新模组应使用 `DSPCore.DspCore.BuildBar`。

## Documentation / 文档

- `docs/getting-started.md`
- `docs/api-migration.md`
- `docs/examples/achievement-policy.md`
- `docs/examples/build-bar.md`
- `docs/examples/save-handler.md`
- `docs/examples/save-blocks.md`
- `docs/examples/iconset.md`
- `docs/examples/tabs.md`
- `docs/examples/proto-phases.md`
- `docs/examples/key-bind.md`
- `docs/examples/compatibility-patch.md`
