# 核心入口

## 职责

本功能块负责 `DspCore` 入口和框架级注册表。

## 公开入口

- `Api/DspCore.cs`
- `Api/Features.cs`：作者侧功能块短入口。
- `Api/Modules.cs`：作者侧模块短入口。
- `Api/FeatureRegistry.cs`
- `Api/ModuleRegistry.cs`
- `Api/PatchRegistry.cs`
- `Api/FeatureDescriptor.cs`
- `Api/ModuleDescriptor.cs`
- `Api/PatchDescriptor.cs`

## 运行时

`../Runtime/Runtime/DSPCorePlugin.cs` 会从 BepInEx 初始化本功能块并应用 Harmony 补丁。

## 边界

Core 只协调功能块，不应持有存档、图标、建造栏、成就等具体功能行为。
