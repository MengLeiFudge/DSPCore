# 建造栏

BuildBar 模块让模组把已有物品放到快捷建造栏指定位置。它不负责创建物品，而是把你已经创建或拿到的 `ItemProto` / item id 映射到 `category`、`row`、`index` 槽位，并在游戏 UI 加载时把这些绑定投射到建造栏。

## 这个模块带来什么便利

- 你不需要自己改 `UIBuildMenu.protos` 或复制建造栏按钮。
- 你可以用同一套槽位模型描述原版第 1 行和 DSPCore 扩展行。
- 旧 BuildBarTool / LDBTool 风格的 `SetBuildBar` 调用会桥接到同一套绑定模型，减少多套建造栏 patch 互相覆盖的风险。
- 扩展行按钮会复用原版点击逻辑、图标、物品数量显示和解锁状态检查，不需要每个模组自己重做 UI 行为。
- DSPCore 自有玩家覆盖层可以覆盖作者默认绑定，并随 `.dspcore` 存档保存。
- 玩家按住建造栏重绑键点击槽位可打开原版物品选择器绑定物品，按住清空键悬停槽位可显式清空。

## 功能：绑定已有物品到建造栏

如果你已经有 `ItemProto`，优先用扩展方法：

```csharp
itemProto.SetBuildBar(category: 3, row: 2, index: 5);
```

如果你只有 item id，可以使用短入口：

```csharp
BuildBar.BindQuickBar(category: 3, row: 2, index: 5, itemId: 9554);
```

`category`、`row`、`index` 都从 1 开始。`row = 1` 是原版建造栏行；`row > 1` 是 DSPCore 扩展行。

默认绑定不会静默覆盖已有作者默认绑定。如果你需要知道目标格是否已经被其他声明占用，使用结构化结果入口：

```csharp
BuildBarBindResult result = itemProto.SetBuildBarWithResult(category: 3, row: 2, index: 5);
if (result.Status == BuildBarBindStatus.Occupied)
{
    // ExistingItemId 是当前占用该槽位的作者默认物品。
}
```

需要明确覆盖已有作者默认绑定时，必须显式传入覆盖策略：

```csharp
itemProto.SetBuildBarWithResult(
    category: 3,
    row: 2,
    index: 5,
    conflictPolicy: BuildBarConflictPolicy.ReplaceExisting);
```

`BuildBarBindStatus.Applied` 表示写入空槽位；`AlreadyBound` 表示槽位已经是同一物品；`Occupied` 表示已有其他作者默认绑定且本次没有覆盖；`Replaced` 表示按显式策略覆盖；`Invalid` 表示参数非法。旧的 bool 入口只返回本次请求是否成为当前作者默认绑定。

## 功能：玩家覆盖槽位

DSPCore 复用建造栏本身作为玩家重绑入口：按住 `DSPCore Build Bar Reassign` 对应按键点击槽位会打开原版物品选择器；按住 `DSPCore Build Bar Clear` 对应按键悬停槽位会把该槽位显式清空。这两个按键会进入原版按键页。

如果你自己的 UI 也需要重绑建造栏，可以写入同一套玩家覆盖层：

```csharp
BuildBar.SetPlayerOverride(category: 3, row: 2, index: 5, itemId: 9555);
BuildBar.ClearPlayerOverride(new BuildBarSlot(3, 2, 5));
```

覆盖层优先于作者默认绑定，并通过 DSPCore 自有 `.dspcore` 存档保存。传入 `itemId = 0` 表示玩家显式清空该槽位；调用 `ClearPlayerOverride(...)` 才会删除覆盖并回到作者默认绑定。
`BuildBar.OpenEditor()` 只保留为旧源码兼容入口，不再打开独立窗口。

## 调用后 DSPCore 会怎么处理

- 注册阶段只记录槽位到 item id 的作者默认绑定；同一个槽位已有其他默认绑定时，默认保留已有绑定并返回 `Occupied`，不会静默覆盖。
- 作者需要覆盖已有默认绑定时，必须使用 `BuildBarConflictPolicy.ReplaceExisting`；返回结果的 `ExistingItemId` 会告诉你被覆盖或阻止覆盖的旧物品。
- 如果 `category`、`row`、`index` 小于 1，或 item id 小于等于 0，绑定会被拒绝并返回 false。
- `UIBuildMenu.StaticLoad` 后，`row = 1` 的绑定会写入原版 `UIBuildMenu.protos`，并同步设置物品 `BuildIndex`。
- 建造栏打开和刷新时，`row > 1` 的绑定会创建或刷新 DSPCore 扩展按钮。
- 扩展按钮点击时，DSPCore 会临时把目标物品放入当前分类的原版槽位，并调用原版建造栏点击逻辑。
- 运行时读取作者默认绑定叠加玩家覆盖后的有效绑定；玩家覆盖会随 DSPCore 存档导入和导出。
- 玩家通过建造栏热键交互改动槽位后，DSPCore 会立即刷新当前建造栏投射。
- 如果当前存档还没有 DSPCore BuildBar 数据，DSPCore 会读取 RebindBuildBar 的 `RebindBuildBar/CustomBarBind.cfg`，把 `[BuildBarBinds]` 中的原版第 1 行配置导入玩家覆盖层。

## 功能：兼容旧 BuildBarTool / LDBTool 入口

旧入口仍保留在 `Compat/` 下：

- `BuildBarTool.BuildBarTool.SetBuildBar(category, index, itemId, isTopRow)`
- `DSPCore.LegacyBuildBarCompatibility.SetBuildBar(category, index, itemId, layer)`
- `xiaoye97.LDBTool.SetBuildBar(category, index, itemId)`

这些入口会映射到 `category`、`row`、`index`。新代码应直接使用 `ItemProto.SetBuildBar(...)` 或 `BuildBar.BindQuickBar(...)`，旧入口只用于迁移和源码兼容。

## 这个模块不负责什么

- 不创建 `ItemProto`、配方、图标或本地化；这些属于 ProtoRegistration、Icons 和 Resources。
- 不决定物品是否解锁；扩展按钮会按原版历史解锁状态和沙盒即时物品状态设置可点击性。
- 不接管 RebindBuildBar 的重绑 UI、快捷键或后续配置写回；DSPCore 只把已有 `CustomBarBind.cfg` 配置导入自己的玩家覆盖层。后续玩家改动由 DSPCore 的建造栏热键交互写入 `.dspcore` 存档。
- `row = 1` 受原版 `UIBuildMenu.protos` 尺寸限制；当前运行时会跳过超出原版 tab/index 范围的第 1 行绑定。

## 示例

- `Examples/QuickBarBinding.md`
- `Examples/QuickBarBindingExample.cs`
