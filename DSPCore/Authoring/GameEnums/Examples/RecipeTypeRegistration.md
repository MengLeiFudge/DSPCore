# RecipeTypeRegistration

本场景用于声明自定义配方类型及其允许使用的机器。
同一能力也提供 `ItemProto.SetCustomItemType()`，用于把已有物品标记为 DSPCore 预留的自定义物品类型。

## 适用时机

- 配方 Proto 已存在，但需要把一组配方归入自定义类型。
- 需要限制哪些机器或制作器可以使用这些配方。

## 关键参数

- `Id`：配方类型稳定 ID。
- `OwnerModGuid`：归属模组。
- `DisplayName`：显示名。
- `RecipeIds`：属于该类型的配方 ID。
- `AssemblerItemIds`：允许使用这些配方的机器物品 ID。
- `GameEnums.RegisterRecipeType(...)`：新示例主推入口；`RecipeTypes.Register(...)` 仍保留为旧短入口。
- `ItemProto.SetCustomItemType()`：已有物品原型时的短入口；只改 `ItemProto.Type`，不创建物品或 UI 分类。

## 运行时前提

配方 ID 和机器物品 ID 应先稳定存在。GameEnums 不创建配方；配方创建属于 ProtoRegistration。
物品 Proto 应先存在，再在 `DataUpdates` 或 `DataFinalFixes` 中标记自定义物品类型。

## 常见误用

- 不要把 RecipeProto 创建逻辑放到 GameEnums。
- 不要把 ItemProto 创建逻辑放到 GameEnums。
- 不要在版本更新中随意改变已有 recipe id，除非同时处理存档迁移。

代码示例见 `RecipeTypeRegistrationExample.cs`。
