# 指引注册

Tutorials 是指引或教程原型注册入口。它只表达“注册 TutorialProto 或同类指引 proto”这一个作者能力；三阶段执行时机由 DataPhases 提供，底层写入和缓存重建由 Systems/ProtoPipeline 处理。

## 这个模块带来什么便利

- 已经拿到 `TutorialProto` 时，可以直接在对象上注册，避免反复跳到 ProtoRegistration 聚合入口。
- `Tutorials.Register(TutorialProto, ...)` 会注册指引并返回同一个对象，适合和相邻字段配置串起来。
- `Tutorials.Register(object, ...)` 仍保留为低层入口，用于兼容旧代码或批量注册。

## 功能：对象链式注册

```csharp
tutorialProto.RegisterTutorial(
    ownerModGuid: "com.example.my-mod",
    phase: CoreDataPhase.DataFinalFixes,
    purpose: "Show example machine guide");
```

`RegisterTutorial(...)` 只把当前指引登记到 DSPCore 的 ProtoPipeline。指引内容、触发条件、前后链路和本地化 key 仍由作者按 DSP 语义准备。

## 功能：低层注册

```csharp
Tutorials.Register(tutorialProto, "com.example.my-mod", CoreDataPhase.DataFinalFixes, "Show example machine guide");
```

需要在数据阶段回调里和其他 Proto 一起声明时，也可以继续使用 `ProtoPhaseContext.RegisterTutorial(...)`。

## 这个模块不负责什么

- 不自动创建 `TutorialProto`，指引字段仍由作者准备。
- 不负责物品、配方、科技或本地化文本；对应能力分别在 Items、Recipes、Techs 和 Resources。
- 不负责运行时业务 UI 或玩家导航逻辑。

## 示例

- `Examples/TutorialAuthoring.md`
- `Examples/TutorialAuthoringExample.cs`
