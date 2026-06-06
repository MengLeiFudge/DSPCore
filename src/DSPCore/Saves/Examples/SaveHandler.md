# SaveHandler

本场景用于注册 DSPCore 模组存档处理器。

## 适用时机

- 模组需要随游戏存档保存自己的状态。
- 希望在加载没有本模组数据的存档时获得初始化回调。

## 关键参数

- `modGuid`：稳定模组 GUID。
- `ICoreSaveHandler.Export`：写出模组数据。
- `ICoreSaveHandler.Import`：读入模组数据。
- `ICoreSaveHandler.IntoOtherSave`：进入无本模组数据存档时重置状态。

## 运行时前提

在插件启动或功能注册阶段调用 `Saves.Register`。复杂或可变字段建议结合 `SaveBlockFormat`。

## 常见误用

- 不要漏写格式版本。
- 不要在 `IntoOtherSave` 中保留上一份存档的运行时状态。

代码示例见 `SaveHandlerExample.cs`。
