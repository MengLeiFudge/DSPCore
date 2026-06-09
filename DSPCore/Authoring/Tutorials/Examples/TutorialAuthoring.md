# TutorialAuthoring

本场景用于把已经准备好的 `TutorialProto` 或同类指引 proto 接入 DSPCore 的作者侧短链路。

## 适用时机

- 指引内容、触发条件、前后链路和本地化 key 已经由模组按 DSP 语义准备好。
- 需要把指引注册到 DSPCore 的 ProtoPipeline。
- 指引引用的物品、配方或科技已经在更早阶段声明时，适合放在 `DataFinalFixes` 做最后挂接。

## 关键参数

- `TutorialProto`：要注册的指引或教程原型。
- `ownerModGuid`：所属模组 GUID，用于诊断和声明归属。
- `CoreDataPhase.DataFinalFixes`：常用于依赖其他声明完成后的最终挂接。
- `purpose`：可读的注册目的，方便诊断文本定位。

## 运行时前提

在 DSPCore 应用 proto 阶段前完成注册。`RegisterTutorial(...)` 只登记当前指引，不会自动填充指引内容或本地化文本。

## 常见误用

- 不要把 Tutorials 当成自动构造器；它不填充 `TutorialProto` 的业务字段。
- 不要在 Tutorials 中注册本地化文本；文本仍使用 Resources。

代码示例见 `TutorialAuthoringExample.cs`。
