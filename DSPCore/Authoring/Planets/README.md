# 星球系统

Planets 模块让模组为每个已加载的 `PlanetFactory` 创建星球级系统。DSPCore 会在工厂创建、星球渲染、工厂 tick 和 sidecar 存档阶段转发生命周期。

## 这个模块带来什么便利

- 不需要每个模组都维护 `planetId -> system` 字典和加载后恢复逻辑。
- 未加载星球的数据会在导入时进入 pending，并在工厂加载后恢复。
- 星球系统适合管理星球范围的缓存、投射状态、建筑扫描索引和跨实体调度。

## 边界

- 星球系统只在 `PlanetFactory` 存在后初始化；未生成工厂的星球不会提前创建系统。
- `DrawUpdate` 只在本地星球的 `FactoryModel.OnCameraPostRender` 之后转发。
- 星球系统不替代恒星/银河级状态；跨星球状态使用 Galaxy 模块。

## 示例

- `Examples/PlanetSystem.md`
- `Examples/PlanetSystemExample.cs`
