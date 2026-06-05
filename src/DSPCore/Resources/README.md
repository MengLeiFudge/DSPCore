# 资源与本地化

## 职责

本功能块注册共享资源和本地化条目。

## 公开入口

- `ResourceDescriptor`
- `ResourceRegistry`
- `LocalizationEntry`

## 运行时

`LocalizationRuntime.cs` 会写入本地化条目。图标 sprite 加载由 `Icons/IconRuntime.cs` 负责。

## 边界

资源注册记录归属和路径。具体功能块决定如何使用这些资源。
