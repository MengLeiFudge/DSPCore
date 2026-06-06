# RecipeTypeRegistration

本场景用于声明自定义配方类型及其允许使用的机器。

## 适用时机

- 配方 Proto 已存在，但需要把一组配方归入自定义类型。
- 需要限制哪些机器或制作器可以使用这些配方。

## 关键参数

- `Id`：配方类型稳定 ID。
- `OwnerModGuid`：归属模组。
- `DisplayName`：显示名。
- `RecipeIds`：属于该类型的配方 ID。
- `AssemblerItemIds`：允许使用这些配方的机器物品 ID。

## 运行时前提

配方 ID 和机器物品 ID 应先稳定存在。RecipeTypes 不创建配方；配方创建属于 ProtoRegistration。

## 常见误用

- 不要把 RecipeProto 创建逻辑放到 RecipeTypes。
- 不要在版本更新中随意改变已有 recipe id，除非同时处理存档迁移。

代码示例见 `RecipeTypeRegistrationExample.cs`。
