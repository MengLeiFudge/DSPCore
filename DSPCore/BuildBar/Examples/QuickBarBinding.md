# QuickBarBinding

本场景用于把已有物品绑定到快捷建造栏槽位。

## 适用时机

- 你的功能已经创建或拿到了 `ItemProto`。
- 只需要声明物品在建造栏的 `tab`、`row`、`index` 位置。

## 关键参数

- `tab`：建造栏分类，从 1 开始。
- `row`：建造栏行，从 1 开始；`row = 1` 是原版行，`row > 1` 是 DSPCore 扩展行。
- `index`：按钮位置，从 1 开始。
- `itemId` 或 `ItemProto`：要绑定的物品。

玩家覆盖接口使用同一套 `tab`、`row`、`index` 槽位模型。`itemId = 0` 或 `ClearPlayerOverride(...)` 会清除覆盖，让槽位回到作者默认绑定。

## 运行时前提

物品 Proto 应先由 Proto 功能块注册。BuildBar 只负责槽位绑定，不创建物品、图标、配方或本地化。

玩家覆盖层是 DSPCore 自有 `.dspcore` 存档数据；它不会读取 RebindBuildBar 的外部配置。

## 常见误用

- 不要把 Proto 创建逻辑放进 BuildBar。
- 调用方已经持有 `ItemProto` 时，优先使用 `ItemProto.SetBuildBar(...)`。
- 不要把 `SetPlayerOverride(...)` 当作作者默认布局；它用于玩家从 UI 重绑槽位后的覆盖数据。

代码示例见 `QuickBarBindingExample.cs`。
