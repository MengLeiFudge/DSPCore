# Lifecycle

本场景用于注册 DSPCore 框架生命周期回调。

## 适用时机

- 需要在 DSPCore 运行时桥接装配完成后执行少量初始化。
- 需要每帧轮询一小段框架状态。
- 需要在 DSPCore 插件销毁时释放本模组挂到 DSPCore 的临时状态。

## 关键参数

- `OnStarted(Action)`：DSPCore 启动完成后调用；如果已经启动，会立即调用。
- `OnUpdate(Action)`：DSPCore 插件每帧调用。
- `OnDestroyed(Action)`：DSPCore 插件销毁时调用。

## 运行时前提

这些事件只代表 DSPCore 框架生命周期。不要在 `OnStarted` 中假设游戏存档、星球工厂或具体 UI surface 已存在。

## 常见误用

- 不要把重型每帧逻辑塞进 `OnUpdate`；需要游戏对象生命周期时优先使用对应 Systems/Authoring 能力。
- 不要用 `OnStarted` 代替 ProtoRegistration 的数据阶段。

代码示例见 `LifecycleExample.cs`。
