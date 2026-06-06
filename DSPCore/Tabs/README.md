# 分页

## 职责

本功能块声明物品、配方和制造器界面的自定义 UI 分页。

## 公开入口

- `Api/Tabs.cs`：作者侧短入口。
- `Api/CoreTabDescriptor.cs`
- `Api/TabRegistry.cs`

## 示例

- `Examples/TabRegistration.md`
- `Examples/TabRegistrationExample.cs`

## 运行时

`Runtime/TabRuntime.cs` 会克隆现有分类按钮，并通过原版 GridIndex 分类流程处理自定义分页点击。

## 边界

信号选择器、全息信标、蓝图等界面需要更完整的分页内容模型后才能正确支持。
