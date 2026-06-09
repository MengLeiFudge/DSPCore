# SaveHandler

本场景用于注册 DSPCore 模组存档处理器。简单对象优先用 `Saves.Auto`，需要直接控制二进制读写时再用委托或 `ICoreSaveHandler`。

## 适用时机

- 模组需要随游戏存档保存自己的状态。
- 希望在加载没有本模组数据的存档时获得初始化回调。
- 简单状态类有无参构造函数，希望 DSPCore 直接创建并注册。

## 关键参数

- `modGuid`：稳定模组 GUID。
- `CoreSaveField`：自动 schema 中的稳定字段 tag。
- `Saves.Auto<TState>(...)`：创建并注册带无参构造函数的自动 schema 状态对象。
- `ICoreSaveHandler.Export`：写出模组数据。
- `ICoreSaveHandler.Import`：读入模组数据。
- `ICoreSaveHandler.IntoOtherSave`：进入无本模组数据存档时重置状态。

## 运行时前提

在插件启动或功能注册阶段调用 `Saves.Auto` 或 `Saves.Register`。复杂或可变字段建议结合 `SaveBlockFormat`。

## 常见误用

- 不要漏写格式版本。
- 不要给自动 schema 字段使用临时 tag；tag 改名等同于新字段。
- 需要构造参数或注册前预填默认值时，不要用无参短入口，改用 `Saves.Auto(modGuid, state)`。
- 不要在 `IntoOtherSave` 中保留上一份存档的运行时状态。

代码示例见 `SaveHandlerExample.cs`。
