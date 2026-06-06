# 建造栏

BuildBar 模块让模组把已有物品放到快捷建造栏指定位置。它不负责创建物品，而是把你已经创建或拿到的 `ItemProto` / item id 映射到 `tab`、`row`、`index` 槽位，并在游戏 UI 加载时把这些绑定投射到建造栏。

## 这个模块带来什么便利

- 你不需要自己改 `UIBuildMenu.protos` 或复制建造栏按钮。
- 你可以用同一套槽位模型描述原版第 1 行和 DSPCore 扩展行。
- 旧 BuildBarTool / LDBTool 风格的 `SetBuildBar` 调用会桥接到同一套绑定模型，减少多套建造栏 patch 互相覆盖的风险。
- 扩展行按钮会复用原版点击逻辑、图标、物品数量显示和解锁状态检查，不需要每个模组自己重做 UI 行为。

## 功能：绑定已有物品到建造栏

如果你已经有 `ItemProto`，优先用扩展方法：

```csharp
itemProto.SetBuildBar(tab: 3, row: 2, index: 5);
```

如果你只有 item id，可以使用短入口：

```csharp
BuildBar.BindQuickBar(tab: 3, row: 2, index: 5, itemId: 9554);
```

`tab`、`row`、`index` 都从 1 开始。`row = 1` 是原版建造栏行；`row > 1` 是 DSPCore 扩展行。

## 调用后 DSPCore 会怎么处理

- 注册阶段只记录槽位到 item id 的绑定；同一个槽位后一次绑定会覆盖前一次绑定。
- 如果 `tab`、`row`、`index` 小于 1，或 item id 小于等于 0，绑定会被拒绝并返回 false。
- `UIBuildMenu.StaticLoad` 后，`row = 1` 的绑定会写入原版 `UIBuildMenu.protos`，并同步设置物品 `BuildIndex`。
- 建造栏打开和刷新时，`row > 1` 的绑定会创建或刷新 DSPCore 扩展按钮。
- 扩展按钮点击时，DSPCore 会临时把目标物品放入当前分类的原版槽位，并调用原版建造栏点击逻辑。

## 功能：兼容旧 BuildBarTool / LDBTool 入口

旧入口仍保留在 `Compat/` 下：

- `BuildBarTool.BuildBarTool.SetBuildBar(category, index, itemId, isTopRow)`
- `DSPCore.LegacyBuildBarCompatibility.SetBuildBar(category, index, itemId, layer)`
- `xiaoye97.LDBTool.SetBuildBar(category, index, itemId)`

这些入口会映射到 `tab`、`row`、`index`。新代码应直接使用 `ItemProto.SetBuildBar(...)` 或 `BuildBar.BindQuickBar(...)`，旧入口只用于迁移和源码兼容。

## 这个模块不负责什么

- 不创建 `ItemProto`、配方、图标或本地化；这些属于 ProtoRegistration、Icons 和 Resources。
- 不决定物品是否解锁；扩展按钮会按原版历史解锁状态和沙盒即时物品状态设置可点击性。
- 玩家自定义建造栏位置和 RebindBuildBar 兼容仍未实现。
- `row = 1` 受原版 `UIBuildMenu.protos` 尺寸限制；当前运行时会跳过超出原版 tab/index 范围的第 1 行绑定。

## 示例

- `Examples/QuickBarBinding.md`
- `Examples/QuickBarBindingExample.cs`
