# 模型和预制体

Models 模块让模组声明从已有 `ModelProto` 克隆出新模型，并在最终 Proto 缓存重建前修改 `ModelProto` 和 `PrefabDesc`。

## 这个模块带来什么便利

- 不需要每个模组都手动扩容 `LDB.models.dataArray`。
- DSPCore 会在 `DataFinalFixes` 后应用模型克隆，并重建 `ModelProto` 派生缓存和 `PlanetFactory.PrefabDescByModelIndex`。
- `PrefabDesc` 会与来源模型分离，避免配置回调直接修改来源模型。

## 边界

- 当前是浅克隆公开字段；mesh、material、prefab 引用仍来自来源模型，除非作者在回调中替换。
- 新 `modelIndex` 必须由作者保证不和其他模组冲突。
- 该模块不负责创建物品或配方；通常配合 Items/Recipes 使用。

## 示例

- `Examples/CloneModel.md`
- `Examples/CloneModelExample.cs`
