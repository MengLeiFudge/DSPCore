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

- 提供 P0/P1 的作者可见功能块：功能生命周期、数据阶段、物品/配方/科技/指引注册、建造栏位置、资源、图标、本地化、分页、选择器、配方类型、按键、存档、成就和错误报告。
- 提供 `xiaoye97.LDBTool`、`crecheng.DSPModSave`、`CommonAPI` 和 `BuildBarTool` 的旧 API 兼容层；兼容代码按所属功能块放入 `Compat/`，不再使用集中式 `Legacy/` 目录。
- 公开 API 提供中英文 XML summary。

当前版本已接入 P0/P1 运行时桥接：BepInEx/Harmony 启动、Proto 写入、多行建造栏绑定、资源/图标加载、物品/配方/制造器分页、选择器弹窗、自定义配方类型限制、按键回调、DSPCore 独立存档、旧 DSPModSave 处理器桥接、成就/异常/平台策略补丁、错误报告、错误窗口复制/关闭按钮和本地化条目。

## 功能块

P0/P1 是当前实现目标。

- 功能生命周期：声明功能块、依赖、优先级和初始化。
- 数据阶段：`Data`、`DataUpdates` 和 `DataFinalFixes`。
- 原型功能：物品、配方、科技、指引、模型/建筑绑定和原版数据查询描述。
- 建造栏位置：将 `ItemProto` 或物品 ID 绑定到 tab/row/index 槽位，并负责两层或更多层建造栏、玩家自定义/动态覆盖格子、相关 UI 投射和刷新，以及 RebindBuildBar / BuildBarTool 兼容。其他功能块，例如物品注册，首选在拿到 `ItemProto` 后调用 `ItemProto.BindQuickBar(...)`；BuildBar 不承担 Proto 创建职责。
- 资源、图标和本地化：资源根、图标描述和翻译条目。
- 分页和选择器：作者可以为物品、配方和制造器界面声明自定义分页，也可以打开物品、配方和信号选择器请求。
- 存档：原始 `BinaryReader`/`BinaryWriter` 处理器和 tagged block 工具。
- 成就和错误：成就策略聚合和结构化错误报告。

## 运行时状态

已接入运行时桥接：

- `DSPCorePlugin` 通过 BepInEx 启动并应用 Harmony 补丁。
- Proto 注册会在 `VFPreload.InvokeOnLoadWorkEnded` 前后执行；DSPCore 在最终修正后重建 `ProtoSet` 索引和关键派生缓存。
- `BuildBarRegistry.BindQuickBar` 会把物品 ID 或 `ItemProto` 映射到建造栏 tab/row/index 槽位；第 1 行写入原版 `UIBuildMenu.protos`，第 2 行及以后使用 DSPCore 扩展按钮。玩家自定义/动态覆盖格子和 RebindBuildBar 适配属于同一 BuildBar 功能块，但尚未实现。
- `IconSetRegistry` 可以加载 Unity `Resources` sprite 或本地 PNG 文件，缓存后写入目标 Proto。
- `TabRegistry` 会通过现有 GridIndex 分类流程把自定义分页投射到物品选择器、配方选择器和制造器界面。
- `Pickers.Open` 会请求打开物品、配方和信号选择器弹窗，并调用请求回调。
- `RecipeTypeRegistry` 会把声明的配方标记为自定义配方类型，并阻止不支持的制作器选择这些配方。
- `KeyBindRegistry` 会轮询已注册按键并调用回调，支持简单的 `Ctrl`/`Alt`/`Shift` 修饰键组合。
- `SaveRegistry` 会写入 `.dspcore` 独立存档，并按 `CoreLoadOrder` 导入处理器。
- `AchievementPolicyRegistry` 汇总每个模组的成就禁用声明；不声明或声明 `disableAchievements: false` 不会请求禁用，任意模组声明 true 时全局阻断成就变更、Milky Way / 排行榜上传和平台成就/元数据调用。没有 true 声明时，DSPCore 会屏蔽原版异常检查并保持成就可用。
- `ErrorReporter` 会接收 Unity fatal/error 日志和错误窗口事件。
- `ResourceRegistry.RegisterLocalization` 会写入 DSP 本地化 key 和语言字符串。

当前运行时限制：

- 玩家自定义建造栏位置和 RebindBuildBar 兼容尚未实现。
- 分页投射当前覆盖物品选择器、配方选择器和制造器界面。信号选择器、全息信标、蓝图等界面需要更完整的分页内容模型后才能正确支持。
- 选择器过滤当前在返回时兜底检查，尚未在实时选择器网格内隐藏无效条目。
- 自定义配方类型运行时会阻止不支持的制作器选择配方，但制作器配方选择列表尚未在选择前过滤。
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

myItemProto.BindQuickBar(tab: 3, row: 2, index: 5);
BuildBar.BindQuickBar(tab: 3, row: 2, index: 4, itemId: 9554);
```

## 示例：分页

```csharp
using DSPCore;

Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "Example Machines",
    IconId: "example-machine-tab",
    Order: 100));
```

## 示例：旧 BuildBarTool 兼容

```csharp
#pragma warning disable CS0618
BuildBarTool.BuildBarTool.SetBuildBar(3, 4, 9554, true);
#pragma warning restore CS0618
```

旧调用会被接受，但会标记为 obsolete。新模组应首选使用 `ItemProto.BindQuickBar(...)`，只有手上只有物品 ID 时再使用 `DSPCore.BuildBar.BindQuickBar(...)`。

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
- `DSPCore/Recipes/Examples/RecipeTypeRegistrationExample.cs`
- `DSPCore/Recipes/Examples/RecipeTypeRegistration.md`
- `DSPCore/Protos/Examples/ProtoPhasesExample.cs`
- `DSPCore/Protos/Examples/ProtoPhases.md`
- `DSPCore/Input/Examples/KeyBindRegistrationExample.cs`
- `DSPCore/Input/Examples/KeyBindRegistration.md`
