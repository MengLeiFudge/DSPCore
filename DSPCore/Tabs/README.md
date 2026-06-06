# 分页

Tabs 模块让模组声明自定义分页按钮，并由 DSPCore 把这些分页投射到当前已支持的原版 UI 表面。

## 这个模块带来什么便利

- 你不需要自己 clone 原版分类按钮、计算按钮位置或 hook 每个窗口的创建流程。
- 多个模组可以把分页声明交给同一个 `TabRegistry`，避免各自改 UI 时互相覆盖。
- 分页图标可以引用 Icons 模块的图标 ID，标题可以使用 DSP 本地化 key。
- DSPCore 会把分页点击转回原版 `OnTypeButtonClick` 流程，让按钮状态和原版分类逻辑保持一致。

## 功能：声明自定义分页

```csharp
Tabs.Add(new CoreTabDescriptor(
    Id: "example-machines",
    OwnerModGuid: "com.example.my-mod",
    Title: "ExampleMachines",
    IconId: "example-machines-icon",
    Order: 100));
```

`Id` 应保持稳定；`OwnerModGuid` 用于归属；`Title` 通常写本地化 key；`IconId` 指向 Icons 中已注册的图标；`Order` 用于表达排序意图。

## 调用后 DSPCore 会怎么处理

- 注册阶段按 `Id` 保存 descriptor；同一个 `Id` 后一次声明会覆盖前一次。
- 创建按钮时，DSPCore 会按 `Order` 再按 `Id` 排序。
- `UIItemPicker`、`UIRecipePicker` 和 `UIReplicatorWindow` 创建时，DSPCore 会 clone 原版 type button，并为每个声明创建一个额外按钮。
- 按钮标题会走 `.Translate()`；如果能通过 `IconId` 找到图标，DSPCore 会解析 sprite 并写入按钮 image。
- 点击自定义按钮时，DSPCore 会调用对应窗口的原版 `OnTypeButtonClick`，并在选择变化时刷新按钮可点击状态。

## 这个模块不负责什么

- 不定义分页内容集合；当前只是把分页按钮投射到原版分类流程。
- 不支持所有 DSP UI 表面；当前覆盖物品选择器、配方选择器和制造器窗口。
- 信号选择器、全息信标、蓝图等界面需要更完整的分页内容模型后再支持。
- 不创建图标或本地化；需要图标和文本时分别使用 Icons 与 Resources。

## 示例

- `Examples/TabRegistration.md`
- `Examples/TabRegistrationExample.cs`
