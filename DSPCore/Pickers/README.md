# 选择器

Pickers 模块让模组从自己的 UI 或流程中请求打开原版物品、配方或信号选择器，并通过回调拿到玩家选择结果。

## 这个模块带来什么便利

- 你不需要自己处理 `UIItemPicker`、`UIRecipePicker`、`UISignalPicker` 的弹窗调用差异。
- 选择请求进入 DSPCore 队列，在 UI update 时统一消费，减少在错误 UI 时机直接打开窗口的风险。
- 结果过滤、异常捕获和失败时回调 `null` 由 DSPCore 统一处理。
- 同一套 `PickerRequest` 能表达物品、配方和信号三类选择。

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
- 返回值如果不满足 `Filter`，DSPCore 会调用 `OnReturn(null)`。
- 打开或回调过程中发生异常时，DSPCore 会记录到 Errors，并调用 `OnReturn(null)`。

## 这个模块不负责什么

- 当前过滤器只在返回时兜底验证，不会在选择器实时网格里隐藏无效条目。
- 不保证 `OnReturn` 一定非空；取消、过滤失败或异常都会返回 null。
- 不提供自定义选择器 UI；当前使用原版 picker 弹窗。

## 示例

- `Examples/PickerRequest.md`
- `Examples/PickerRequestExample.cs`
