# 游戏枚举扩展

GameEnums 模块承接“扩展原版游戏枚举”的作者能力，例如新的配方类型、新的物品类型等。当前已实现自定义配方类型运行时限制、自定义物品类型声明，以及 DSPCore 预留 enum 槽的运行时映射。

## 这个模块带来什么便利

- 你不需要每个模组自己 patch 制作器选配方逻辑。
- 一组自定义配方可以集中声明类型 ID、显示名、配方 ID 和允许机器。
- 一组自定义物品可以集中声明类型 ID、显示名和物品 ID。
- DSPCore 会在 Proto 缓存重建后把目标配方标记为 `ERecipeType.Custom`。
- DSPCore Preloader 会在缺失时注入 `ERecipeType.Custom = 20` 和 `EItemType.Custom = 100`，运行时代码用 `GameEnums.CustomRecipeTypeValue` / `CustomItemTypeValue` 避免直接编译期依赖注入字段。
- 物品可以通过 `GameEnums.RegisterItemType(...)` 批量声明类型，也可以用 `ItemProto.SetCustomItemType()` 对单个已有物品直接打 marker。
- 制作器打开或刷新配方选择器时，DSPCore 会隐藏当前机器不能使用的自定义配方。
- `SetRecipe` 阶段仍保留最终保护，减少外部 patch 或非标准 UI 绕过选择器过滤后的错误设置。

## 当前能力：声明自定义配方类型

```csharp
GameEnums.RegisterRecipeType(
    id: "example-smelting",
    ownerModGuid: "com.example.my-mod",
    displayName: "ExampleSmelting",
    recipeIds: new[] { 9554001, 9554002 },
    assemblerItemIds: new[] { 2303 });
```

`RecipeIds` 指属于该类型的已有配方。`AssemblerItemIds` 为空或 null 时，当前运行时不会限制机器；非空时，只有这些机器物品 ID 对应的制作器可以使用该类型配方。

已有 `RecipeTypes.Register(...)` 继续保留为旧短入口，也支持同一组参数重载并转发到 `GameEnums.RegisterRecipeType(...)`。新文档和示例仍主推 `GameEnums.RegisterRecipeType(...)`，避免把 GameEnums 能力继续收窄成单一 RecipeTypes 概念。

## 当前能力：标记自定义物品类型

```csharp
GameEnums.RegisterItemType(
    id: "example-items",
    ownerModGuid: "com.example.my-mod",
    displayName: "Example Items",
    itemIds: new[] { 9554001, 9554002 });
```

`ItemIds` 指属于该类型的已有物品。派生缓存重建时，DSPCore 会把这些物品的 `ItemProto.Type` 设置为 `(EItemType)GameEnums.CustomItemTypeValue`，并保留 item id 到 descriptor 的查询映射。这个 stable id / runtime id 是 DSPCore 自己的作者层类型，不是 CLR enum literal 名称。

也可以在已经拿到 `ItemProto` 时直接标记：

```csharp
itemProto.SetCustomItemType();
bool isCustom = itemProto.IsCustomItemType();
```

直接 marker 只设置 `ItemProto.Type`，不会自动登记 item id -> descriptor。

## 调用后 DSPCore 会怎么处理

- 注册阶段按 `Id` 保存 descriptor；同一个 `Id` 后一次声明会覆盖前一次。
- 派生缓存重建时，DSPCore 会给每个配方类型和物品类型分配运行时 ID，并查找 `RecipeIds` / `ItemIds` 指向的原型。
- 找到的配方会被设置为 `ERecipeType.Custom`，并建立 recipe id 到 descriptor 的映射。
- 找到的物品会被设置为 `EItemType.Custom` 预留值，并建立 item id 到 descriptor 的映射。
- `ItemProto.SetCustomItemType()` 会直接修改目标物品的 `Type` 字段，适合在 `DataUpdates` 或 `DataFinalFixes` 中对已存在物品做临时或手动 marker。
- `GameEnums.CanAssemblerUseRecipe(assemblerEntityId, recipeId)` 可复用同一套限制判断。
- `GameEnums.TryGetRecipeTypeForRecipe(...)` 和 `TryGetItemTypeForItem(...)` 可查询运行时映射。
- 制作器窗口打开或刷新配方选择器时，DSPCore 会记录当前制作器实体，并在 `UIRecipePicker.RefreshIcons` 中隐藏该机器不允许使用的自定义配方。
- `AssemblerComponent.SetRecipe` 被调用时，如果目标 recipe 属于自定义类型，DSPCore 仍会检查当前制作器实体的 `protoId` 是否在 `AssemblerItemIds` 中。
- 不允许的机器会在选择列表里被隐藏；如果外部 UI 或其他 patch 绕过了选择列表，也会在设置阶段被阻止。

## 这个模块不负责什么

- 不创建 `RecipeProto`；配方创建属于 ProtoRegistration。
- 不创建 `ItemProto`；物品创建属于 ProtoRegistration / Items。
- 不处理 recipe id 变更带来的存档迁移；稳定 ID 仍由模组作者负责。
- 自定义物品类型提供 stable descriptor、runtime id、预留 enum 槽映射和查询，不提供完整物品分类 UI 或过滤逻辑。
- 不在运行时创建任意新的 CLR enum literal；新增 literal 只能由 Preloader 在游戏程序集加载前注入固定预留值。
- 不定义完整 UI 分类；显示和分页应结合 Tabs 或具体 UI 功能处理。

## 示例

- `Examples/RecipeTypeRegistration.md`
- `Examples/RecipeTypeRegistrationExample.cs`
