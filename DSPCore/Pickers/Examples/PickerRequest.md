# PickerRequest

本场景用于从你的 UI 请求打开原版物品、配方或信号选择器。

## 适用时机

- 你的设置面板或功能 UI 需要让玩家选择一个物品、配方或信号。
- 选择结果只需要通过回调返回给你的模组。

## 关键参数

- `Kind`：选择器类型。
- `OwnerModGuid`：请求归属模组。
- `Filter`：结果过滤器。
- `ShowLocked` / `ShowAll`：是否展示锁定项或全部项。
- `OnReturn`：选择完成后的回调。

## 运行时前提

请求会进入队列，由 DSPCore 在合适的 UI update 时机打开弹窗。当前过滤器在返回时兜底检查，尚未在实时网格中隐藏无效项。

## 常见误用

- 不要假设 `OnReturn` 一定返回非空对象。
- 不要把实时筛选 UI 当作当前已支持能力。

代码示例见 `PickerRequestExample.cs`。
