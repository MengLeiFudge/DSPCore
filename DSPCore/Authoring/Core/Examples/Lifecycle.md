# Lifecycle

本场景用于注册 DSPCore 框架生命周期回调。

## 适用时机

- 需要在 DSPCore 运行时桥接装配完成后执行少量初始化。
- 需要每帧轮询一小段框架状态。
- 需要在 DSPCore 插件销毁时释放本模组挂到 DSPCore 的临时状态。
- 需要在新游戏、保存前、读取前、读取后或删除存档后刷新非持久缓存。

## 关键参数

- `OnStarted(Action)`：DSPCore 启动完成后调用；如果已经启动，会立即调用。
- `OnUpdate(Action)`：DSPCore 插件每帧调用。
- `OnDestroyed(Action)`：DSPCore 插件销毁时调用。
- `OnNewGame(Action)`：新游戏开始后调用。
- `OnBeforeSave(Action<string>)`：保存当前游戏前调用，参数是存档名。
- `OnBeforeLoad(Action<string>)`：读取当前游戏前调用，参数是存档名。
- `OnAfterLoad(Action)`：读取当前游戏并完成 DSPCore postload 存档导入后调用。
- `OnSaveDeleted(Action<string>)`：删除存档后调用，参数是存档名。

## 运行时前提

`OnStarted`、`OnUpdate` 和 `OnDestroyed` 只代表 DSPCore 框架生命周期。不要在 `OnStarted` 中假设游戏存档、星球工厂或具体 UI surface 已存在。

存档链路事件来自 DSPCore 已经接入的 `GameData` / `GameSave` / 删除存档 patch，适合刷新缓存或非持久状态。需要读写持久状态时仍应使用 `Saves`。

## 常见误用

- 不要把重型每帧逻辑塞进 `OnUpdate`；需要游戏对象生命周期时优先使用对应 Systems/Authoring 能力。
- 不要用 `OnStarted` 代替 ProtoRegistration 的数据阶段。
- 不要用存档链路事件手写旁路文件；这类持久化应交给 `Saves`。

代码示例见 `LifecycleExample.cs`。
