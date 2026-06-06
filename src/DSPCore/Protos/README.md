# 原型阶段

## 职责

本功能块声明 Proto 注册和 DSPCore 数据阶段。

## 公开入口

- `Api/Protos.cs`：作者侧短入口。
- `Api/ProtoRegistryFacade.cs`
- `Api/ProtoRegistration.cs`
- `Api/CoreDataPhase.cs`
- `Api/ProtoKind.cs`
- `Api/VanillaDataView.cs`

## 示例

- `Examples/ProtoPhasesExample.cs`
- `Examples/ProtoPhases.md`

## 运行时

`Runtime/ProtoRuntime.cs` 会在 `VFPreload.InvokeOnLoadWorkEnded` 前后应用 Proto 注册，并重建关键派生缓存。

## 边界

本功能块负责 Proto 写入和阶段顺序。建造栏位置、图标绑定、本地化、自定义配方类型行为属于独立功能块，可由 Proto 工作流调用。
