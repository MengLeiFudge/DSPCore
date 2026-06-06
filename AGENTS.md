# AGENTS.md - DSPCore Development Guide

This file is the long-term workflow source for AI agents working in this repository.

本文档是 AI agent 在本仓库工作的长期工作流事实源。

## Project Overview / 项目概览

- Project name: `DSPCore`
- 项目名：`DSPCore`

- Thunderstore package target: `MengLei-DSPCore`
- Thunderstore 包名目标：`MengLei-DSPCore`

- Root namespace: `DSPCore`
- 根命名空间：`DSPCore`

- DLL name: `DSPCore.dll`
- DLL 名称：`DSPCore.dll`

DSPCore is a new common framework standard for Dyson Sphere Program mods. It is not a personal namespace library and must not use `MLJ` in new public namespaces.

DSPCore 是戴森球计划模组的新通用底层标准。它不是个人命名空间库，新公开命名空间不得使用 `MLJ`。

- Code projects: `DSPCore`, `DSPCore.Preloader`, and `DSPCore.Packaging`.
- 代码项目：`DSPCore`、`DSPCore.Preloader` 和 `DSPCore.Packaging`。

## Architecture Rules / 架构规则

- New implementation code uses the `DSPCore` namespace.
- 新实现代码使用 `DSPCore` 命名空间。

- Legacy namespaces are compatibility shims only and must be marked `[Obsolete]`.
- 旧命名空间只作为兼容 shim，必须标记 `[Obsolete]`。

- Compatibility code belongs to the owning feature block under `DSPCore/<Feature>/Compat/`; do not create a top-level `Legacy/` or centralized legacy compatibility directory. Old namespaces may be declared from feature `Compat/` files when required by source compatibility.
- 兼容代码必须放在所属功能块的 `DSPCore/<Feature>/Compat/` 下；禁止创建顶层 `Legacy/` 或集中式旧兼容目录。为了源码兼容需要保留旧命名空间时，也应在对应功能块的 `Compat/` 文件中声明。

- First-version compatibility target: existing mods should be able to reference DSPCore without changing source code where the old API surface is covered.
- 初版兼容目标：已覆盖旧 API 面的已有模组，应能在不改源码的情况下引用 DSPCore。

- Do not make legacy namespaces the internal design language.
- 不要把旧命名空间作为内部设计语言。

- Public API must include bilingual XML summary: one Chinese sentence and one English sentence.
- 公开 API 必须包含中英文 XML summary：一句中文，一句英文。

- Author-facing API names must be checked against the source mod ecosystem before promotion in examples or README. When the caller already has a domain object, prefer a concise object extension method over a registry-style static call.
- 面向作者的 API 命名在写入示例或 README 前必须对照来源模组生态复查。调用方已经持有领域对象时，优先提供简洁的对象扩展方法，而不是要求作者绕到 registry 风格静态调用。

## Build Commands / 构建命令

Primary verification:

```bash
dotnet build DSPCore.sln
```

Expected result:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

构建要求：

- Target framework is `net472`.
- 目标框架是 `net472`。

- Build output is fixed under `bin/` and `obj/`; do not commit build outputs.
- 构建产物固定在 `bin/` 和 `obj/`；不要提交构建产物。

- `Directory.Build.props` owns shared build settings.
- `Directory.Build.props` 管理共享构建设置。

- `DefaultPath.props` is local-only and ignored by Git. Update `DefaultPath.props.example` when shared path variables change.
- `DefaultPath.props` 只用于本机，并被 Git 忽略。共享路径变量变化时更新 `DefaultPath.props.example`。

## Documentation Rules / 文档规则

- README.md is the Chinese document for users and mod authors; README-EN.md is the matching English document.
- README.md 是面向玩家和模组作者的中文说明；README-EN.md 是对应英文说明。

- AGENTS.md is for repository workflow, validation, and AI rules.
- AGENTS.md 面向仓库工作流、验证要求和 AI 规则。

- Legacy API migration notes belong in `README.md`, `README-EN.md`, or the owning feature block README until a real cross-feature documentation need appears.
- 旧 API 迁移说明先放在 `README.md`、`README-EN.md` 或所属功能块 README；只有出现真实跨功能长文档需求时才恢复根 `docs/`。

- Every public capability should have a concrete `.cs` example under its feature block's `Examples/` directory. All example `.cs` files are documentation artifacts and must be excluded from compilation.
- 每一项公开能力都应在所属功能块的 `Examples/` 目录下有具体 `.cs` 示例。所有示例 `.cs` 文件都属于文档产物，必须排除编译。

- Feature-specific examples should use paired scenario files when practical: `Examples/<Scenario>.md` explains when to use the API, key parameters, runtime prerequisites, and common mistakes; `Examples/<Scenario>Example.cs` provides the small demo code. The `.md` explanation does not replace the feature block README, and the `.cs` demo remains excluded from compilation.
- 功能专属示例在可行时应使用成对场景文件：`Examples/<Scenario>.md` 说明适用时机、关键参数、运行时前提和常见误用；`Examples/<Scenario>Example.cs` 提供小型 demo 代码。`.md` 说明不替代功能块 README，`.cs` demo 仍必须排除编译。

- Every feature block directory under `DSPCore/` must have `README.md` for Chinese documentation and `README-EN.md` for English documentation, excluding build/metadata directories such as `bin/`, `obj/`, `Properties/`, `Core/`, and project-only directories.
- `DSPCore/` 下每个功能块目录都必须有中文 `README.md` 和英文 `README-EN.md`，但 `bin/`、`obj/`、`Properties/`、`Core/` 和纯项目目录除外。

- User-visible behavior or public API changes require README/docs review in the same task.
- 用户可见行为或公开 API 变更，必须在同一任务检查 README/docs。

- Feature block README files target mod authors who understand DSP modding concepts but have not used DSPCore before. They must explain what the block does, what author-side work it removes, what repeated patching or compatibility problems it avoids, what happens after the author calls each public API, which runtime/game behaviors are affected, and the important defaults, conflicts, repeated calls, and unsupported boundaries.
- 功能块 README 的目标读者是了解 DSP 模组语境、但第一次接触 DSPCore 的模组作者。文档必须说明该功能块能做什么、减少作者哪些重复劳动、避免哪些 patch 或兼容问题、作者调用公开 API 后 DSPCore 会怎么处理、会影响哪些运行时/游戏行为，以及重要的默认值、冲突处理、重复调用结果和不支持边界。

- When one feature block contains multiple author-facing capabilities, its README must introduce them in separate sections instead of compressing them into one generic responsibility statement. Simple single-capability blocks may use a shorter structure, but still need author-facing behavior and boundary explanations.
- 当一个功能块包含多个作者侧能力时，README 必须按功能分区块分别介绍，不能压缩成一句泛泛职责说明。简单的单功能块可以使用更短结构，但仍必须说明作者侧行为和边界。

- Feature blocks are internally split into `Api/`, `Runtime/`, `Compat/`, and `Examples/` when they contain those responsibilities. `Api/` owns author-facing entry points, descriptors, registries, extension methods, and public models. `Runtime/` owns BepInEx/Harmony/DSP lifecycle bridges, Unity UI projection, data-phase executors, and other implementation that applies registered intent to the game. `Compat/` owns all legacy or third-party compatibility adapters for that feature block, including files that declare old namespaces or old type names. `Examples/` owns author-facing scenario documentation and demo code.
- 功能块内部按职责拆成 `Api/`、`Runtime/`、`Compat/`、`Examples/`，只有实际存在对应职责时才创建目录。`Api/` 负责作者侧入口、descriptor、registry、扩展方法和公开模型。`Runtime/` 负责 BepInEx/Harmony/DSP 生命周期桥接、Unity UI 投射、数据阶段执行器，以及把注册意图应用到游戏里的实现。`Compat/` 负责该功能块的全部旧 API 或第三方兼容适配，包括声明旧命名空间或旧类型名的文件。`Examples/` 负责作者侧场景说明和 demo 代码。

## Git Rules / Git 规则

- Use simplified Chinese conventional-style commit messages.
- 提交信息使用简体中文 conventional 风格。

- Recommended prefixes: `功能：`, `调整：`, `修复：`, `优化：`, `重构：`, `构建：`, `文档：`, `测试：`, `杂项：`。
- 推荐前缀：`功能：`, `调整：`, `修复：`, `优化：`, `重构：`, `构建：`, `文档：`, `测试：`, `杂项：`。

- Run `dotnet build DSPCore.sln` before commits that touch code, project files, or public API docs.
- 修改代码、项目文件或公开 API 文档后，提交前运行 `dotnet build DSPCore.sln`。

- Do not push unless the user explicitly approves push.
- 未经用户明确批准，不要 push。

## Patch Rules / 补丁规则

- Harmony patches must use `typeof(TargetType)` plus `nameof(TargetType.Method)` whenever the target method is accessible to the compiler.
- Harmony 补丁在目标方法可被编译器访问时，必须使用 `typeof(TargetType)` + `nameof(TargetType.Method)`。

- Use a string method name only when `nameof` cannot compile, such as private game methods or underscore lifecycle hooks.
- 只有 `nameof` 无法编译时才使用字符串方法名，例如游戏私有方法或下划线生命周期钩子。

## Directory Map / 目录说明

```text
DSPCore.sln
├── DSPCore/                 # main BepInEx plugin project / 主插件项目
│   ├── Core/                # Api/Runtime/Compat: DspCore entry, startup assembly, feature/module registries, CommonAPI compatibility
│   ├── Achievements/        # Api/Runtime/Examples: achievement policy aggregation
│   ├── BuildBar/            # Api/Runtime/Compat/Examples: multi-row build bar placement and BuildBarTool compatibility
│   ├── Errors/              # Api/Runtime: error report model and fatal-window runtime bridge
│   ├── Icons/               # Api/Runtime/Examples: icon registration and icon runtime bridge
│   ├── Input/               # Api/Runtime/Examples: key bind declarations and polling bridge
│   ├── Pickers/             # Api/Runtime/Examples: picker requests and popup bridge
│   ├── ProtoRegistration/   # Api/Runtime/Compat/Examples: proto registration facade, data-phase bridge, LDBTool compatibility
│   ├── RecipeTypes/         # Api/Runtime/Examples: custom recipe types and recipe guard bridge
│   ├── Resources/           # Api/Runtime: resource and localization declarations plus localization bridge
│   ├── Saves/               # Api/Runtime/Compat/Examples: save abstraction, sidecar save bridge, DSPModSave compatibility
│   ├── Tabs/                # Api/Runtime/Examples: tab slot declarations and UI projection bridge
│   └── UI/                  # Api/Foundation/Controls/Layout/Theme: common UI descriptors, windows, controls, grid layout, and theme helpers
├── DSPCore.Preloader/       # BepInEx patchers project / BepInEx patchers 项目
└── DSPCore.Packaging/       # Thunderstore packaging project / Thunderstore 打包项目
```

## Current Limitations / 当前限制

- The current implementation includes P0/P1 runtime bridges, not just author-facing API skeletons.
- 当前实现已包含 P0/P1 运行时桥接，不再只是作者可见 API 骨架。

- Implemented runtime bridges: BepInEx/Harmony startup, proto insertion near `VFPreload.InvokeOnLoadWorkEnded`, multi-row build bar binding, DSPCore-owned player build-bar overrides, RebindBuildBar `CustomBarBind.cfg` import, resource/icon loading, item/recipe/replicator/signal/tag-icon tab projection, item/recipe/signal picker popups, live filtering, duplicate `GridIndex` fallback, dynamic picker row/column expansion, custom recipe type guards and pre-selection assembler recipe filtering, key callbacks, `.dspcore` sidecar saves, legacy DSPModSave handler bridging, achievement/abnormality/platform policy patches, error logging/fatal-window buttons, localization entries, and common UI window lifecycle forwarding.
- 已实现运行时桥接：BepInEx/Harmony 启动、`VFPreload.InvokeOnLoadWorkEnded` 附近的 Proto 写入、多行建造栏绑定、DSPCore 自有玩家建造栏覆盖、RebindBuildBar `CustomBarBind.cfg` 导入、资源/图标加载、物品/配方/制造器/信号/标签图标分页投射、物品/配方/信号选择器弹窗、实时过滤、重复 `GridIndex` 兜底、选择器动态行列扩容、自定义配方类型限制与制作器配方选择前过滤、按键回调、`.dspcore` 独立存档、旧 DSPModSave 处理器桥接、成就/异常/平台策略补丁、错误日志/错误窗口按钮、本地化条目，以及通用 UI 窗口生命周期转发。

- UI owns common framework pieces only: descriptors, Unity window lifecycle helpers, reusable controls, declarative grid layout, and theme/card helpers. Concrete feature pages, business navigation, unlock logic, save state, and mod-specific panels stay in the owning feature block or mod.
- UI 只负责通用框架件：描述对象、Unity 窗口生命周期辅助、可复用控件、声明式网格布局和主题/卡片辅助。具体功能页面、业务导航、解锁逻辑、存档状态和模组专属面板留在所属功能块或业务模组中。

- BuildBar owns the build bar placement feature block: item id or `ItemProto` -> tab/row/index bindings, two or more build bar rows, player-defined or dynamically overridden slots, related UI projection and refresh handling, RebindBuildBar compatibility, and BuildBarTool compatibility shims. New author-facing examples should prefer `ItemProto.SetBuildBar(...)`; use `BuildBar.BindQuickBar(...)` only when the caller only has an item id. Do not move proto creation responsibilities into BuildBar.
- BuildBar 负责建造栏位置功能块：物品 ID 或 `ItemProto` -> tab/row/index 绑定、两层或更多层建造栏、玩家自定义或动态覆盖格子、相关 UI 投射和刷新处理、RebindBuildBar 兼容，以及 BuildBarTool 兼容 shim。新的作者侧示例应首选 `ItemProto.SetBuildBar(...)`；只有调用方手上只有物品 ID 时才使用 `BuildBar.BindQuickBar(...)`。不要把 Proto 创建职责移入 BuildBar。

- Runtime host owns only BepInEx startup and cross-feature patch assembly. Concrete runtime bridges belong in their feature block directories.
- Runtime 宿主只负责 BepInEx 启动和跨功能 patch 装配。具体运行时桥接必须放在对应功能块目录。

- Tabs own page declaration and `TabSlot` allocation. `GridIndex` remains the native item/recipe cell field on `ItemProto` and `RecipeProto`; use ProtoRegistration/GridIndexes helpers to build `GridIndex` values from `TabSlot`, row, and index.
- Tabs 负责页面声明和 `TabSlot` 分配。`GridIndex` 仍是 `ItemProto` / `RecipeProto` 的游戏原生格子字段；用 ProtoRegistration/GridIndexes 辅助方法从 `TabSlot`、行号和格子号生成 `GridIndex`。

- P2/P3 features such as custom machine components, planet/star systems, network helpers, and player convenience modules are not implemented.
- 自定义机器组件、星球/恒星系统、网络工具和玩家便利模块等 P2/P3 功能尚未实现。

- RebindBuildBar compatibility imports `BuildBarBinds` from `CustomBarBind.cfg` into DSPCore row-1 player overrides when no DSPCore BuildBar save data exists. DSPCore does not take over RebindBuildBar's rebinding UI, hotkeys, or later config writes.
- RebindBuildBar 兼容会在没有 DSPCore BuildBar 存档数据时，把 `CustomBarBind.cfg` 里的 `BuildBarBinds` 导入 DSPCore 第 1 行玩家覆盖层。DSPCore 不接管 RebindBuildBar 自己的重绑 UI、快捷键或后续配置写回。

- Picker runtime is layered as author intent (`TabSlot` and native `GridIndex`), picker content layout planning, and UI surface adapters. DSPCore does not skip signal/tag picker tab injection based on GenesisBook, OrbitalRing, FE, or other plugin GUIDs. Vanilla picker surfaces remain covered through `UIItemPicker`, `UIRecipePicker`, `UIReplicatorWindow`, `UISignalPicker`, and `UISignalTagPicker`; truly rebuilt third-party picker surfaces still need dedicated adapters.
- Picker 运行时按作者意图层（`TabSlot` 与原生 `GridIndex`）、选择器内容布局层、UI surface adapter 层组织。DSPCore 不按 GenesisBook、OrbitalRing、FE 或其他插件 GUID 跳过 signal/tag picker 分页按钮注入。原版 picker surface 仍通过 `UIItemPicker`、`UIRecipePicker`、`UIReplicatorWindow`、`UISignalPicker` 和 `UISignalTagPicker` 覆盖；真正自建、不复用原版 picker 的第三方界面仍需要专门 adapter。

- Picker row/column counts are not declared by mods. Runtime starts from the current UI surface's real dimensions, scans registered item/recipe/signal `GridIndex` data for the largest required row and column, and expands backing arrays, `ComputeBuffer`, material grid, mouse hit testing, and visible content size together.
- Picker 行列数不由模组声明。运行时会以当前 UI surface 的实际尺寸为基准，扫描已注册物品、配方和信号 `GridIndex` 数据得到需要的最大行列，并同步扩展后备数组、`ComputeBuffer`、材质网格、鼠标命中和可见内容尺寸。

- Legacy shims bridge covered calls to implemented DSPCore runtime blocks, but they are not guaranteed to reproduce every old-library edge behavior.
- 旧 API shim 会把已覆盖调用桥接到已实现的 DSPCore 运行时功能块，但不保证复刻旧库所有边缘行为。
