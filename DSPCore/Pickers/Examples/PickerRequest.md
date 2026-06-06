# PickerRequest

本场景用于从你的 UI 请求打开原版物品、配方或信号选择器。

## 适用时机

- 你的设置面板或功能 UI 需要让玩家选择一个物品、配方或信号。
- 选择结果只需要通过回调返回给你的模组。
- 这是临时选择请求，不是物品/配方注册位置模型。

## 关键参数

- `Kind`：选择器类型。
- `OwnerModGuid`：请求归属模组。
- `Filter`：结果过滤器。
- `ShowLocked` / `ShowAll`：是否展示锁定项或全部项。
- `OnReturn`：选择完成后的回调。

## 运行时前提

请求会进入队列，由 DSPCore 在合适的 UI update 时机打开弹窗。物品、配方和信号选择器刷新网格时会应用 `Filter`，返回值仍会再做一次兜底检查。

## 常见误用

- 不要用 Pickers 分配页面或格子；页面用 Tabs / `TabSlot`，物品和配方格子用 `GridIndex`。
- 不要假设 `OnReturn` 一定返回非空对象。
- 不要假设第三方重写的 picker UI 一定会走原版实时过滤；GenesisBook、OrbitalRing、FE 等接管界面需要专门适配。

代码示例见 `PickerRequestExample.cs`。
