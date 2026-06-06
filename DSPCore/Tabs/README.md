# 分页槽位

Tabs 模块让模组声明自定义页面，并取得可用于物品/配方 `GridIndex` 的 `TabSlot`。DSPCore 会把这些页面投射到当前已支持的原版 UI 表面。

## 这个模块带来什么便利

- 你不需要自己分配自定义 tab 编号，也不需要协调多个模组之间的页面编号冲突。
- 你不需要自己 clone 原版分类按钮、计算按钮位置或 hook 每个窗口的创建流程。
- 多个模组可以把分页声明交给同一个 `TabRegistry`，DSPCore 会为每个稳定 `Id` 保留同一个 `TabSlot`。
- 分页图标可以引用 Icons 模块的图标 ID，标题可以使用 DSP 本地化 key。
- DSPCore 会把分页点击转回原版 `OnTypeButtonClick` 流程，让按钮状态和原版分类逻辑保持一致。

## 功能：声明自定义页面

```csharp
TabSlot machinesTab = Tabs.AddTab(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "ExampleMachines",
    IconId: "example-machines-icon",
    Order: 100));
```

`Id` 应保持稳定；`OwnerModGuid` 用于归属；`Title` 通常写本地化 key；`IconId` 指向 Icons 中已注册的图标；`Order` 用于表达按钮显示排序。返回的 `TabSlot` 是页面槽位，不是物品/配方 `GridIndex`。

## 功能：把物品或配方放到页面格子

`GridIndex` 是 `ItemProto` / `RecipeProto` 自己的游戏原生格子字段。创建物品或配方时，用 `TabSlot`、行号和格子号生成 `GridIndex`：

```csharp
itemProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
recipeProto.GridIndex = ProtoRegistration.GetGridIndex(machinesTab, row: 1, index: 5);
```

如果不注册新页面，也可以继续用游戏原本的 tab 分类编号生成 `GridIndex`：

```csharp
itemProto.GridIndex = ProtoRegistration.GetGridIndex(tab: 1, row: 2, index: 3);
```

## 调用后 DSPCore 会怎么处理

- 注册阶段按 `Id` 保存 descriptor；同一个 `Id` 后一次声明会覆盖前一次，但保留已经分配的 `TabSlot`。
- 新 `Id` 会从 DSPCore 自定义页面范围分配新的 `TabSlot`。
- 创建按钮时，DSPCore 会按 `Order` 再按 `Id` 排序；显示顺序不改变已分配的 `TabSlot` 值。
- `UIItemPicker`、`UIRecipePicker`、`UIReplicatorWindow`、`UISignalPicker` 和 `UISignalTagPicker` 创建时，DSPCore 会 clone 原版 type button，并为每个声明创建一个额外按钮。
- 按钮标题会走 `.Translate()`；如果能通过 `IconId` 找到图标，DSPCore 会解析 sprite 并写入按钮 image。
- 点击自定义按钮时，DSPCore 会调用对应窗口的原版 `OnTypeButtonClick`，并在选择变化时刷新按钮可点击状态。

## 这个模块不负责什么

- 不直接注册物品或配方；物品/配方仍通过 ProtoRegistration 注册，并用自己的 `GridIndex` 指向页面里的格子。
- 不支持所有 DSP UI 表面；当前覆盖原版物品选择器、配方选择器、制造器窗口、信号选择器和标签图标选择器。
- 蓝图图标、描述图标和智能输入框图标等复用原版 signal/tag picker 的界面会受益；GenesisBook、OrbitalRing、FE 等接管 UI 的第三方界面需要专门适配。
- 不创建图标或本地化；需要图标和文本时分别使用 Icons 与 Resources。

## 示例

- `Examples/TabRegistration.md`
- `Examples/TabRegistrationExample.cs`
