# 建造栏

## 职责

本功能块只负责把物品 ID 或 `ItemProto` 绑定到快捷建造栏槽位。

## 槽位模型

新标准槽位是 `tab`、`row`、`index`。

## 公开入口

- `Api/ItemProtoQuickBarExtensions.cs`：`ItemProto.BindQuickBar(...)` 首选作者写法。
- `Api/BuildBar.cs`：只有物品 ID 时使用的短入口。
- `Api/BuildBarRegistry.cs`：槽位绑定注册表。
- `Api/BuildBarSlot.cs`

## 兼容入口

- `Compat/BuildBarTier.cs`：旧 BuildBarTool 层级语义。
- `Compat/LegacyBuildBarCompatibility.cs`：旧 `SetBuildBar(category,index,itemId,layer/tier)` 到 `tab/row/index` 的桥接。
- `Compat/BuildBarToolShim.cs`：旧命名空间 `BuildBarTool.BuildBarTool` 外壳。

## 示例

- `Examples/QuickBarBinding.md`
- `Examples/QuickBarBindingExample.cs`

## 运行时

`Runtime/BuildBarRuntime.cs` 会把第 1 行写入原版 `UIBuildMenu.protos`，并为第 2 行及以后创建 DSPCore 扩展按钮。

## 边界

物品创建属于原型/物品功能。那些功能可以在创建或修改物品后调用 `ItemProto.BindQuickBar(...)`。旧 `SetBuildBar` 只允许放在 `Compat/`，用于兼容 BuildBarTool 和 LDBTool 旧入口。
