# 选择器

## 职责

本功能块声明打开原版物品、配方或信号选择器弹窗的请求。

## 公开入口

- `Pickers`：作者侧短入口。
- `PickerRequest`
- `PickerRegistry`
- `PickerKind`

## 示例

- `Examples/PickerExample.cs`

## 运行时

`PickerRuntime.cs` 会消费队列请求、打开选择器、执行返回时过滤，并调用 `OnReturn`。

## 边界

过滤器当前验证返回值，尚未在实时选择器网格内隐藏无效条目。
