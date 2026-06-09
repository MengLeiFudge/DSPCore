# 恒星和银河系统

Galaxy 模块提供恒星级和银河级生命周期。作者可用 `GalaxySystems.RegisterStar<TSystem>(...)` 或 `GalaxySystems.RegisterGalaxy<TSystem>(...)` 注册带无参构造函数的系统；需要构造时直接使用 `StarData` 或 `GalaxyData` 时再声明 descriptor。DSPCore 会在银河数据存在后创建系统，并在 `SpaceSector.GameTick` 转发更新和 sidecar 存档。

## 这个模块带来什么便利

- 不需要每个模组维护 `starId -> system` 和银河级单例状态。
- 恒星系统和银河系统使用稳定 system ID 存档。
- 适合星系范围缓存、星际事件、恒星级扫描和跨星球调度。

## 功能：短入口注册恒星/银河系统

```csharp
GalaxySystems.RegisterGalaxy<ExampleGalaxySystem>(
    systemId: "com.example.galaxy",
    ownerModGuid: "com.example.my-mod");
```

`RegisterStar<TSystem>(...)` 和 `RegisterGalaxy<TSystem>(...)` 会用无参构造函数创建系统，并把 `systemId` 与 `StarData` / `GalaxyData` 写入系统上下文。需要在构造阶段立即读取 `StarData` 或 `GalaxyData` 时，使用对应 descriptor。

## 边界

- 当前更新挂点是太空扇区 tick，不是 UIStarmap 绘制阶段。
- 星球工厂级逻辑应使用 Planets 模块。
- system ID 必须长期稳定。

## 示例

- `Examples/GalaxyLifecycle.md`
- `Examples/GalaxyLifecycleExample.cs`
