# 建造栏

## 职责

本功能块只负责把物品 ID 或 `ItemProto` 绑定到快捷建造栏槽位。

## 槽位模型

新标准槽位是 `tab`、`row`、`index`。

## 公开入口

- `ItemProto.BindQuickBar(tab, row, index)`：首选作者写法。
- `ItemProto.BindQuickBar(buildIndex, row)`：迁移 BuildIndex 风格代码时使用。
- `BuildBar.BindQuickBar(tab, row, index, itemId)`：只有物品 ID 时使用。
- `BuildBarRegistry.BindQuickBar(tab, row, index, itemId)`
- `BuildBarSlot`
- `BuildBarTier` 用于 obsolete 兼容调用。

## 示例

- `Examples/BuildBarExample.cs`

## 运行时

`BuildBarRuntime.cs` 会把第 1 行写入原版 `UIBuildMenu.protos`，并为第 2 行及以后创建 DSPCore 扩展按钮。

## 边界

物品创建属于原型/物品功能。那些功能可以在创建或修改物品后调用 `ItemProto.BindQuickBar(...)`。旧 `SetBuildBar` 只用于兼容 BuildBarTool 和 LDBTool 旧入口。
