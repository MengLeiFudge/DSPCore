# UI 抽象

UI 模块目前只是轻量描述层，用来创建按钮 descriptor 等可复用 UI 元数据。它不是完整 UI 框架，也不会把按钮自动挂到游戏窗口。

## 这个模块带来什么便利

- 功能块可以先共享简单 UI 描述对象，而不是直接互相传 Unity `GameObject`。
- 未来如果多个功能块需要统一按钮、面板或节点工厂，可以在这里扩展公共描述模型。
- 当前实现很小，能避免把具体窗口逻辑提前塞进 Core。

## 功能：创建按钮描述

```csharp
var button = new UiNodeFactory().Button(
    id: "example.copy",
    title: "Copy",
    tooltip: "Copy error text");
```

返回值是 `UiButtonDescriptor`，只包含 `Id`、`Title` 和 `Tooltip`。

## 调用后 DSPCore 会怎么处理

当前没有通用 UI runtime 会消费这些 descriptor。具体窗口行为仍由所属功能块直接适配，例如 Errors 自己处理 fatal window 按钮，Tabs 自己处理分页按钮。

## 这个模块不负责什么

- 不自动创建 Unity `GameObject`。
- 不提供布局、样式、事件绑定或生命周期管理。
- 不承载具体功能 UI；具体行为必须留在所属功能块。
- 不应把 Tabs、Pickers、Errors 等已明确归属的 UI 行为迁入这里。
