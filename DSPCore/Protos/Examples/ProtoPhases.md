# ProtoPhases

本场景用于把 Proto 注册到 DSPCore 的数据阶段。

## 适用时机

- 模组需要添加或调整 `ItemProto`、`RecipeProto`、`TechProto`、`TutorialProto` 等数据。
- 多个 Proto 之间存在依赖顺序，需要区分基础声明、依赖更新和最终修正。

## 关键参数

- `proto`：要注册的 Proto 实例。
- `ownerModGuid`：归属模组 GUID。
- `phase`：`Data`、`DataUpdates` 或 `DataFinalFixes`。
- `purpose`：注册目的说明。

## 运行时前提

在 DSPCore 应用数据阶段前完成注册。建造栏、图标、本地化和自定义配方类型属于独立功能块，应由 Proto 工作流在合适位置调用。

## 常见误用

- 不要把所有更新都塞进 `DataFinalFixes`。
- 不要在 Protos 中承担 BuildBar 或 Icons 的职责。

代码示例见 `ProtoPhasesExample.cs`。
