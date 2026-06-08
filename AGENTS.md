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

- Compatibility code belongs to the owning authoring capability under `DSPCore/Authoring/<Capability>/Compat/`; do not create a top-level `Legacy/` or centralized legacy compatibility directory. Old namespaces may be declared from capability `Compat/` files when required by source compatibility.
- 兼容代码必须放在所属作者能力的 `DSPCore/Authoring/<Capability>/Compat/` 下；禁止创建顶层 `Legacy/` 或集中式旧兼容目录。为了源码兼容需要保留旧命名空间时，也应在对应能力的 `Compat/` 文件中声明。

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

- Legacy API migration notes belong in `README.md`, `README-EN.md`, or the owning authoring capability README until a real cross-capability documentation need appears.
- 旧 API 迁移说明先放在 `README.md`、`README-EN.md` 或所属作者能力 README；只有出现真实跨能力长文档需求时才恢复根 `docs/`。

- Every public capability should have a concrete `.cs` example under `DSPCore/Authoring/<Capability>/Examples/` when a runnable author scenario exists. All example `.cs` files are documentation artifacts and must be excluded from compilation.
- 每一项公开能力在存在可演示作者场景时，应在 `DSPCore/Authoring/<Capability>/Examples/` 下有具体 `.cs` 示例。所有示例 `.cs` 文件都属于文档产物，必须排除编译。

- Capability-specific examples should use paired scenario files when practical: `Examples/<Scenario>.md` explains when to use the API, key parameters, runtime prerequisites, and common mistakes; `Examples/<Scenario>Example.cs` provides the small demo code. The `.md` explanation does not replace the authoring capability README, and the `.cs` demo remains excluded from compilation.
- 能力专属示例在可行时应使用成对场景文件：`Examples/<Scenario>.md` 说明适用时机、关键参数、运行时前提和常见误用；`Examples/<Scenario>Example.cs` 提供小型 demo 代码。`.md` 说明不替代作者能力 README，`.cs` demo 仍必须排除编译。

- Every authoring capability directory under `DSPCore/Authoring/` must have `README.md` for Chinese documentation and `README-EN.md` for English documentation, excluding project-only or metadata directories. System integration directories under `DSPCore/Systems/` need README files only when they expose durable lifecycle, projection, persistence, or compatibility rules that are not already covered by the owning authoring capability.
- `DSPCore/Authoring/` 下每个作者能力目录都必须有中文 `README.md` 和英文 `README-EN.md`，但纯项目或元数据目录除外。`DSPCore/Systems/` 下的系统集成目录只有在暴露长期生命周期、投射、持久化或兼容规则，且这些规则没有被所属作者能力文档覆盖时，才需要 README。

- User-visible behavior or public API changes require README/docs review in the same task.
- 用户可见行为或公开 API 变更，必须在同一任务检查 README/docs。

- Authoring capability README files target mod authors who understand DSP modding concepts but have not used DSPCore before. They must explain what the capability does, what author-side work it removes, what repeated patching or compatibility problems it avoids, what happens after the author calls each public API, which runtime/game behaviors are affected, and the important defaults, conflicts, repeated calls, and unsupported boundaries.
- 作者能力 README 的目标读者是了解 DSP 模组语境、但第一次接触 DSPCore 的模组作者。文档必须说明该能力能做什么、减少作者哪些重复劳动、避免哪些 patch 或兼容问题、作者调用公开 API 后 DSPCore 会怎么处理、会影响哪些运行时/游戏行为，以及重要的默认值、冲突处理、重复调用结果和不支持边界。

- When one authoring directory contains multiple author-facing capabilities, its README must introduce them in separate sections instead of compressing them into one generic responsibility statement. Simple single-capability directories may use a shorter structure, but still need author-facing behavior and boundary explanations.
- 当一个作者目录包含多个作者侧能力时，README 必须按功能分区块分别介绍，不能压缩成一句泛泛职责说明。简单的单能力目录可以使用更短结构，但仍必须说明作者侧行为和边界。

- `DSPCore/Authoring/` owns the author-facing surface: short entries, descriptors, registries, extension methods, public models, compatibility shims, examples, and author documentation. `DSPCore/Systems/` owns integration behavior: BepInEx/Harmony/DSP lifecycle bridges, Unity UI projection, data-phase executors, persistence bridges, resource loading, and other implementation that applies registered intent to the game. Do not put runtime patch classes back under `Authoring/`; do not make `Systems/` expose new author APIs directly.
- `DSPCore/Authoring/` 负责作者侧表面：短入口、descriptor、registry、扩展方法、公开模型、兼容 shim、示例和作者文档。`DSPCore/Systems/` 负责系统集成行为：BepInEx/Harmony/DSP 生命周期桥接、Unity UI 投射、数据阶段执行器、持久化桥、资源加载，以及把注册意图应用到游戏里的实现。不要把运行时 patch 类放回 `Authoring/`；不要让 `Systems/` 直接暴露新的作者 API。

- Authoring capability directories may contain `Api/`, `Compat/`, and `Examples/` when those responsibilities exist. `Api/` owns author-facing entry points, descriptors, registries, extension methods, and public models. `Compat/` owns legacy or third-party compatibility adapters for that capability, including files that declare old namespaces or old type names. `Examples/` owns author-facing scenario documentation and demo code.
- 作者能力目录在实际存在对应职责时，可以包含 `Api/`、`Compat/` 和 `Examples/`。`Api/` 负责作者侧入口、descriptor、registry、扩展方法和公开模型。`Compat/` 负责该能力的旧 API 或第三方兼容适配，包括声明旧命名空间或旧类型名的文件。`Examples/` 负责作者侧场景说明和 demo 代码。

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
│   ├── Authoring/           # author-facing capabilities / 作者侧能力
│   │   ├── Core/            # DspCore entry, feature/module registries, CommonAPI compatibility
│   │   ├── Achievements/    # achievement policy declarations and examples
│   │   ├── BuildBar/        # quick bar binding API and BuildBarTool compatibility
│   │   ├── Icons/           # icon registration descriptors
│   │   ├── KeyBinds/        # key bind declarations
│   │   ├── DataPhases/      # Data, DataUpdates, DataFinalFixes entries
│   │   ├── ProtoAccess/     # phase-aware proto lookup and mutation direction
│   │   ├── ProtoRegistration/ # low-level proto registry facade and LDBTool compatibility
│   │   ├── Items/           # item proto registration entry
│   │   ├── Recipes/         # recipe proto registration entry
│   │   ├── Techs/           # tech proto registration entry
│   │   ├── Tutorials/       # tutorial/guide proto registration entry
│   │   ├── GameEnums/       # vanilla enum extension direction, current recipe type guard API
│   │   ├── Resources/       # resource and localization declarations
│   │   ├── Saves/           # save abstraction and DSPModSave compatibility
│   │   ├── Components/      # entity component descriptors and lifecycle base classes
│   │   ├── Planets/         # planet factory system descriptors
│   │   ├── Blueprints/      # tagged building parameter blocks
│   │   ├── Models/          # model and prefab clone descriptors
│   │   ├── Options/         # BepInEx config option descriptors
│   │   ├── Multiplayer/     # optional multiplayer packet declarations
│   │   ├── Networks/        # factory network query adapters
│   │   ├── Galaxy/          # star and galaxy lifecycle descriptors
│   │   ├── Tabs/            # tab slot declarations
│   │   └── UI/              # common UI descriptors, windows, controls, grid layout, and theme helpers
│   ├── Systems/             # lifecycle, projection, persistence, and patch bridges / 系统集成
│   │   ├── Lifecycle/       # BepInEx startup and cross-system patch assembly
│   │   ├── ProtoPipeline/   # data-phase execution and proto insertion bridge
│   │   ├── TabProjection/   # tab projection into vanilla UI surfaces
│   │   ├── PickerSurfaces/  # picker surface adapters and dynamic layout
│   │   ├── QuickBarProjection/ # build bar projection and player overrides
│   │   ├── ResourceLoading/ # localization and resource application
│   │   ├── IconProjection/  # icon loading and application
│   │   ├── GameEnumProjection/ # current recipe type guards and future enum patching
│   │   ├── KeyBindProjection/ # key polling and callbacks
│   │   ├── SaveBridge/      # sidecar saves and legacy handler bridge
│   │   ├── EntityLifecycle/ # entity component runtime and preloader field access
│   │   ├── PlanetLifecycle/ # planet factory system runtime
│   │   ├── BlueprintParameters/ # copy-paste and blueprint tagged parameter runtime
│   │   ├── ModelProjection/ # model/prefab clone application and cache rebuild
│   │   ├── Options/         # BepInEx config binding
│   │   ├── Multiplayer/     # optional multiplayer detection
│   │   ├── GalaxyLifecycle/ # star and galaxy runtime lifecycle
│   │   ├── PatchPlatform/   # conditional patch declaration application
│   │   ├── AchievementPolicy/ # achievement and platform policy patches
│   │   ├── ErrorWindow/     # error logging and fatal-window bridge
│   │   └── UiRuntime/       # UI window lifecycle patches
│   ├── Properties/
│   └── DSPCore.csproj
├── DSPCore.Preloader/       # BepInEx patchers project / BepInEx patchers 项目
└── DSPCore.Packaging/       # Thunderstore packaging project / Thunderstore 打包项目
```

## Current Limitations / 当前限制

- The current implementation includes P0/P1 runtime bridges, not just author-facing API skeletons.
- 当前实现已包含 P0/P1 运行时桥接，不再只是作者可见 API 骨架。

- Implemented runtime bridges: BepInEx/Harmony startup, real preloader field injection for `EntityData` / `PrefabDesc` and `ERecipeType.Custom`, Factorio-like `Data` / `DataUpdates` / `DataFinalFixes` proto phase callbacks and proto insertion near `VFPreload.InvokeOnLoadWorkEnded`, multi-row build bar binding, DSPCore-owned player build-bar overrides, RebindBuildBar `CustomBarBind.cfg` import, resource/icon loading, item/recipe/replicator/signal/tag-icon tab projection, item/recipe/signal picker popups, live filtering, duplicate `GridIndex` fallback, dynamic picker row/column expansion, custom recipe type guards and pre-selection assembler recipe filtering, key callbacks, `.dspcore` sidecar saves, legacy DSPModSave handler bridging, entity component lifecycle, planet/star/galaxy lifecycle, blueprint parameter blocks, model/prefab cloning, option binding, optional multiplayer detection and packet declarations, network query adapters, conditional patch platform, achievement/abnormality/platform policy patches, error logging/fatal-window buttons, localization entries, and common UI window lifecycle forwarding.
- 已实现运行时桥接：BepInEx/Harmony 启动、面向 `EntityData` / `PrefabDesc` 和 `ERecipeType.Custom` 的真实 Preloader 字段注入、类似 Factorio `Data` / `DataUpdates` / `DataFinalFixes` 的 Proto 阶段回调和 `VFPreload.InvokeOnLoadWorkEnded` 附近的 Proto 写入、多行建造栏绑定、DSPCore 自有玩家建造栏覆盖、RebindBuildBar `CustomBarBind.cfg` 导入、资源/图标加载、物品/配方/制造器/信号/标签图标分页投射、物品/配方/信号选择器弹窗、实时过滤、重复 `GridIndex` 兜底、选择器动态行列扩容、自定义配方类型限制与制作器配方选择前过滤、按键回调、`.dspcore` 独立存档、旧 DSPModSave 处理器桥接、实体组件生命周期、星球/恒星/银河生命周期、蓝图参数块、模型/预制体克隆、配置项绑定、可选联机检测和 packet 声明、网络查询适配器、条件补丁平台、成就/异常/平台策略补丁、错误日志/错误窗口按钮、本地化条目，以及通用 UI 窗口生命周期转发。

- UI owns common framework pieces only: descriptors, Unity window lifecycle helpers, reusable controls, declarative grid layout, and theme/card helpers. Concrete feature pages, business navigation, unlock logic, save state, and mod-specific panels stay in the owning authoring capability, owning system, or mod.
- UI 只负责通用框架件：描述对象、Unity 窗口生命周期辅助、可复用控件、声明式网格布局和主题/卡片辅助。具体功能页面、业务导航、解锁逻辑、存档状态和模组专属面板留在所属作者能力、所属系统或业务模组中。

- BuildBar owns the author-facing build bar placement capability: item id or `ItemProto` -> tab/row/index bindings, two or more build bar rows, player-defined or dynamically overridden slots, related UI projection and refresh handling, RebindBuildBar compatibility, and BuildBarTool compatibility shims. New author-facing examples should prefer `ItemProto.SetBuildBar(...)`; use `BuildBar.BindQuickBar(...)` only when the caller only has an item id. Do not move proto creation responsibilities into BuildBar.
- BuildBar 负责作者侧建造栏位置能力：物品 ID 或 `ItemProto` -> tab/row/index 绑定、两层或更多层建造栏、玩家自定义或动态覆盖格子、相关 UI 投射和刷新处理、RebindBuildBar 兼容，以及 BuildBarTool 兼容 shim。新的作者侧示例应首选 `ItemProto.SetBuildBar(...)`；只有调用方手上只有物品 ID 时才使用 `BuildBar.BindQuickBar(...)`。不要把 Proto 创建职责移入 BuildBar。

- Lifecycle owns only BepInEx startup and cross-system patch assembly. Concrete runtime bridges belong in their corresponding `DSPCore/Systems/` directories.
- Lifecycle 只负责 BepInEx 启动和跨系统 patch 装配。具体运行时桥接必须放在对应的 `DSPCore/Systems/` 目录。

- ProtoRegistration owns the author-facing data lifecycle. New examples should prefer `ProtoRegistration.Data(...)`, `ProtoRegistration.DataUpdates(...)`, and `ProtoRegistration.DataFinalFixes(...)` callbacks with `ProtoPhaseContext`; direct `RegisterItem(..., phase)` remains available as a lower-level and compatibility entry.
- ProtoRegistration 负责作者侧数据生命周期。新示例应优先使用 `ProtoRegistration.Data(...)`、`ProtoRegistration.DataUpdates(...)` 和 `ProtoRegistration.DataFinalFixes(...)` 回调，并通过 `ProtoPhaseContext` 注册物品、配方、科技和指引；直接 `RegisterItem(..., phase)` 继续作为低层和兼容入口保留。

- Tabs own page declaration and `TabSlot` allocation. `GridIndex` remains the native item/recipe cell field on `ItemProto` and `RecipeProto`; use ProtoRegistration/GridIndexes helpers to build `GridIndex` values from `TabSlot`, row, and index.
- Tabs 负责页面声明和 `TabSlot` 分配。`GridIndex` 仍是 `ItemProto` / `RecipeProto` 的游戏原生格子字段；用 ProtoRegistration/GridIndexes 辅助方法从 `TabSlot`、行号和格子号生成 `GridIndex`。

- P2/P3 framework blocks now include custom entity components, planet/star/galaxy systems, blueprint parameter blocks, model/prefab cloning, option descriptors, optional multiplayer declarations, network query adapters, and a conditional patch platform. Player convenience modules such as RecipeFinder, FreeMechaCustom, and AssemblerUI-style panels are still outside core.
- P2/P3 框架块当前已包含自定义实体组件、星球/恒星/银河系统、蓝图参数块、模型/预制体克隆、配置项描述、可选联机声明、网络查询适配器和条件补丁平台。RecipeFinder、FreeMechaCustom、AssemblerUI 风格面板等玩家便利模块仍未进入核心。

- Preloader-injected fields are auxiliary interoperability markers. Runtime code must not compile directly against those injected fields because the referenced dumped `Assembly-CSharp.dll` may not contain them; use `InjectedFieldAccess` or reflection helpers instead.
- Preloader 注入字段是辅助互操作 marker。运行时代码不能直接编译期访问这些注入字段，因为当前引用的 dumped `Assembly-CSharp.dll` 可能没有对应字段；必须使用 `InjectedFieldAccess` 或反射辅助层。

- Blueprint parameter extensions must use DSPCore tagged blocks appended to `BuildingParameters.parameters`; do not reserve fixed vanilla parameter slots for a mod-specific feature.
- 蓝图参数扩展必须使用追加到 `BuildingParameters.parameters` 的 DSPCore tagged block；不要为单个模组功能预留固定原版参数槽位。

- Optional multiplayer support must stay soft in the main DSPCore project. Do not add a hard Nebula reference unless a separate adapter project or explicit dependency decision is added. The main project may declare packet, host relay, planet data request, and client missing-save boundaries for an adapter to consume.
- 主 DSPCore 项目的可选联机支持必须保持软依赖。除非新增独立适配项目或明确依赖决策，不要添加 Nebula 硬引用。主项目可以声明 packet、host relay、planet data request 和 client missing-save 边界，供适配器消费。

- RebindBuildBar compatibility imports `BuildBarBinds` from `CustomBarBind.cfg` into DSPCore row-1 player overrides when no DSPCore BuildBar save data exists. DSPCore does not take over RebindBuildBar's rebinding UI, hotkeys, or later config writes.
- RebindBuildBar 兼容会在没有 DSPCore BuildBar 存档数据时，把 `CustomBarBind.cfg` 里的 `BuildBarBinds` 导入 DSPCore 第 1 行玩家覆盖层。DSPCore 不接管 RebindBuildBar 自己的重绑 UI、快捷键或后续配置写回。

- Picker runtime owns vanilla UI surface adapters. DSPCore does not skip signal/tag picker tab injection based on GenesisBook, OrbitalRing, FE, or other plugin GUIDs. Vanilla picker surfaces remain covered through `UIItemPicker`, `UIRecipePicker`, `UIReplicatorWindow`, `UISignalPicker`, and `UISignalTagPicker`; truly rebuilt third-party picker surfaces still need dedicated adapters.
- Picker runtime 负责原版 UI surface adapter。DSPCore 不按 GenesisBook、OrbitalRing、FE 或其他插件 GUID 跳过 signal/tag picker 分页按钮注入。原版 picker surface 仍通过 `UIItemPicker`、`UIRecipePicker`、`UIReplicatorWindow`、`UISignalPicker` 和 `UISignalTagPicker` 覆盖；真正自建、不复用原版 picker 的第三方界面仍需要专门 adapter。

- Picker row/column counts are not declared by mods. Runtime starts from the current UI surface's real dimensions, scans registered item/recipe/signal `GridIndex` data for the largest required row and column, and expands backing arrays, `ComputeBuffer`, material grid, mouse hit testing, and visible content size together.
- Picker 行列数不由模组声明。运行时会以当前 UI surface 的实际尺寸为基准，扫描已注册物品、配方和信号 `GridIndex` 数据得到需要的最大行列，并同步扩展后备数组、`ComputeBuffer`、材质网格、鼠标命中和可见内容尺寸。

- Legacy shims bridge covered calls to implemented DSPCore runtime blocks, but they are not guaranteed to reproduce every old-library edge behavior.
- 旧 API shim 会把已覆盖调用桥接到已实现的 DSPCore 运行时功能块，但不保证复刻旧库所有边缘行为。
