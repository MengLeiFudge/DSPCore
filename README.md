# DSPCore

DSPCore 是戴森球计划模组的新通用底层标准。

## 目标

- 为常见模组基础设施提供统一 API。
- 逐步替代分散的 LDBTool、DSPModSave、CommonAPI、BuildBarTool、ErrorAnalyzer 式诊断、成就策略聚合、图标注册和旧 API 兼容需求。
- 保留旧 API 命名空间作为 `[Obsolete]` 兼容层，让已有模组可以先运行，再逐步迁移。

## 包信息

- Thunderstore 包名：`MengLei-DSPCore`
- DLL 名称：`DSPCore.dll`
- 新命名空间：`DSPCore`
- 英文说明：`README-EN.md`

## 项目结构

- `DSPCore/`：主 BepInEx 插件项目，包含 Core 和各功能块。
- `DSPCore.Preloader/`：BepInEx patchers 项目，用于游戏程序集加载前 patch。
- `DSPCore.Packaging/`：Thunderstore 打包项目。

## 初版范围

- 提供 P0/P1 的作者可见功能块：功能生命周期、数据阶段、原型注册、建造栏位置、资源、图标、本地化、分页、选择器、配方类型、按键、存档、成就、错误报告和通用 UI 框架。
- 提供 `xiaoye97.LDBTool`、`crecheng.DSPModSave`、`CommonAPI` 和 `BuildBarTool` 的旧 API 兼容层；兼容代码按所属功能块放入 `Compat/`，不再使用集中式 `Legacy/` 目录。
- 公开 API 提供中英文 XML summary。

当前版本已接入 P0/P1 运行时桥接：BepInEx/Harmony 启动、Proto 写入、多行建造栏绑定、玩家覆盖、资源/图标加载、物品/配方/制造器/信号/标签图标分页、选择器弹窗和实时过滤、自定义配方类型选择前过滤、按键回调、DSPCore 独立存档、旧 DSPModSave 处理器桥接、成就/异常/平台策略补丁、错误报告、错误窗口复制/关闭按钮、本地化条目和通用 UI 窗口生命周期转发。

## 功能块

P0/P1 是当前实现目标。

- 功能生命周期：声明功能块、依赖、优先级和初始化。
- 数据阶段：`Data`、`DataUpdates` 和 `DataFinalFixes`。
- 原型注册：物品、配方、科技、指引、模型/建筑绑定和原版数据查询描述。
- 建造栏位置：将 `ItemProto` 或物品 ID 绑定到 tab/row/index 槽位；第 1 行写入原版建造栏，第 2 行及以后使用 DSPCore 扩展按钮，并保留 BuildBarTool 兼容入口。其他功能块，例如物品注册，首选在拿到 `ItemProto` 后调用 `ItemProto.SetBuildBar(...)`；BuildBar 不承担 Proto 创建职责。
- 资源、图标和本地化：资源根、图标描述和翻译条目。
- 分页和选择器：作者可以声明自定义页面并取得 `TabSlot`，再用 `TabSlot` 生成物品/配方 `GridIndex`；也可以从自己的 UI 打开物品、配方和信号选择器请求。
- 存档：原始 `BinaryReader`/`BinaryWriter` 处理器和 tagged block 工具。
- 成就和错误：成就策略聚合和结构化错误报告。
- UI 框架：窗口生命周期、标签页窗口、基础控件、声明式网格布局和主题卡片辅助；不包含具体业务页面。

## 运行时状态

已接入运行时桥接：

- `DSPCorePlugin` 通过 BepInEx 启动并应用 Harmony 补丁。
- 原型注册会在 `VFPreload.InvokeOnLoadWorkEnded` 前后执行；DSPCore 在最终修正后重建 `ProtoSet` 索引和关键派生缓存。
- `BuildBarRegistry.BindQuickBar` 会把物品 ID 或 `ItemProto` 映射到建造栏 tab/row/index 槽位；第 1 行写入原版 `UIBuildMenu.protos`，第 2 行及以后使用 DSPCore 扩展按钮。`BuildBar.SetPlayerOverride(...)` 会写入玩家覆盖层并保存到 `.dspcore`，运行时总是用作者默认绑定叠加玩家覆盖后的有效绑定。
- `IconSetRegistry` 可以加载 Unity `Resources` sprite 或本地 PNG 文件，缓存后写入目标 Proto。
- `TabRegistry` 会为稳定页面 ID 分配 `TabSlot`，并通过现有 GridIndex 分类流程把自定义页面投射到物品选择器、配方选择器、制造器界面、信号选择器和标签图标选择器。
- `Pickers.Open` 会请求打开物品、配方和信号选择器弹窗，实时网格会应用请求过滤和重复 `GridIndex` 兜底，返回时仍会再做一次过滤检查。
- `RecipeTypeRegistry` 会把声明的配方标记为自定义配方类型，并在制作器配方列表打开前隐藏当前机器不能使用的配方；`AssemblerComponent.SetRecipe` 仍保留最终保护。
- `KeyBindRegistry` 会轮询已注册按键并调用回调，支持简单的 `Ctrl`/`Alt`/`Shift` 修饰键组合。
- `SaveRegistry` 会写入 `.dspcore` 独立存档，并按 `CoreLoadOrder` 导入处理器。
- `AchievementPolicyRegistry` 汇总每个模组的成就禁用声明；不声明或声明 `disableAchievements: false` 不会请求禁用，任意模组声明 true 时全局阻断成就变更、Milky Way / 排行榜上传和平台成就/元数据调用。没有 true 声明时，DSPCore 会屏蔽原版异常检查并保持成就可用。
- `ErrorReporter` 会接收 Unity fatal/error 日志和错误窗口事件。
- `ResourceRegistry.RegisterLocalization` 会写入 DSP 本地化 key 和语言字符串。
- `UiWindowManager` 会在 `UIRoot` 打开、更新和销毁时转发 DSPCore 窗口生命周期；具体窗口由模组自己创建和打开。

当前运行时限制：

- RebindBuildBar 的外部玩家配置读取尚未接入；当前玩家覆盖层是 DSPCore 自有存档数据。
- 分页投射当前覆盖原版 `UIItemPicker`、`UIRecipePicker`、`UIReplicatorWindow`、`UISignalPicker` 和 `UISignalTagPicker`。蓝图图标、蓝图说明图标、智能输入框图标等使用这些原版 picker 的界面会受益；GenesisBook、OrbitalRing、FE 这类接管 UI 的第三方界面仍需要单独适配。
- UI 框架只提供通用架子，不注册具体页面、业务导航、解锁条件或存档状态。
- 当前 Proto 阶段挂点是保守的第一版桥接，不是最终 VFPreload 中段生命周期。

P2/P3 的自定义机器组件、星球/恒星系统、网络工具和玩家便利模块仍是 TODO，尚未实现。

## 示例：成就策略

```csharp
using DSPCore;

Achievements.Declare("com.example.my-mod", disableAchievements: true);

bool disabled = Achievements.ShouldDisableAchievements();
```

不调用或声明 `disableAchievements: false` 表示该模组不要求禁用成就；多个模组同时声明时，任意 true 胜出。详细边界见 `DSPCore/Achievements/README.md`。

## 示例：建造栏

```csharp
using DSPCore;

myItemProto.SetBuildBar(tab: 3, row: 2, index: 5);
BuildBar.BindQuickBar(tab: 3, row: 2, index: 4, itemId: 9554);
```

## 示例：分页和 GridIndex

```csharp
using DSPCore;

TabSlot machinesTab = Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "Example Machines",
    IconId: "example-machine-tab",
    Order: 100));

itemProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
recipeProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
```

## 示例：旧 BuildBarTool 兼容

```csharp
#pragma warning disable CS0618
BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
#pragma warning restore CS0618
```

旧调用会被接受，但会标记为 obsolete。新模组应首选使用 `ItemProto.SetBuildBar(...)`，只有手上只有物品 ID 时再使用 `DSPCore.BuildBar.BindQuickBar(...)`。

## 文档

- `README-EN.md`
- 功能块示例采用 `Examples/<Scenario>.md` + `Examples/<Scenario>Example.cs` 成对文件；`.cs` 示例只作为文档产物，不参与编译。
- `DSPCore/Achievements/Examples/AchievementPolicyExample.cs`
- `DSPCore/Achievements/Examples/AchievementPolicy.md`
- `DSPCore/BuildBar/Examples/QuickBarBindingExample.cs`
- `DSPCore/BuildBar/Examples/QuickBarBinding.md`
- `DSPCore/Saves/Examples/SaveHandlerExample.cs`
- `DSPCore/Saves/Examples/SaveHandler.md`
- `DSPCore/Saves/Examples/SaveBlocksExample.cs`
- `DSPCore/Saves/Examples/SaveBlocks.md`
- `DSPCore/Icons/Examples/IconSetRegistrationExample.cs`
- `DSPCore/Icons/Examples/IconSetRegistration.md`
- `DSPCore/Tabs/Examples/TabRegistrationExample.cs`
- `DSPCore/Tabs/Examples/TabRegistration.md`
- `DSPCore/Pickers/Examples/PickerRequestExample.cs`
- `DSPCore/Pickers/Examples/PickerRequest.md`
- `DSPCore/RecipeTypes/Examples/RecipeTypeRegistrationExample.cs`
- `DSPCore/RecipeTypes/Examples/RecipeTypeRegistration.md`
- `DSPCore/ProtoRegistration/Examples/ProtoPhasesExample.cs`
- `DSPCore/ProtoRegistration/Examples/ProtoPhases.md`
- `DSPCore/Input/Examples/KeyBindRegistrationExample.cs`
- `DSPCore/Input/Examples/KeyBindRegistration.md`
- `DSPCore/UI/Examples/WindowScaffoldExample.cs`
- `DSPCore/UI/Examples/WindowScaffold.md`
