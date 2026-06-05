# 配方类型

## 职责

本功能块声明自定义配方类型，以及允许使用这些配方的机器。

## 公开入口

- `RecipeTypes`：作者侧短入口。
- `RecipeTypeDescriptor`
- `RecipeTypeRegistry`

## 示例

- `Examples/RecipeTypeExample.cs`

## 运行时

`RecipeTypeRuntime.cs` 会把声明配方标记为自定义类型，并阻止不支持的制作器选择这些配方。

## 边界

本功能块不创建配方。配方创建属于 Proto 注册；本功能块只分类并保护已有配方 ID。
