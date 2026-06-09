# 科技注册

Techs 是科技原型注册入口。它只表达“注册 TechProto”这一个作者能力；三阶段执行时机由 DataPhases 提供，底层写入和缓存重建由 Systems/ProtoPipeline 处理。

## 这个模块带来什么便利

- 已经拿到 `TechProto` 时，可以直接调用对象扩展方法注册，避免回到大的 ProtoRegistration 聚合入口。
- `Techs.Register(TechProto, ...)` 会注册科技并返回同一个对象，适合和其他作者侧配置代码串起来。
- `Techs.Register(object, ...)` 仍保留为低层入口，用于兼容旧代码或批量注册。

## 功能：对象链式注册

```csharp
techProto.RegisterTech(
    ownerModGuid: "com.example.my-mod",
    phase: CoreDataPhase.DataUpdates,
    purpose: "Unlock example machine");
```

`RegisterTech(...)` 只把当前科技登记到 DSPCore 的 ProtoPipeline。科技字段、解锁树、前置科技和配方引用仍由作者按 DSP 语义准备。

## 功能：低层注册

```csharp
Techs.Register(techProto, "com.example.my-mod", CoreDataPhase.DataUpdates, "Unlock example machine");
```

需要在数据阶段回调里和其他 Proto 一起声明时，也可以继续使用 `ProtoPhaseContext.RegisterTech(...)`。

## 这个模块不负责什么

- 不自动创建 `TechProto`，科技字段仍由作者准备。
- 不负责物品、配方、教程或 UI 页面；对应能力分别在 Items、Recipes、Tutorials 和 UI。
- 不负责调整其他模组已注册科技；跨模组查询和修改放在 DataUpdates / DataFinalFixes 的 `ProtoPhaseContext.Access`。

## 示例

- `Examples/TechAuthoring.md`
- `Examples/TechAuthoringExample.cs`
