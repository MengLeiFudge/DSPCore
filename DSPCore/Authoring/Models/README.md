# 模型和预制体

Models 模块让模组声明从已有 `ModelProto` 克隆出新模型，并在最终 Proto 缓存重建前修改 `ModelProto` 和 `PrefabDesc`。

## 这个模块带来什么便利

- 不需要每个模组都手动扩容 `LDB.models.dataArray`。
- DSPCore 会在 `DataFinalFixes` 后应用模型克隆，并重建 `ModelProto` 派生缓存和 `PlanetFactory.PrefabDescByModelIndex`。
- `PrefabDesc` 会与来源模型分离，避免配置回调直接修改来源模型。
- 调用方已经持有 `ModelProto` 时，可以直接使用 `sourceModel.CloneAsModel(...)`；只有手上只有 model index 时再使用 `Models.CloneModel(...)`。

## 功能：声明模型克隆

```csharp
sourceModel.CloneAsModel(
    modelIndex: 9554,
    ownerModGuid: "com.example.my-mod",
    configureModel: static model => model.Name = "Example Cloned Model",
    configurePrefab: static prefab => prefab.modelIndex = 9554);
```

`CloneAsModel(...)` 使用当前 `ModelProto.ID` 作为来源模型索引。运行时会从 LDB 重新读取来源模型并创建 clone，因此这个调用只登记声明，不会立刻返回新模型实例。

如果只有来源模型索引，可以使用：

```csharp
Models.CloneModel(
    sourceModelIndex: 230,
    modelIndex: 9554,
    ownerModGuid: "com.example.my-mod");
```

`Models.Register(new ModelDescriptor(...))` 保留为配置驱动或批量构造的高级路径。

## 边界

- 当前是浅克隆公开字段；mesh、material、prefab 引用仍来自来源模型，除非作者在回调中替换。
- 新 `modelIndex` 必须由作者保证不和其他模组冲突。
- 该模块不负责创建物品或配方；通常配合 Items/Recipes 使用。

## 示例

- `Examples/CloneModel.md`
- `Examples/CloneModelExample.cs`
