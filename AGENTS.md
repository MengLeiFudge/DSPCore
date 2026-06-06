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

- `docs/api-migration.md` tracks migration from legacy APIs to DSPCore APIs.
- `docs/api-migration.md` 记录旧 API 到 DSPCore API 的迁移。

- Every public capability should have a concrete `.cs` example under its feature block's `Examples/` directory. Cross-feature walkthroughs may use `docs/examples/`. All example `.cs` files are documentation artifacts and must be excluded from compilation.
- 每一项公开能力都应在所属功能块的 `Examples/` 目录下有具体 `.cs` 示例。跨功能完整流程可以放在 `docs/examples/`。所有示例 `.cs` 文件都属于文档产物，必须排除编译。

- Feature-specific examples should use paired scenario files when practical: `Examples/<Scenario>.md` explains when to use the API, key parameters, runtime prerequisites, and common mistakes; `Examples/<Scenario>Example.cs` provides the small demo code. The `.md` explanation does not replace the feature block README, and the `.cs` demo remains excluded from compilation.
- 功能专属示例在可行时应使用成对场景文件：`Examples/<Scenario>.md` 说明适用时机、关键参数、运行时前提和常见误用；`Examples/<Scenario>Example.cs` 提供小型 demo 代码。`.md` 说明不替代功能块 README，`.cs` demo 仍必须排除编译。

- Every feature block directory under `src/DSPCore/` must have `README.md` for Chinese documentation and `README-EN.md` for English documentation, excluding build/metadata directories such as `bin/`, `obj/`, and `Properties/`.
- `src/DSPCore/` 下每个功能块目录都必须有中文 `README.md` 和英文 `README-EN.md`，但 `bin/`、`obj/`、`Properties/` 等构建/元数据目录除外。

- User-visible behavior or public API changes require README/docs review in the same task.
- 用户可见行为或公开 API 变更，必须在同一任务检查 README/docs。

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
src/DSPCore/
├── Core/                    # Api/Compat/Examples: framework entry point, feature/module registries, CommonAPI compatibility
├── Achievements/            # Api/Runtime/Examples: achievement policy aggregation
├── BuildBar/                # Api/Runtime/Compat/Examples: build bar slot binding and BuildBarTool compatibility
├── Errors/                  # Api/Runtime: error report model and fatal-window runtime bridge
├── Icons/                   # Api/Runtime/Examples: icon registration and icon runtime bridge
├── Input/                   # Api/Runtime/Examples: key bind declarations and polling bridge
├── Pickers/                 # Api/Runtime/Examples: picker requests and popup bridge
├── Protos/                  # Api/Runtime/Compat/Examples: Proto registration facade, data-phase bridge, LDBTool compatibility
├── Recipes/                 # Api/Runtime/Examples: custom recipe types and recipe guard bridge
├── Resources/               # Api/Runtime: resource and localization declarations plus localization bridge
├── Runtime/                 # Runtime/: BepInEx plugin entry and cross-feature runtime assembly only
├── Saves/                   # Api/Runtime/Compat/Examples: save abstraction, sidecar save bridge, DSPModSave compatibility
├── Tabs/                    # Api/Runtime/Examples: tab declarations and UI projection bridge
└── UI/                      # Api/: UI abstraction descriptors
```

## Current Limitations / 当前限制

- The current implementation includes P0/P1 runtime bridges, not just author-facing API skeletons.
- 当前实现已包含 P0/P1 运行时桥接，不再只是作者可见 API 骨架。

- Implemented runtime bridges: BepInEx/Harmony startup, proto insertion near `VFPreload.InvokeOnLoadWorkEnded`, multi-row build bar binding, resource/icon loading, item/recipe/replicator tab projection, item/recipe/signal picker popups, custom recipe type guards, key callbacks, `.dspcore` sidecar saves, legacy DSPModSave handler bridging, achievement/abnormality/platform policy patches, error logging/fatal-window buttons, and localization entries.
- 已实现运行时桥接：BepInEx/Harmony 启动、`VFPreload.InvokeOnLoadWorkEnded` 附近的 Proto 写入、多行建造栏绑定、资源/图标加载、物品/配方/制造器分页投射、物品/配方/信号选择器弹窗、自定义配方类型限制、按键回调、`.dspcore` 独立存档、旧 DSPModSave 处理器桥接、成就/异常/平台策略补丁、错误日志/错误窗口按钮和本地化条目。

- BuildBar owns the build bar placement feature block: item id or `ItemProto` -> tab/row/index bindings, two or more build bar rows, player-defined or dynamically overridden slots, related UI projection and refresh handling, RebindBuildBar compatibility, and BuildBarTool compatibility shims. New author-facing examples should prefer `ItemProto.BindQuickBar(...)`; use `BuildBar.BindQuickBar(...)` only when the caller only has an item id. Do not move proto creation responsibilities into BuildBar.
- BuildBar 负责建造栏位置功能块：物品 ID 或 `ItemProto` -> tab/row/index 绑定、两层或更多层建造栏、玩家自定义或动态覆盖格子、相关 UI 投射和刷新处理、RebindBuildBar 兼容，以及 BuildBarTool 兼容 shim。新的作者侧示例应首选 `ItemProto.BindQuickBar(...)`；只有调用方手上只有物品 ID 时才使用 `BuildBar.BindQuickBar(...)`。不要把 Proto 创建职责移入 BuildBar。

- Runtime host owns only BepInEx startup and cross-feature patch assembly. Concrete runtime bridges belong in their feature block directories.
- Runtime 宿主只负责 BepInEx 启动和跨功能 patch 装配。具体运行时桥接必须放在对应功能块目录。

- P2/P3 features such as custom machine components, planet/star systems, network helpers, and player convenience modules are not implemented.
- 自定义机器组件、星球/恒星系统、网络工具和玩家便利模块等 P2/P3 功能尚未实现。

- Remaining P0/P1 runtime gaps: player-defined build bar positions, RebindBuildBar compatibility, signal/beacon/blueprint tab surfaces, live picker-grid filtering, and pre-selection assembler recipe list filtering.
- 剩余 P0/P1 运行时缺口：玩家自定义建造栏位置、RebindBuildBar 兼容、信号/全息信标/蓝图分页界面、选择器实时网格过滤，以及制作器配方列表选择前过滤。

- Legacy shims bridge covered calls to implemented DSPCore runtime blocks, but they are not guaranteed to reproduce every old-library edge behavior.
- 旧 API shim 会把已覆盖调用桥接到已实现的 DSPCore 运行时功能块，但不保证复刻旧库所有边缘行为。
