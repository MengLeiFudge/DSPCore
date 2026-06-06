# TabRegistration

本场景用于声明作者可见的自定义页面，并取得可用于 `GridIndex` 的 `TabSlot`。

## 适用时机

- 模组需要新增一个页面，再把物品或配方放到该页面的格子里。
- 模组只需要声明页面；DSPCore 负责为它分配稳定 `TabSlot` 并投射到已支持界面。

## 关键参数

- `Id`：分页稳定 ID。
- `OwnerModGuid`：归属模组。
- `Title`：分页标题。
- `IconId`：图标 ID。
- `Order`：按钮显示排序权重。
- `TabSlot`：`Tabs.AddTab(...)` 返回的页面槽位。
- `GridIndex`：`ItemProto` / `RecipeProto` 自己的格子字段，可用 `ProtoRegistration.GetGridIndex(tabSlot, row, index)` 生成。

## 运行时前提

在启动阶段注册一次。当前已支持的投射面有限；全息信标、蓝图、信号选择器等界面需要更完整内容模型后再支持。

## 常见误用

- 不要把 `TabSlot` 和 `GridIndex` 混成同一个概念：`TabSlot` 是页面槽位，`GridIndex` 是物品/配方格子。
- 不要把分页内容和复杂 UI 状态塞进 descriptor。
- 不要假设所有 DSP UI 表面都已支持自定义分页。

代码示例见 `TabRegistrationExample.cs`。
