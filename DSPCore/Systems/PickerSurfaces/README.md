# 选择器请求

Pickers 模块让模组从自己的 UI 或流程中请求打开原版物品、配方或信号选择器，并通过回调拿到玩家选择结果。

它不负责物品/配方的注册位置模型。作者注册物品或配方时仍应使用 `GridIndex`；需要新页面时先通过 Tabs 取得 `TabSlot`。

## 这个模块带来什么便利

- 你不需要自己处理 `UIItemPicker`、`UIRecipePicker`、`UISignalPicker` 的弹窗调用差异。
- 选择请求进入 DSPCore 队列，在 UI update 时统一消费，减少在错误 UI 时机直接打开窗口的风险。
- 结果过滤、异常捕获和失败时回调 `null` 由 DSPCore 统一处理。
- 同一套 `PickerRequest` 能表达物品、配方和信号三类选择。
- 物品、配方和信号 picker 的实时网格会应用请求过滤，并对重复 `GridIndex` 自动寻找后续空格，避免后注册内容覆盖前一个入口。
- 选择器行列数会从最终 `GridIndex` 汇总结果中推导；DSPCore 会同步扩展数组、材质、鼠标命中和可见内容尺寸。

## 功能：打开一次选择器

```csharp
Pickers.Open(new PickerRequest(
    Kind: PickerKind.Item,
    OwnerModGuid: "com.example.my-mod",
    Filter: value => value is ItemProto item && item.ID > 0,
    ShowLocked: false,
    ShowAll: false,
    OnReturn: value => { /* handle item, recipe, signal id, or null */ }));
```

`Kind` 决定打开物品、配方或信号选择器。`OnReturn` 可能收到 `ItemProto`、`RecipeProto`、信号 ID，或者 `null`。

## 调用后 DSPCore 会怎么处理

- `Pickers.Open(...)` 会把请求加入队列，而不是立即打开 UI。
- 运行时 update 会消费当前队列并逐个打开选择器。
- 物品选择器会根据 `ShowAll` 或 `ShowLocked` 设置 `UIItemPicker.showAll`。
- 物品、配方和信号选择器刷新网格时会应用 `Filter`；返回值如果仍不满足 `Filter`，DSPCore 会调用 `OnReturn(null)`。
- 原版 `UIItemPicker`、`UIRecipePicker`、`UISignalPicker` 和 `UISignalTagPicker` 会获得重复 `GridIndex` 兜底和动态行列扩容；蓝图图标、描述图标和智能输入框图标等复用原版 signal/tag picker 的界面会受益。
- 打开或回调过程中发生异常时，DSPCore 会记录到 Errors，并调用 `OnReturn(null)`。

## 这个模块不负责什么

- 不分配 `TabSlot`；页面注册属于 Tabs。
- 不设置 `GridIndex`；物品/配方格子属于 ProtoRegistration 和对应 Proto 对象。
- 不保证 `OnReturn` 一定非空；取消、过滤失败或异常都会返回 null。
- 不提供自定义选择器 UI；当前使用原版 picker 弹窗。
- 不按 GenesisBook、OrbitalRing、FE 等插件 GUID 做 signal/tag picker 跳过注入；DSPCore 统一覆盖原版 picker surface。
- 不直接适配接管 UI 后自建且不复用原版 picker 的界面；这些第三方自建界面需要单独 runtime adapter。
- 不接受模组显式声明 picker 行数或列数；行列数由运行时扫描已注册 `GridIndex` 后自动计算。

## 示例

- `Examples/PickerRequest.md`
- `Examples/PickerRequestExample.cs`
