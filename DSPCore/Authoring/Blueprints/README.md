# 蓝图参数

Blueprints 模块为建筑复制粘贴、蓝图和预建筑落地提供 tagged 参数块。模组不需要抢原版 `int[] parameters` 的固定槽位，而是注册稳定 `BlockId`，由 DSPCore 编码到参数数组尾部并在粘贴或预建筑落地时回调。

## 这个模块带来什么便利

- 多个模组可以共享同一个 `BuildingParameters.parameters` 数组而不互相覆盖。
- 复制、蓝图导出、蓝图导入、粘贴和预建筑落地走同一个 block ID。
- `CanPaste` 可以阻止不兼容的参数块粘贴。

## 边界

- 参数块负载当前是 `int[]`；复杂二进制数据应由模组自行编码为整数。
- `BlockId` 必须长期稳定。
- DSPCore 只管理参数块的搬运和分发，不理解具体业务语义。

## 示例

- `Examples/BuildingParameters.md`
- `Examples/BuildingParametersExample.cs`
