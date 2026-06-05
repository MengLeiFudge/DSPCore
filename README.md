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

- New API skeleton for modules, patches, saves, protos, build bar, icons, achievements, error reports, UI nodes, and compatibility patches.
- 提供模块、补丁、存档、Proto、建造栏、图标、成就、错误报告、UI 节点和兼容补丁的新 API 骨架。

- Legacy compatibility shims for `xiaoye97.LDBTool`, `crecheng.DSPModSave`, `CommonAPI`, and `BuildBarTool`.
- 提供 `xiaoye97.LDBTool`、`crecheng.DSPModSave`、`CommonAPI` 和 `BuildBarTool` 的旧 API 兼容层。

- Bilingual XML summaries for public APIs.
- 公开 API 提供中英文 XML summary。

The first version is an API and documentation preview. Runtime BepInEx/Harmony wiring and full behavior migration will be implemented in later steps.

初版是 API 和文档预览。BepInEx/Harmony 运行时接入和完整行为迁移会在后续步骤实现。

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
    tier: BuildBarTier.Secondary);
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
- `docs/examples/iconset.md`
- `docs/examples/compatibility-patch.md`
