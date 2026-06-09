# ModuleDeclaration

本场景用于声明可被其他模组查询的功能块或模块元数据。

## 适用时机

- 模组需要向其他模组暴露“我已经启用某个能力”。
- 模组内部有可选模块，需要让兼容代码或其他模组查询是否存在。
- 需要在 DSPCore 初始化时按功能块优先级执行少量注册逻辑。

## 关键参数

- `Features.Register(id, displayName, initialize, priority, dependencies)`：声明功能块级能力。
- `Modules.Register(id, displayName, initialize, dependencies)`：声明模组内部或跨模组可查询模块。
- `id`：稳定 ID，建议带模组前缀。
- `displayName`：人类可读名称，用于诊断和兼容报告。
- `initialize`：初始化回调。
- `dependencies`：描述性依赖 ID；当前不会做依赖拓扑排序。

## 运行时前提

在启动阶段注册一次。DSPCore 初始化时会按 priority 和 ID 初始化 feature；module 当前按 registry 枚举顺序初始化，不会自动解析依赖图。

## 常见误用

- 不要把 `Features` / `Modules` 当成大型服务容器；它们主要是声明和查询入口。
- 不要依赖 `dependencies` 自动决定初始化顺序；需要严格顺序时自己在初始化逻辑里处理。
- 不要用模块声明替代具体能力 API；物品、分页、存档、按键等仍应使用对应 Authoring 能力。

代码示例见 `ModuleDeclarationExample.cs`。
