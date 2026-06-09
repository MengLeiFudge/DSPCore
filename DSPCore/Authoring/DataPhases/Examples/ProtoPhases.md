# ProtoPhases

本场景用于注册 DSPCore 的三段 Proto 数据回调，并在回调内添加或调整 Proto。

## 适用时机

- 模组需要添加或调整 `ItemProto`、`RecipeProto`、`TechProto`、`TutorialProto` 等数据。
- 物品、配方、科技或指引之间存在依赖顺序，需要像 Factorio 一样区分基础声明、依赖更新和最终修正。
- 作者希望把“这个阶段要做什么”集中写在阶段回调里，而不是给每个 Proto 单独手填 phase。

## 关键参数

- `ownerModGuid`：归属模组 GUID。
- `configure`：阶段回调，DSPCore 在对应阶段执行并传入 `ProtoPhaseContext`。
- `priority`：同一阶段内的执行顺序，数值越小越早；相同 priority 保持注册顺序。
- `ProtoPhaseContext`：当前阶段上下文，提供 `RegisterItem`、`RegisterRecipe`、`RegisterTech`、`RegisterTutorial` 和通用 `Register`；类型化注册会返回原 proto，便于链式继续配置。
- `purpose`：阶段回调或 Proto 注册目的说明。

## 运行时前提

在 DSPCore 应用数据阶段前注册 `Data`、`DataUpdates` 和 `DataFinalFixes` 回调。回调会在对应阶段执行；回调内通过 `ProtoPhaseContext` 注册的 Proto 会归入当前阶段并在同一阶段写入 LDB。

建造栏、图标、本地化和游戏枚举扩展属于独立作者能力，应由 Proto 工作流在合适位置调用。

## 常见误用

- 不要把所有更新都塞进 `DataFinalFixes`。
- 不要在已经进入某个阶段后再注册同阶段回调；阶段回调应在启动阶段声明完毕。
- 不要在 ProtoRegistration 中承担 BuildBar 或 Icons 的职责。

代码示例见 `ProtoPhasesExample.cs`。
