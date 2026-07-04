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

- Code projects: `DSPCore`, `DSPCore.Preloader`, `DSPCore.NebulaAdapter`, and `DSPCore.Packaging`.
- 代码项目：`DSPCore`、`DSPCore.Preloader`、`DSPCore.NebulaAdapter` 和 `DSPCore.Packaging`。

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

Targeted logic tests for changes that touch StableIds, BuildBar conflict behavior, Options import/export, save block formats, automatic saves, global save overview, localization lookup, resource root resolution, icon loading/binding, GameEnums item/recipe type declarations, recipe type assembler filtering, picker layout planning, Blueprint tagged parameter blocks, entity components, planet lifecycle systems, star/galaxy lifecycle systems, or model projection:

```bash
tests/run-logic-tests.sh
```

Runtime wiring guard for changes that touch startup assembly, Harmony patch targets, vanilla option/key-bind entries, GlobalSaves read-only viewer entry, BuildBar editor entry, BuildBar extended rows, picker dynamic layout, error window buttons, entity/planet/galaxy lifecycle, blueprint parameters, model projection, or save bridge:

```bash
tests/verify-runtime-wiring.sh
```

Smoke-mod guard for changes that touch manual in-game verification coverage, Options / KeyBinds / GlobalSaves / BuildBar / Errors / Multiplayer player-facing entry points, or Thunderstore packaging exclusions:

```bash
tests/verify-smoke-mod.sh
dotnet build tests/DSPCore.SmokeMod/DSPCore.SmokeMod.csproj
```

`tests/DSPCore.SmokeMod` is a manual in-game verification plugin and must not be added to Thunderstore packaging. It is allowed in `DSPCore.sln` so normal solution builds catch public API drift, but release packaging scripts must keep excluding it. Use `tests/install-smoke-mod.sh` to copy built DLLs into a test BepInEx profile and `tests/verify-smoke-evidence.sh` to check single-end BepInEx logs after a real game run, including UI pages, opt-in icon binding, opt-in recipe type projection, error reporting, offline multiplayer behavior, and opt-in automatic Nebula host/client behavior. The content projection smoke option is disabled by default and should only be enabled in a dedicated smoke profile. Use `tests/prepare-nebula-smoke-profiles.sh <source> <host> <client>` to copy isolated Nebula smoke profiles without overwriting existing targets. Use `tests/update-nebula-smoke-profile.sh <profile> <Host|Client|Off> [address]` to refresh an existing isolated profile with current DLLs and automatic Nebula mode. Use `tests/start-dsp-profile.sh <profile> [game-dir] [-- extra args]` to temporarily point the game Doorstop config at one profile, start DSP, restore the original config after launch, and pass Nebula dedicated arguments such as `-nebula-server -batchmode -newgame ...` when needed. Use `tests/verify-nebula-profile-ready.sh <profile>` before Nebula room tests to confirm the profile has the real Nebula mod enabled, not only `.old` files, and that its dependency DLLs are not disabled as `.dll.old`. Use `tests/verify-nebula-smoke-evidence.sh <host-log> <client-log>` for real two-client room evidence.

Packaging verification when release artifacts, project list, manifest, or Thunderstore contents change. Run `dotnet build DSPCore.sln` first because the packaging helper packages existing DLL outputs instead of rebuilding referenced projects:

```bash
dotnet run --project DSPCore.Packaging/DSPCore.Packaging.csproj
tests/verify-thunderstore-packages.sh
tests/verify-multiplayer-adapter-boundaries.sh
```

Expected result:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

The packaging command should produce `artifacts/thunderstore/MengLei-DSPCore-<version>.zip` with `plugins/DSPCore/DSPCore.dll` and `patchers/DSPCore.Preloader.dll`, plus `artifacts/thunderstore/MengLei-DSPCore_NebulaAdapter-<version>.zip` with `plugins/DSPCore/DSPCore.NebulaAdapter.dll` and a manifest dependency on `nebula-NebulaMultiplayerModApi`. The package verification script asserts that the base package does not include `DSPCore.NebulaAdapter.dll` and that the adapter package keeps its DSPCore/Nebula dependencies. The multiplayer adapter boundary script asserts that the main DSPCore project stays free of NebulaAPI references while the adapter registers its transport and packet processor.

涉及发布产物、项目列表、manifest、联机适配器边界或 Thunderstore 包内容变化时，还要运行打包验证命令、`tests/verify-thunderstore-packages.sh` 和 `tests/verify-multiplayer-adapter-boundaries.sh`。先运行 `dotnet build DSPCore.sln`，因为打包 helper 只打包已有 DLL 产物，不重新构建引用项目。打包命令应生成基础包 `artifacts/thunderstore/MengLei-DSPCore-<version>.zip`，其中包含 `plugins/DSPCore/DSPCore.dll` 和 `patchers/DSPCore.Preloader.dll`，且不包含 `DSPCore.NebulaAdapter.dll`；同时生成可选适配器包 `artifacts/thunderstore/MengLei-DSPCore_NebulaAdapter-<version>.zip`，其中包含 `plugins/DSPCore/DSPCore.NebulaAdapter.dll`，并在 manifest 里依赖 `DSPCore` 和 `nebula-NebulaMultiplayerModApi`。联机边界测试会检查主 DSPCore 项目不硬引用 NebulaAPI，而适配器注册 transport 和 packet processor。

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
│   │   ├── GameEnums/       # vanilla enum extension direction, recipe type guards, custom item type declarations
│   │   ├── Resources/       # resource and localization declarations
│   │   ├── Saves/           # save abstraction and DSPModSave compatibility
│   │   ├── Diagnostics/     # author declaration diagnostics
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
│   │   ├── Diagnostics/     # startup author declaration checks
│   │   ├── ErrorWindow/     # error logging and fatal-window bridge
│   │   └── UiRuntime/       # UI window lifecycle patches
│   ├── Properties/
│   └── DSPCore.csproj
├── DSPCore.Preloader/       # BepInEx patchers project / BepInEx patchers 项目
├── DSPCore.NebulaAdapter/   # optional Nebula transport adapter / 可选 Nebula 传输适配器
└── DSPCore.Packaging/       # Thunderstore packaging project / Thunderstore 打包项目
```

## Current Limitations / 当前限制

- The current implementation includes P0/P1 runtime bridges, not just author-facing API skeletons.
- 当前实现已包含 P0/P1 运行时桥接，不再只是作者可见 API 骨架。

- Implemented runtime bridges: BepInEx/Harmony startup, real preloader field injection for `EntityData` / `PrefabDesc` plus reserved enum members `ERecipeType.Custom` / `EItemType.Custom`, Factorio-like `Data` / `DataUpdates` / `DataFinalFixes` proto phase callbacks and proto insertion near `VFPreload.InvokeOnLoadWorkEnded`, multi-row build bar binding, DSPCore-owned player build-bar overrides, DSPCore Build Bar editor, RebindBuildBar `CustomBarBind.cfg` import, resource root resolution and file reads, resource/icon loading including Resources, PNG, embedded resources, and AssetBundles, item/recipe/replicator/signal/tag-icon tab projection, item/recipe/signal picker popups, live filtering, duplicate `GridIndex` fallback, dynamic picker row/column expansion, custom recipe type guards and pre-selection assembler recipe filtering, custom item type declarations mapped to the reserved item-type slot, key binding short registration, key callbacks, vanilla option-window entry, vanilla key-binding page entry, text/capture rebinding, display placement, and conflict hints through the DSPCore settings window, `.dspcore` sidecar saves, cross-save global save storage and read-only metadata viewing, save-chain lifecycle callbacks, legacy DSPModSave handler bridging, entity component lifecycle, planet/star/galaxy lifecycle, blueprint parameter blocks, model/prefab cloning, option binding with page/section context, bool/string/number/enum/range controls, `OptionUi` presentation/order/reset metadata, and option snapshot import/export, author declaration diagnostics, optional multiplayer detection, packet declarations and send abstractions, standalone Nebula transport adapter, network query adapters, conditional patch platform, achievement abnormality-check and competitive-upload policy patches, error logging, error reports and copyable diagnostic text with game/entity context, fatal-window buttons, localization entries plus player locale-file overrides, and common UI window lifecycle forwarding.
- 已实现运行时桥接：BepInEx/Harmony 启动、面向 `EntityData` / `PrefabDesc` 的真实 Preloader 字段注入，以及 `ERecipeType.Custom` / `EItemType.Custom` 预留枚举成员注入、类似 Factorio `Data` / `DataUpdates` / `DataFinalFixes` 的 Proto 阶段回调和 `VFPreload.InvokeOnLoadWorkEnded` 附近的 Proto 写入、多行建造栏绑定、DSPCore 自有玩家建造栏覆盖、DSPCore 建造栏绑定编辑器、RebindBuildBar `CustomBarBind.cfg` 导入、资源根解析与文件读取、包含 Resources、PNG、嵌入资源和 AssetBundle 的资源/图标加载、物品/配方/制造器/信号/标签图标分页投射、物品/配方/信号选择器弹窗、实时过滤、重复 `GridIndex` 兜底、选择器动态行列扩容、自定义配方类型限制与制作器配方选择前过滤、映射到预留物品类型槽的自定义物品类型声明、按键短注册、按键回调、原版设置窗口入口和 DSPCore 设置窗口文本/捕获重绑定、显示位置策略与冲突提示、`.dspcore` 独立存档、跨存档全局保存和只读 metadata 查看、存档链路生命周期回调、旧 DSPModSave 处理器桥接、实体组件生命周期、星球/恒星/银河生命周期、蓝图参数块、模型/预制体克隆、带页面/分区上下文、bool/string/number/enum/range 控件、`OptionUi` 展示/排序/重置元数据和配置快照导入导出的配置项绑定、作者声明诊断、可选联机检测和 packet 声明/发送抽象、独立 Nebula transport adapter、网络查询适配器、条件补丁平台、成就异常检查和竞争性上传策略补丁、错误日志、带游戏/实体上下文的错误报告和可复制诊断文本、错误窗口按钮、本地化条目与玩家 locale 文件覆盖，以及通用 UI 窗口生命周期转发。

- UI owns common framework pieces only: descriptors, Unity window lifecycle helpers, reusable controls, declarative grid layout, theme/card helpers, and standard form/list/detail/status scaffolds. Concrete feature pages, business navigation, unlock logic, save state, and mod-specific panels stay in the owning authoring capability, owning system, or mod.
- UI 只负责通用框架件：描述对象、Unity 窗口生命周期辅助、可复用控件、声明式网格布局、主题/卡片辅助，以及标准表单/列表/详情/状态脚手架。具体功能页面、业务导航、解锁逻辑、存档状态和模组专属面板留在所属作者能力、所属系统或业务模组中。

- BuildBar owns the author-facing build bar placement capability and the player-facing override editor: item id or `ItemProto` -> category/row/index bindings, two or more build bar rows, player-defined or dynamically overridden slots, the DSPCore Build Bar editor inside the unified settings window, `BuildBar.OpenEditor()` for buttons or key binds that should reuse the same editor, related UI projection and refresh handling, RebindBuildBar compatibility, and BuildBarTool compatibility shims. Author default bindings must not silently replace another existing author default; use `BuildBar.BindQuickBarWithResult(...)` or `ItemProto.SetBuildBarWithResult(...)` when callers need `Applied` / `Occupied` / `Replaced` details, and require `BuildBarConflictPolicy.ReplaceExisting` for intentional replacement. New author-facing examples should prefer `ItemProto.SetBuildBar(...)`; use `BuildBar.BindQuickBar(...)` only when the caller only has an item id. Do not move proto creation responsibilities into BuildBar. The old `tab` named parameter remains only as an obsolete source-compatibility name.
- BuildBar 负责作者侧建造栏位置能力和玩家侧覆盖编辑器：物品 ID 或 `ItemProto` -> category/row/index 绑定、两层或更多层建造栏、玩家自定义或动态覆盖格子、DSPCore 统一设置窗口内的建造栏绑定编辑器、供按钮或快捷键复用同一编辑器的 `BuildBar.OpenEditor()`、相关 UI 投射和刷新处理、RebindBuildBar 兼容，以及 BuildBarTool 兼容 shim。作者默认绑定不得静默覆盖另一个已有作者默认绑定；调用方需要 `Applied` / `Occupied` / `Replaced` 详情时使用 `BuildBar.BindQuickBarWithResult(...)` 或 `ItemProto.SetBuildBarWithResult(...)`，确实要覆盖时必须显式使用 `BuildBarConflictPolicy.ReplaceExisting`。新的作者侧示例应首选 `ItemProto.SetBuildBar(...)`；只有调用方手上只有物品 ID 时才使用 `BuildBar.BindQuickBar(...)`。不要把 Proto 创建职责移入 BuildBar。旧 `tab` 命名参数只作为 obsolete 源码兼容名保留。

- Lifecycle owns only BepInEx startup and cross-system patch assembly. Concrete runtime bridges belong in their corresponding `DSPCore/Systems/` directories.
- Lifecycle 只负责 BepInEx 启动和跨系统 patch 装配。具体运行时桥接必须放在对应的 `DSPCore/Systems/` 目录。

- Core feature and module declarations should prefer `Features.Register(id, displayName, initialize, priority, dependencies)` and `Modules.Register(id, displayName, initialize, dependencies)` in new examples; descriptor objects remain the advanced construction path.
- Core 功能块和模块声明的新示例应优先使用 `Features.Register(id, displayName, initialize, priority, dependencies)` 与 `Modules.Register(id, displayName, initialize, dependencies)`；descriptor 对象保留为高级构造路径。

- ProtoRegistration owns the author-facing data lifecycle. New examples should prefer `ProtoRegistration.Data(...)`, `ProtoRegistration.DataUpdates(...)`, and `ProtoRegistration.DataFinalFixes(...)` callbacks with `ProtoPhaseContext`; inside a phase callback, prefer typed `data.RegisterItem(ItemProto)`, `RegisterRecipe(RecipeProto)`, `RegisterTech(TechProto)`, and `RegisterTutorial(TutorialProto)` overloads because they return the original object for chaining; object-centric extension entries such as `ItemProto.RegisterItem(...)` remain useful outside phase callbacks when the caller already has the domain object; direct `RegisterItem(..., phase)` remains available as a lower-level and compatibility entry.
- ProtoRegistration 负责作者侧数据生命周期。新示例应优先使用 `ProtoRegistration.Data(...)`、`ProtoRegistration.DataUpdates(...)` 和 `ProtoRegistration.DataFinalFixes(...)` 回调；阶段回调内部优先使用类型化 `data.RegisterItem(ItemProto)`、`RegisterRecipe(RecipeProto)`、`RegisterTech(TechProto)` 和 `RegisterTutorial(TutorialProto)` 重载，因为它们会返回原对象便于链式继续配置；回调外调用方已有领域对象时，`ItemProto.RegisterItem(...)` 等对象扩展入口仍然适用；直接 `RegisterItem(..., phase)` 继续作为低层和兼容入口保留。

- ProtoAccess owns phase-aware lookup and mutation of visible protos through `ProtoPhaseContext.FindItem(...)`, `FindRecipe(...)`, `FindTech(...)`, `FindTutorial(...)`, and `data.Access`. Cross-mod mutation belongs in `DataUpdates` or `DataFinalFixes`; `Data` remains for initial declarations.
- ProtoAccess 负责通过 `ProtoPhaseContext.FindItem(...)`、`FindRecipe(...)`、`FindTech(...)`、`FindTutorial(...)` 和 `data.Access` 进行阶段感知的可见 Proto 查询与修改。跨模组修改应放在 `DataUpdates` 或 `DataFinalFixes`；`Data` 仍用于初始声明。

- Tabs own page declaration and `TabSlot` allocation. New examples should prefer `Tabs.AddTab(id, ownerModGuid, title, iconId, order)`; `CoreTabDescriptor` remains the advanced construction path. `GridIndex` remains the native item/recipe cell field on `ItemProto` and `RecipeProto`; use ProtoRegistration/GridIndexes helpers to build `GridIndex` values from `TabSlot`, row, and index.
- Tabs 负责页面声明和 `TabSlot` 分配。新示例应优先使用 `Tabs.AddTab(id, ownerModGuid, title, iconId, order)`；`CoreTabDescriptor` 保留为高级构造路径。`GridIndex` 仍是 `ItemProto` / `RecipeProto` 的游戏原生格子字段；用 ProtoRegistration/GridIndexes 辅助方法从 `TabSlot`、行号和格子号生成 `GridIndex`。

- P2/P3 framework blocks now include custom entity components, planet/star/galaxy systems, blueprint parameter blocks, model/prefab cloning, option descriptors, optional multiplayer declarations and adapter dispatch entries, network query adapters, and a conditional patch platform. New examples should prefer typed short registration such as `Components.Register<TComponent>(...)`, `PlanetSystems.Register<TSystem>(...)`, and `GalaxySystems.RegisterGalaxy<TSystem>(...)` when the managed type has a parameterless constructor; save examples should prefer `Saves.Auto<TState>(...)` for parameterless automatic-schema state objects and `Saves.Auto(modGuid, state)` only when the caller must prepare an instance first; model cloning should prefer `ModelProto.CloneAsModel(...)` when the caller already has the source model object; option examples with several rows on one settings page should prefer `Options.Page(...).Section(...)`, and buttons/key binds that open framework option surfaces should use `Options.OpenWindow()` or `Options.OpenGlobalSavesWindow()`; resource pack icon examples with a known target type should prefer `pack.ItemIcon(...)`, `RecipeIcon(...)`, `TechIcon(...)`, `TutorialIcon(...)`, or `SignalIcon(...)`; ordinary network adapters should prefer `Networks.Register(networkId, ownerModGuid, tryGetCommonNetwork, ...)`; conditional patches should prefer `Patches.Register(...)` or `Patches.RegisterForPlugin(...)`, and late patch registration is expected to apply immediately once DSPCore already owns its Harmony instance. Descriptors remain the advanced construction path. Player convenience modules such as RecipeFinder, FreeMechaCustom, and AssemblerUI-style panels are still outside core.
- P2/P3 框架块当前已包含自定义实体组件、星球/恒星/银河系统、蓝图参数块、模型/预制体克隆、配置项描述、可选联机声明和 adapter dispatch 入口、网络查询适配器和条件补丁平台。当托管类型有无参构造函数时，新示例应优先使用 `Components.Register<TComponent>(...)`、`PlanetSystems.Register<TSystem>(...)`、`GalaxySystems.RegisterGalaxy<TSystem>(...)` 等类型化短入口；自动 schema 存档示例应优先使用 `Saves.Auto<TState>(...)` 创建无参状态对象，只有调用方需要先准备实例时才使用 `Saves.Auto(modGuid, state)`；模型克隆在调用方已有来源模型对象时应优先使用 `ModelProto.CloneAsModel(...)`；同一设置页有多行配置时，配置项示例应优先使用 `Options.Page(...).Section(...)`，按钮或快捷键需要打开框架配置界面时使用 `Options.OpenWindow()` 或 `Options.OpenGlobalSavesWindow()`；资源包图标示例在目标类型明确时应优先使用 `pack.ItemIcon(...)`、`RecipeIcon(...)`、`TechIcon(...)`、`TutorialIcon(...)` 或 `SignalIcon(...)`；普通网络适配器应优先使用 `Networks.Register(networkId, ownerModGuid, tryGetCommonNetwork, ...)`；条件补丁应优先使用 `Patches.Register(...)` 或 `Patches.RegisterForPlugin(...)`，并且 DSPCore 已持有 Harmony 实例后，晚注册补丁应立即尝试应用。descriptor 保留为高级构造路径。RecipeFinder、FreeMechaCustom、AssemblerUI 风格面板等玩家便利模块仍未进入核心。

- Achievement policy must keep local and platform achievements available. DSPCore blocks vanilla abnormality checks, and `Achievements.BlockCompetitiveUpload(...)` only blocks Milky Way / Steam leaderboard uploads. The old `disableAchievements` parameter remains as a compatibility name for competitive-upload blocking; new examples should prefer `BlockCompetitiveUpload` and `ShouldBlockCompetitiveUpload`.
- 成就策略必须保持本地成就和平台成就可用。DSPCore 屏蔽原版异常检查；`Achievements.BlockCompetitiveUpload(...)` 只阻断 Milky Way / Steam 排行榜上传。旧 `disableAchievements` 参数仅作为竞争性上传阻断的兼容名称保留；新示例应优先使用 `BlockCompetitiveUpload` 和 `ShouldBlockCompetitiveUpload`。

- Diagnostics owns author declaration quality checks. Startup runs built-in checks for registered proto IDs, item/recipe GridIndex reuse, custom tab references, tab icons, option pages, and basic localization language pairs. New mod-specific checks should use `Diagnostics.Warn(...)`, `Error(...)`, or `Info(...)` so reports appear in logs and `Errors.BuildDiagnosticText(...)`; do not use Diagnostics to disable runtime behavior.
- Diagnostics 负责作者声明质量检查。启动期会检查已注册 Proto ID、物品/配方 GridIndex 复用、自定义分页引用、分页图标、配置页和本地化基础语言配对。新的模组专属检查应使用 `Diagnostics.Warn(...)`、`Error(...)` 或 `Info(...)`，让报告进入日志和 `Errors.BuildDiagnosticText(...)`；不要用 Diagnostics 禁用运行时行为。

- Preloader-injected fields are auxiliary interoperability markers. Runtime code must not compile directly against those injected fields because the referenced dumped `Assembly-CSharp.dll` may not contain them; use `InjectedFieldAccess` or reflection helpers instead.
- Preloader 注入字段是辅助互操作 marker。运行时代码不能直接编译期访问这些注入字段，因为当前引用的 dumped `Assembly-CSharp.dll` 可能没有对应字段；必须使用 `InjectedFieldAccess` 或反射辅助层。

- Preloader-injected enum members are reserved compatibility markers. Runtime code should use numeric constants such as `GameEnums.CustomRecipeTypeValue` and `GameEnums.CustomItemTypeValue` instead of compiling directly against injected enum names.
- Preloader 注入的枚举成员是预留兼容 marker。运行时代码应使用 `GameEnums.CustomRecipeTypeValue`、`GameEnums.CustomItemTypeValue` 等数值常量，不要直接编译期访问注入枚举名。

- GameEnums owns stable author-layer recipe/item type declarations that map onto DSPCore reserved enum values. Do not describe runtime author calls as arbitrary CLR enum literal extension; new enum literals can only be injected by the Preloader before the game assembly loads.
- GameEnums 负责映射到 DSPCore 预留枚举值的作者层稳定配方/物品类型声明。不要把运行时作者调用描述成任意 CLR enum literal 扩展；新的 enum literal 只能由 Preloader 在游戏程序集加载前注入。

- Blueprint parameter extensions must use DSPCore tagged blocks appended to `BuildingParameters.parameters`; do not reserve fixed vanilla parameter slots for a mod-specific feature. New examples should prefer the integer-payload `Blueprints.Register(blockId, ownerModGuid, copy, paste, ...)` overload for ordinary settings and use `BuildingParameterDescriptor` only when full block control is needed.
- 蓝图参数扩展必须使用追加到 `BuildingParameters.parameters` 的 DSPCore tagged block；不要为单个模组功能预留固定原版参数槽位。普通设置的新示例应优先使用整数负载版 `Blueprints.Register(blockId, ownerModGuid, copy, paste, ...)` 重载；只有需要完整 block 控制时才使用 `BuildingParameterDescriptor`。

- Optional multiplayer support must stay soft in the main DSPCore project. Do not add a hard Nebula reference to `DSPCore`; Nebula-specific transport belongs in `DSPCore.NebulaAdapter`. The main project may declare packet, host relay, planet data request, and client missing-save boundaries, expose send/request methods through `IMultiplayerTransport`, and provide adapter snapshot/query/dispatch entries for an adapter to consume. New examples should prefer parameter short entries such as `Multiplayer.RegisterPacket(...)`; descriptor overloads remain the advanced construction path for configuration-driven or batch declarations. Adapter examples should call `Multiplayer.DispatchPacket(...)`, `DispatchHostRelay(...)`, `TryExportPlanetData(...)`, or `ImportPlanetData(...)` instead of invoking descriptor callbacks directly.
- 主 DSPCore 项目的可选联机支持必须保持软依赖。不要给 `DSPCore` 增加 Nebula 硬引用；Nebula 专用传输放在 `DSPCore.NebulaAdapter`。主项目可以声明 packet、host relay、planet data request 和 client missing-save 边界，通过 `IMultiplayerTransport` 暴露发送/请求方法，并提供 adapter snapshot/query/dispatch 入口供适配器消费。新示例应优先使用 `Multiplayer.RegisterPacket(...)` 等参数短入口；descriptor 重载保留为配置驱动或批量声明的高级构造路径。adapter 示例应调用 `Multiplayer.DispatchPacket(...)`、`DispatchHostRelay(...)`、`TryExportPlanetData(...)` 或 `ImportPlanetData(...)`，不要直接调用 descriptor callback。

- RebindBuildBar compatibility imports `BuildBarBinds` from `CustomBarBind.cfg` into DSPCore row-1 player overrides when no DSPCore BuildBar save data exists. DSPCore does not take over RebindBuildBar's rebinding UI, hotkeys, or later config writes.
- RebindBuildBar 兼容会在没有 DSPCore BuildBar 存档数据时，把 `CustomBarBind.cfg` 里的 `BuildBarBinds` 导入 DSPCore 第 1 行玩家覆盖层。DSPCore 不接管 RebindBuildBar 自己的重绑 UI、快捷键或后续配置写回。

- Picker runtime owns vanilla UI surface adapters. DSPCore does not skip signal/tag picker tab injection based on GenesisBook, OrbitalRing, FE, or other plugin GUIDs. Vanilla picker surfaces remain covered through `UIItemPicker`, `UIRecipePicker`, `UIReplicatorWindow`, `UISignalPicker`, and `UISignalTagPicker`; truly rebuilt third-party picker surfaces still need dedicated adapters.
- Picker runtime 负责原版 UI surface adapter。DSPCore 不按 GenesisBook、OrbitalRing、FE 或其他插件 GUID 跳过 signal/tag picker 分页按钮注入。原版 picker surface 仍通过 `UIItemPicker`、`UIRecipePicker`、`UIReplicatorWindow`、`UISignalPicker` 和 `UISignalTagPicker` 覆盖；真正自建、不复用原版 picker 的第三方界面仍需要专门 adapter。

- Picker row/column counts are not declared by mods. Runtime starts from the current UI surface's real dimensions, scans registered item/recipe/signal `GridIndex` data for the largest required row and column, and expands backing arrays, `ComputeBuffer`, material grid, mouse hit testing, and visible content size together.
- Picker 行列数不由模组声明。运行时会以当前 UI surface 的实际尺寸为基准，扫描已注册物品、配方和信号 `GridIndex` 数据得到需要的最大行列，并同步扩展后备数组、`ComputeBuffer`、材质网格、鼠标命中和可见内容尺寸。

- Legacy shims bridge covered calls to implemented DSPCore runtime blocks, but they are not guaranteed to reproduce every old-library edge behavior.
- 旧 API shim 会把已覆盖调用桥接到已实现的 DSPCore 运行时功能块，但不保证复刻旧库所有边缘行为。
