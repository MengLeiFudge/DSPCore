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

- `DSPCore/`：主 BepInEx 插件项目，内部拆成 `Authoring/` 作者能力和 `Systems/` 系统集成两片。
- `DSPCore/Authoring/`：模组作者直接调用的能力，例如 Core、DataPhases、ProtoAccess、Items、Recipes、Techs、Tutorials、Tabs、BuildBar、Resources、Icons、GameEnums、KeyBinds、Saves、Achievements、Components、Planets、Blueprints、Models、Options、Multiplayer、Networks、Galaxy 和 UI。
- `DSPCore/Systems/`：DSPCore 对作者声明的运行时处理，例如生命周期、Proto pipeline、分页投射、选择器 surface、快捷栏投射、资源加载、存档桥、成就策略和错误窗口。
- `DSPCore.Preloader/`：BepInEx patchers 项目，用于游戏程序集加载前 patch。
- `DSPCore.Packaging/`：Thunderstore 打包项目。

## 初版范围

- 提供 P0/P1 的作者可见能力：功能生命周期、数据阶段、原型访问、物品/配方/科技/指引注册、建造栏位置、资源、图标、本地化、分页、原版游戏枚举扩展、按键、存档、成就策略和通用 UI 框架。
- 提供 `xiaoye97.LDBTool`、`crecheng.DSPModSave`、`CommonAPI` 和 `BuildBarTool` 的旧 API 兼容层；兼容代码按所属作者能力放入 `Authoring/<Capability>/Compat/`，不再使用集中式 `Legacy/` 目录。
- 公开 API 提供中英文 XML summary。

当前版本已接入 P0/P1 和第一批 P2/P3 运行时桥接：BepInEx/Harmony 启动、Preloader 字段和枚举预留槽注入、Proto 写入、多行建造栏绑定、玩家覆盖、RebindBuildBar 配置导入、资源/图标加载、物品/配方/制造器/信号/标签图标分页、选择器弹窗和实时过滤、自定义配方类型选择前过滤、自定义物品类型标记入口、按键回调、DSPCore 独立存档、旧 DSPModSave 处理器桥接、实体组件生命周期、星球/恒星/银河系统生命周期、蓝图参数块、模型和预制体克隆、配置项绑定、可选联机软桥、网络查询适配器、补丁平台、成就/异常/平台策略补丁、错误报告、带游戏/实体上下文的可复制诊断文本、错误窗口复制/关闭按钮、本地化条目和通用 UI 窗口生命周期转发。

## 功能块

P0/P1 是当前实现目标。

- 功能生命周期：声明能力块、依赖、优先级和初始化，并可通过 `Lifecycle` 注册 DSPCore 启动、更新、销毁和常见存档链路回调。
- 数据阶段：`Data`、`DataUpdates` 和 `DataFinalFixes`。
- 原型相关能力：DataPhases 提供三阶段；ProtoAccess 通过 `ProtoPhaseContext.FindItem(...)` / `FindRecipe(...)` 和 `data.Access` 承接第二/第三阶段读取和修改他人注册数据；Items、Recipes、Techs、Tutorials 分别负责对应 proto 注册；`ItemProto` / `RecipeProto` 可用对象短入口设置 `GridIndex`、绑定图标并注册；ProtoRegistration 保留低层聚合和兼容入口。
- 建造栏位置：将 `ItemProto` 或物品 ID 绑定到 tab/row/index 槽位；第 1 行写入原版建造栏，第 2 行及以后使用 DSPCore 扩展按钮，并保留 BuildBarTool 兼容入口。其他作者能力，例如物品注册，首选在拿到 `ItemProto` 后调用 `ItemProto.SetBuildBar(...)`；BuildBar 不承担 Proto 创建职责。
- 资源、图标和本地化：通过 `ModResources` 登记资源根和翻译条目；同一模组可用 `ModResources.Pack(...)` 复用 owner、资源根和 assembly；通过 `Icons.FromResources(...)`、`Icons.FromFile(...)`、`Icons.FromEmbedded(...)`、`Icons.FromAssetBundle(...)` 或 `Icons.BindToProto(...)` 注册图标。
- 分页：作者可以声明自定义页面并取得 `TabSlot`，再用 `TabSlot` 生成物品/配方 `GridIndex`。选择器 surface 属于 DSPCore 系统实现。
- 游戏枚举：`GameEnums.RegisterRecipeType(...)` 声明自定义配方类型限制，`ItemProto.SetCustomItemType()` 标记 DSPCore 预留的自定义物品类型；运行时代码使用 `GameEnums.CustomRecipeTypeValue` / `CustomItemTypeValue` 避免直接编译期依赖 Preloader 注入字段。
- 存档：`Saves.Auto(...)` 自动 schema、委托式简单存档处理器、原始 `BinaryReader`/`BinaryWriter` 处理器和 tagged block 工具。
- 成就策略：声明是否影响银河系/排行榜上传等策略。错误窗口和错误收集属于 DSPCore 系统实现。
- UI 框架：窗口生命周期、标签页窗口、基础控件、声明式网格布局、主题卡片辅助，以及标准表单、列表、详情区和状态页脚脚手架；不包含具体业务页面。
- 实体组件：用 `Components.Register<TComponent>(...)` 按 item id、model index 或 `PrefabDesc` 条件给实体挂自定义组件，转发移除、tick 和存档；复杂构造再使用 descriptor。
- 星球/恒星/银河系统：用 `PlanetSystems.Register<TSystem>(...)`、`GalaxySystems.RegisterStar<TSystem>(...)` 或 `RegisterGalaxy<TSystem>(...)` 注册系统，按 `PlanetFactory`、`StarData` 或 `GalaxyData` 创建实例并转发生命周期；复杂构造再使用 descriptor。
- 蓝图参数：用 `Blueprints.Register(blockId, ownerModGuid, copy, paste, ...)` 注册整数负载 tagged block，避免多个模组抢 `BuildingParameters.parameters` 固定槽位；复杂 block 处理再使用 descriptor。
- 模型和预制体：从已有 `ModelProto` 克隆新模型，配置独立 `PrefabDesc`，并重建模型派生缓存。
- 配置、联机和网络：提供 `Options.String/Bool/Int/Float/Enum/IntRange/FloatRange` 短入口，支持同名方法加 `OptionUi` 设置页面、显示名、同页排序和 Reset 按钮，提供 BepInEx 配置绑定、DSPCore 统一设置窗口、设置页面、设置版本描述、配置导入/导出、Nebula 软检测、packet/host relay/planet data/client save 声明、adapter snapshot/query 入口，以及 `Networks.Register(...)` 工厂网络查询适配器短入口。
- 补丁平台：集中声明条件补丁、必需插件 GUID/version、禁用原因和应用失败报告。

## 运行时状态

已接入运行时桥接：

- `DSPCorePlugin` 通过 BepInEx 启动并应用 Harmony 补丁。
- `Lifecycle` 会在 DSPCore 运行时桥接装配后触发 `OnStarted`，在插件更新和销毁时触发 `OnUpdate` / `OnDestroyed`，并从 SaveRuntime 转发 `OnNewGame`、`OnBeforeSave`、`OnBeforeLoad`、`OnAfterLoad` 和 `OnSaveDeleted`。
- 原型注册会按类似 Factorio 的 `Data`、`DataUpdates`、`DataFinalFixes` 三阶段执行回调；阶段上下文提供当前可见 Proto 查询和修改入口；运行时在 `VFPreload.InvokeOnLoadWorkEnded` 前后写入对应 Proto，并在最终修正后重建 `ProtoSet` 索引和关键派生缓存。
- `BuildBarRegistry.BindQuickBar` 会把物品 ID 或 `ItemProto` 映射到建造栏 tab/row/index 槽位；第 1 行写入原版 `UIBuildMenu.protos`，第 2 行及以后使用 DSPCore 扩展按钮。`BuildBar.SetPlayerOverride(...)` 会写入玩家覆盖层并保存到 `.dspcore`，运行时总是用作者默认绑定叠加玩家覆盖后的有效绑定。没有 DSPCore BuildBar 存档数据时，DSPCore 会从 RebindBuildBar 的 `CustomBarBind.cfg` 导入第 1 行玩家配置。
- `IconSetRegistry` 可以加载 Unity `Resources` sprite、本地 PNG 文件、已加载 assembly 中的嵌入 PNG 或 AssetBundle 中的 `Sprite` / `Texture2D`，缓存后写入目标 Proto；作者侧短入口是 `Icons.FromResources(...)`、`Icons.FromFile(...)`、`Icons.FromEmbedded(...)`、`Icons.FromAssetBundle(...)` 和 `Icons.BindToProto(...)`。同一模组有统一资源根时，可先创建 `ModResources.Pack(...)`，再用 pack 上的图标方法减少重复参数。
- `TabRegistry` 会为稳定页面 ID 分配 `TabSlot`，并通过现有 GridIndex 分类流程把自定义页面投射到物品选择器、配方选择器、制造器界面、信号选择器和标签图标选择器。
- `PickerSurfaces` 会处理物品、配方和信号选择器 surface，实时网格会应用过滤、重复 `GridIndex` 兜底和动态行列扩容。
- `GameEnums.RegisterRecipeType(...)` 当前会把声明的配方标记为自定义配方类型，并在制作器配方列表打开前隐藏当前机器不能使用的配方；`ItemProto.SetCustomItemType()` 会把已有物品标记为 DSPCore 预留的自定义物品类型；`RecipeTypes` 保留为旧别名，`AssemblerComponent.SetRecipe` 仍保留最终保护。
- `KeyBindRegistry` 会轮询已注册按键并调用回调；作者侧短入口是 `KeyBinds.Register(id, ownerModGuid, displayName, defaultKey, callback, ...)`，支持简单的 `Ctrl`/`Alt`/`Shift` 修饰键组合；`CanOverride=true` 的按键会进入 DSPCore 统一设置窗口，玩家可用 Capture 捕获按键或直接编辑文本，同一 `ConflictGroup` 内的同键配置会显示冲突提示；运行时优先读取玩家配置，配置为空或非法时回落默认键。
- `SaveRegistry` 会写入 `.dspcore` 独立存档，并按 `CoreLoadOrder` 导入处理器。
- `AchievementPolicyRegistry` 汇总每个模组的成就禁用声明；不声明或声明 `disableAchievements: false` 不会请求禁用，任意模组声明 true 时全局阻断成就变更、Milky Way / 排行榜上传和平台成就/元数据调用。没有 true 声明时，DSPCore 会屏蔽原版异常检查并保持成就可用。
- `ErrorWindow` 会接收 Unity fatal/error 日志和错误窗口事件，并生成包含当前游戏状态、可选星球/实体上下文、最近错误、候选插件文本命中、DSPCore 声明和 Harmony patch map 概览的可复制诊断文本。
- `ResourceRegistry.RegisterLocalization` 会写入 DSP 本地化 key 和语言字符串；作者侧短入口是 `ModResources.Root(...)`、`ModResources.Text(...)` 和 `ModResources.Pack(...)`。
- `UiWindowManager` 会在 `UIRoot` 打开、更新和销毁时转发 DSPCore 窗口生命周期；具体窗口由模组自己创建和打开。
- `Components` 会在 `PlanetFactory.CreateEntityLogicComponents` 后按描述创建组件；无参构造组件可用 `Components.Register<TComponent>(...)` 短入口注册。运行时会在实体移除、电力 tick、工厂 tick 和后置阶段转发回调；组件数据写入 `.dspcore`，未加载星球的数据会延迟到 `GameData.GetOrCreateFactory` 后恢复。
- `Planets` 会在 `GameData.GetOrCreateFactory` 后为每个 `PlanetFactory` 创建星球系统；无参构造系统可用 `PlanetSystems.Register<TSystem>(...)` 短入口注册。运行时转发本地星球绘制、电力 tick、工厂 tick 和后置阶段。
- `Blueprints` 会把作者参数块编码到 `BuildingParameters` 参数数组尾部，在复制、蓝图、粘贴和预建筑落地链路中保持 block ID；简单参数块可用 `Blueprints.Register(...)` 直接处理 `int[]` 负载。
- `Models` 会在最终 Proto 派生缓存重建前克隆 `ModelProto` 和 `PrefabDesc`，然后重建 `ModelProto` 索引和 `PlanetFactory.PrefabDescByModelIndex`。
- `Options` 会把作者声明的字符串配置项绑定到 DSPCore 的 BepInEx 配置文件，并保存设置页面和设置版本描述；`String`、`Bool`、`Int`、`Float`、`Enum`、`IntRange` 和 `FloatRange` 会注册配置并返回当前值，需要页面、显示名、排序或 Reset 按钮时可给同名短入口传入 `OptionUi`；`ExportValues` / `ExportText` 与 `ImportValues` / `ImportText` 提供配置快照导入导出；`Options.OpenWindow()` 会打开 DSPCore 自有统一设置窗口。
- `Multiplayer` 当前检测 Nebula 是否加载，保存 packet、host relay、planet data request 和 client missing-save 声明，并提供 adapter snapshot/query 入口；真实 Nebula 发送由专门适配器接入。
- `Networks` 提供 `Register(...)` 适配器短入口，以及 `TryGetCommonNetwork(...)` 和 `IsConnectedToNetwork(...)` 查询表面；具体网络扫描由注册适配器提供。
- `Galaxy` 会在银河数据存在后创建恒星/银河系统；无参构造系统可用 `GalaxySystems.RegisterStar<TSystem>(...)` / `RegisterGalaxy<TSystem>(...)` 短入口注册。运行时在 `SpaceSector.GameTick` 转发更新和 sidecar 存档。
- `PatchRuntime` 会应用 `PatchDescriptor` 声明的条件补丁，记录禁用原因和失败异常。

当前运行时限制：

- RebindBuildBar 的 `BuildBarBinds` 配置会导入到 DSPCore 第 1 行玩家覆盖层；DSPCore 不接管 RebindBuildBar 自己的重绑 UI、快捷键或后续配置写回。
- 分页投射当前覆盖原版 `UIItemPicker`、`UIRecipePicker`、`UIReplicatorWindow`、`UISignalPicker` 和 `UISignalTagPicker`。蓝图图标、蓝图说明图标、智能输入框图标等使用这些原版 picker 的界面会受益；DSPCore 不按 GenesisBook、OrbitalRing、FE 等插件 GUID 跳过注入。真正自建且不复用原版 picker 的第三方界面需要专门 adapter。
- 选择器行列数由运行时汇总 `GridIndex` 后计算；DSPCore 会以当前 UI surface 的实际尺寸为基准，统计所有相关物品、配方或信号格子需要的最大行列，并同步扩展数组、材质、鼠标命中和显示尺寸。模组不需要也不能单独声明 picker 长宽。
- UI 框架只提供通用架子，不注册具体页面、业务导航、解锁条件或存档状态。
- 当前 Proto 阶段挂点是保守的第一版桥接，不是最终 VFPreload 中段生命周期。
- 可选联机桥当前不直接发送 Nebula packet；需要专门 Nebula 适配器读取 DSPCore multiplayer registry。
- 网络模块当前是查询适配平台，不内置所有原版或第三方网络扫描。
- 玩家便利模块，例如 RecipeFinder、FreeMechaCustom、AssemblerUI 风格功能，仍未并入核心。

## 示例：原型三阶段注册

```csharp
using DSPCore;

ProtoRegistration.Data("com.example.my-mod", data =>
{
    itemProto
        .SetGridIndex(tab: 3, row: 1, index: 5)
        .RegisterItem("com.example.my-mod", purpose: "Declare base item");
});

ProtoRegistration.DataUpdates("com.example.my-mod", data =>
{
    recipeProto
        .SetGridIndex(tab: 3, row: 1, index: 6)
        .RegisterRecipe("com.example.my-mod", CoreDataPhase.DataUpdates, "Attach recipe after item declarations");

    ItemProto baseItem = data.FindItem(1001);
    if (baseItem != null)
        baseItem.GridIndex = GridIndexes.From(tab: 3, row: 1, index: 5);
});

ProtoRegistration.DataFinalFixes("com.example.my-mod", data =>
{
    data.RegisterTutorial(tutorialProto, "Final tutorial chain fix");
});
```

## 示例：成就策略

```csharp
using DSPCore;

Achievements.Declare("com.example.my-mod", disableAchievements: true);

bool disabled = Achievements.ShouldDisableAchievements();
```

不调用或声明 `disableAchievements: false` 表示该模组不要求禁用成就；多个模组同时声明时，任意 true 胜出。详细边界见 `DSPCore/Authoring/Achievements/README.md`。

## 示例：建造栏

```csharp
using DSPCore;

myItemProto.SetBuildBar(tab: 3, row: 2, index: 5);
BuildBar.BindQuickBar(tab: 3, row: 2, index: 4, itemId: 9554);
```

## 示例：配置、资源和图标短入口

```csharp
using DSPCore;

bool enabled = Options.Bool("Example", "Enabled", true, "Enable example feature.");
int rows = Options.Int("Example", "Rows", 2, "Example row count.");
int maxRows = Options.IntRange("Example", "MaxRows", 3, "Maximum rows.", minimum: 1, maximum: 6);
// 在按钮、快捷键或自定义 UI 回调中打开，调用时 UIRoot 必须已经初始化。
// Open from a button, key bind, or custom UI callback after UIRoot is ready.
Options.OpenWindow();

var pack = ModResources.Pack(
    ownerModGuid: "com.example.my-mod",
    rootPath: "assets/icons",
    assembly: typeof(MyPlugin).Assembly);

pack.Text("ExampleMachines", "zhCN", "示例机器");
pack.IconFromEmbedded("example-embedded", "ExampleMod.Assets.example.png");
pack.IconFromAssetBundle("example-bundle", "example-icons", "example-machine");
pack.BindIconToProto("example-machine", "example.png", ProtoKind.Item, 9554);
```

## 示例：自动存档、委托式存档和生命周期

```csharp
using DSPCore;

private sealed class ExampleState
{
    [CoreSaveField("counter")]
    public int Counter { get; set; }
}

private static readonly ExampleState State = Saves.Auto("com.example.auto-mod", new ExampleState());

Saves.Register(
    modGuid: "com.example.my-mod",
    export: writer => writer.Write(counter),
    import: reader => counter = reader.ReadInt32(),
    intoOtherSave: () => counter = 0);

Lifecycle.OnStarted(InitializeAfterDspCore);
Lifecycle.OnBeforeSave(saveName => FlushTransientCache(saveName));
Lifecycle.OnAfterLoad(RebuildTransientCache);
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
- 能力示例采用 `Examples/<Scenario>.md` + `Examples/<Scenario>Example.cs` 成对文件；`.cs` 示例只作为文档产物，不参与编译。
- `DSPCore/Authoring/Core/Examples/LifecycleExample.cs`
- `DSPCore/Authoring/Core/Examples/Lifecycle.md`
- `DSPCore/Authoring/Achievements/Examples/AchievementPolicyExample.cs`
- `DSPCore/Authoring/Achievements/Examples/AchievementPolicy.md`
- `DSPCore/Authoring/BuildBar/Examples/QuickBarBindingExample.cs`
- `DSPCore/Authoring/BuildBar/Examples/QuickBarBinding.md`
- `DSPCore/Authoring/Saves/Examples/SaveHandlerExample.cs`
- `DSPCore/Authoring/Saves/Examples/SaveHandler.md`
- `DSPCore/Authoring/Saves/Examples/SaveBlocksExample.cs`
- `DSPCore/Authoring/Saves/Examples/SaveBlocks.md`
- `DSPCore/Authoring/Icons/Examples/IconSetRegistrationExample.cs`
- `DSPCore/Authoring/Icons/Examples/IconSetRegistration.md`
- `DSPCore/Authoring/Resources/Examples/ResourceRegistrationExample.cs`
- `DSPCore/Authoring/Resources/Examples/ResourceRegistration.md`
- `DSPCore/Authoring/Items/Examples/ItemAuthoringChainExample.cs`
- `DSPCore/Authoring/Items/Examples/ItemAuthoringChain.md`
- `DSPCore/Authoring/Recipes/Examples/RecipeAuthoringChainExample.cs`
- `DSPCore/Authoring/Recipes/Examples/RecipeAuthoringChain.md`
- `DSPCore/Authoring/Tabs/Examples/TabRegistrationExample.cs`
- `DSPCore/Authoring/Tabs/Examples/TabRegistration.md`
- `DSPCore/Systems/PickerSurfaces/Examples/PickerRequestExample.cs`
- `DSPCore/Systems/PickerSurfaces/Examples/PickerRequest.md`
- `DSPCore/Authoring/GameEnums/Examples/RecipeTypeRegistrationExample.cs`
- `DSPCore/Authoring/GameEnums/Examples/RecipeTypeRegistration.md`
- `DSPCore/Authoring/DataPhases/Examples/ProtoPhasesExample.cs`
- `DSPCore/Authoring/DataPhases/Examples/ProtoPhases.md`
- `DSPCore/Authoring/KeyBinds/Examples/KeyBindRegistrationExample.cs`
- `DSPCore/Authoring/KeyBinds/Examples/KeyBindRegistration.md`
- `DSPCore/Authoring/UI/Examples/WindowScaffoldExample.cs`
- `DSPCore/Authoring/UI/Examples/WindowScaffold.md`
- `DSPCore/Authoring/Components/Examples/EntityComponentExample.cs`
- `DSPCore/Authoring/Components/Examples/EntityComponent.md`
- `DSPCore/Authoring/Planets/Examples/PlanetSystemExample.cs`
- `DSPCore/Authoring/Planets/Examples/PlanetSystem.md`
- `DSPCore/Authoring/Blueprints/Examples/BuildingParametersExample.cs`
- `DSPCore/Authoring/Blueprints/Examples/BuildingParameters.md`
- `DSPCore/Authoring/Models/Examples/CloneModelExample.cs`
- `DSPCore/Authoring/Models/Examples/CloneModel.md`
- `DSPCore/Authoring/Options/Examples/OptionRegistrationExample.cs`
- `DSPCore/Authoring/Options/Examples/OptionRegistration.md`
- `DSPCore/Authoring/Multiplayer/Examples/SoftPacketExample.cs`
- `DSPCore/Authoring/Multiplayer/Examples/SoftPacket.md`
- `DSPCore/Authoring/Networks/Examples/CommonNetworkExample.cs`
- `DSPCore/Authoring/Networks/Examples/CommonNetwork.md`
- `DSPCore/Authoring/Galaxy/Examples/GalaxyLifecycleExample.cs`
- `DSPCore/Authoring/Galaxy/Examples/GalaxyLifecycle.md`
