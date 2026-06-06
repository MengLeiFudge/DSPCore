# 配方类型

RecipeTypes 模块让模组把已有配方归入自定义配方类型，并声明哪些机器允许使用这些配方。它不是配方创建工具，而是配方创建后的分类和运行时保护层。

## 这个模块带来什么便利

- 你不需要每个模组自己 patch 制作器选配方逻辑。
- 一组自定义配方可以集中声明类型 ID、显示名、配方 ID 和允许机器。
- DSPCore 会在 Proto 缓存重建后把目标配方标记为 `ERecipeType.Custom`。
- 运行时会阻止不支持的制作器设置这些配方，减少“配方出现在不该使用的机器里”的问题。

## 功能：声明自定义配方类型

```csharp
RecipeTypes.Register(new RecipeTypeDescriptor(
    Id: "example-smelting",
    OwnerModGuid: "com.example.my-mod",
    DisplayName: "ExampleSmelting",
    RecipeIds: new[] { 9554001, 9554002 },
    AssemblerItemIds: new[] { 2303 }));
```

`RecipeIds` 指属于该类型的已有配方。`AssemblerItemIds` 为空或 null 时，当前运行时不会限制机器；非空时，只有这些机器物品 ID 对应的制作器可以使用该类型配方。

## 调用后 DSPCore 会怎么处理

- 注册阶段按 `Id` 保存 descriptor；同一个 `Id` 后一次声明会覆盖前一次。
- 派生缓存重建时，DSPCore 会给每个类型分配运行时 ID，并查找 `RecipeIds` 指向的配方。
- 找到的配方会被设置为 `ERecipeType.Custom`，并建立 recipe id 到 descriptor 的映射。
- `AssemblerComponent.SetRecipe` 被调用时，如果目标 recipe 属于自定义类型，DSPCore 会检查当前制作器实体的 `protoId` 是否在 `AssemblerItemIds` 中。
- 不允许的机器会被阻止设置该配方。

## 这个模块不负责什么

- 不创建 `RecipeProto`；配方创建属于 ProtoRegistration。
- 不在配方选择列表打开前隐藏无效配方；当前保护发生在 `SetRecipe` 阶段。
- 不处理 recipe id 变更带来的存档迁移；稳定 ID 仍由模组作者负责。
- 不定义完整 UI 分类；显示和分页应结合 Tabs、Pickers 或具体 UI 功能处理。

## 示例

- `Examples/RecipeTypeRegistration.md`
- `Examples/RecipeTypeRegistrationExample.cs`
