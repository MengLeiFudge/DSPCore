# 运行时桥接

## 职责

本功能块把 DSPCore 注册表连接到 BepInEx、Harmony、Unity 和 DSP 运行时对象。

## 公开入口

`DSPCorePlugin` 是 BepInEx 插件入口。其他运行时类是内部适配器。

## 运行时

运行时适配器会 patch 游戏方法、应用队列注册、重建缓存、保存 sidecar 数据，并更新 UI 辅助功能。

## 边界

Runtime 不应定义新的公开功能语义。公开语义属于各功能块注册表。
