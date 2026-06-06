# 选择器

## 职责

本功能块声明打开原版物品、配方或信号选择器弹窗的请求。

## 公开入口

- `Api/Pickers.cs`：作者侧短入口。
- `Api/Pickers.cs#Open(request)`：请求打开一次选择器弹窗。
- `Api/PickerRequest.cs`
- `Api/PickerRegistry.cs`
- `Api/PickerKind.cs`

## 示例

- `Examples/PickerRequest.md`
- `Examples/PickerRequestExample.cs`

## 运行时

`Runtime/PickerRuntime.cs` 会消费队列请求、打开选择器、执行返回时过滤，并调用 `OnReturn`。

## 边界

过滤器当前验证返回值，尚未在实时选择器网格内隐藏无效条目。
