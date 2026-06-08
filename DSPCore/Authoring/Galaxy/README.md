# 恒星和银河系统

Galaxy 模块提供恒星级和银河级生命周期。作者可以注册 `StarSystemDescriptor` 或 `GalaxySystemDescriptor`，DSPCore 会在银河数据存在后创建系统，并在 `SpaceSector.GameTick` 转发更新和 sidecar 存档。

## 这个模块带来什么便利

- 不需要每个模组维护 `starId -> system` 和银河级单例状态。
- 恒星系统和银河系统使用稳定 system ID 存档。
- 适合星系范围缓存、星际事件、恒星级扫描和跨星球调度。

## 边界

- 当前更新挂点是太空扇区 tick，不是 UIStarmap 绘制阶段。
- 星球工厂级逻辑应使用 Planets 模块。
- system ID 必须长期稳定。

## 示例

- `Examples/GalaxyLifecycle.md`
- `Examples/GalaxyLifecycleExample.cs`
