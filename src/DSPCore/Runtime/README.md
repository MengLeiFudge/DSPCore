# 运行时宿主

## 职责

本目录只保留 BepInEx 插件入口和跨功能运行时装配。

## 公开入口

`DSPCorePlugin` 是 BepInEx 插件入口。

## 运行时

`DSPCorePlugin` 会初始化 `DspCore`，注册旧 DSPModSave 处理器，应用各功能块自己的 Harmony patch，并在 Unity `Update` 中驱动需要逐帧轮询的功能块。

## 边界

Runtime 不定义公开功能语义，也不承载具体功能运行时实现。具体桥接代码放在对应功能块目录，例如 `BuildBar/BuildBarRuntime.cs`、`Protos/ProtoRuntime.cs` 和 `Saves/SaveRuntime.cs`。
