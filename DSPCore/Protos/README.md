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

## 兼容入口

- `Compat/LDBToolShim.cs`：旧命名空间 `xiaoye97.LDBTool` 外壳；Proto 入口归本功能块，建造栏入口委托给 BuildBar 兼容桥。

## 示例

- `Examples/ProtoPhasesExample.cs`
- `Examples/ProtoPhases.md`

## 运行时

`Runtime/ProtoRuntime.cs` 会在 `VFPreload.InvokeOnLoadWorkEnded` 前后应用 Proto 注册，并重建关键派生缓存。

## 边界

本功能块负责 Proto 写入和阶段顺序。建造栏位置、图标绑定、本地化、自定义配方类型行为属于独立功能块，可由 Proto 工作流调用。
