# ResourceRegistration

本场景用于登记模组资源根和本地化文本。

## 适用时机

- 多个功能块需要引用同一组资源路径，且不想在每次调用中重复传 owner 和根路径。
- UI、分页、Proto 名称或提示文本需要由 DSPCore 统一写入本地化表。
- 图标资源和目标 Proto 属于同一个资源包，希望用 `pack.ItemIcon(...)` 这类 typed helper 避免重复写 `ProtoKind`。

## 关键参数

- `id`：资源根稳定 ID。
- `ownerModGuid`：资源或文本归属模组。
- `keyword`：资源分类关键字，例如 `icons` 或 `ui`。
- `rootPath`：资源根路径。
- `ModResources.Pack(...)`：创建复用 owner、资源根和默认 assembly 的资源包短入口。
- `key` / `language` / `value`：本地化 key、语言和文本。
- `ItemIcon` / `RecipeIcon` / `TechIcon` / `TutorialIcon` / `SignalIcon`：把资源包内图标绑定到对应 Proto 类型。

## 运行时前提

本地化条目需要在 DSPCore 本地化桥接应用前注册。资源根只记录元数据，具体加载由 Icons、UI 或其他消费方负责。

## 常见误用

- 不要把 ModResources 当成图标加载器；图标 sprite 加载属于 Icons。
- 不要用 typed icon helper 创建 Proto；目标物品、配方、科技、指引或信号仍必须已经存在。
- 不要把 `rootPath` 当成嵌入资源命名空间；嵌入资源仍需要完整 manifest resource name。
- 不要把临时文件路径作为稳定资源 ID。

代码示例见 `ResourceRegistrationExample.cs`。
