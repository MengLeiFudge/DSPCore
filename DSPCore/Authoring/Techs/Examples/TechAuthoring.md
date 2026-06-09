# TechAuthoring

本场景用于把已经准备好的 `TechProto` 接入 DSPCore 的作者侧短链路。

## 适用时机

- 科技字段、前置科技、解锁关系和配方引用已经由模组按 DSP 语义准备好。
- 需要把科技注册到 DSPCore 的 ProtoPipeline。
- 科技依赖已经声明的物品或配方时，适合放在 `DataUpdates`。

## 关键参数

- `TechProto`：要注册的科技原型。
- `ownerModGuid`：所属模组 GUID，用于诊断和声明归属。
- `CoreDataPhase.DataUpdates`：常用于依赖已声明物品或配方的科技。
- `purpose`：可读的注册目的，方便诊断文本定位。

## 运行时前提

在 DSPCore 应用 proto 阶段前完成注册。`RegisterTech(...)` 只登记当前科技，不会自动填充科技字段或修复解锁链路。

## 常见误用

- 不要把 Techs 当成自动构造器；它不填充 `TechProto` 的业务字段。
- 不要在 Techs 中修改其他模组科技；跨模组读取和修改应放在 `DataUpdates` 或 `DataFinalFixes` 的 `ProtoPhaseContext.Access`。

代码示例见 `TechAuthoringExample.cs`。
