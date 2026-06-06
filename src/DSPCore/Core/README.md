# 核心入口

## 职责

本功能块负责 `DspCore` 入口和框架级注册表。

兼容补丁声明属于 Core 的框架级补丁支撑机制，不是独立功能块。具体 tutorial、build bar、save、UI 等兼容逻辑仍归所属功能块实现。

## 公开入口

- `Api/DspCore.cs`
- `Api/Features.cs`：作者侧功能块短入口。
- `Api/Modules.cs`：作者侧模块短入口。
- `Api/FeatureRegistry.cs`
- `Api/ModuleRegistry.cs`
- `Api/PatchRegistry.cs`
- `Api/Compatibility.cs`：作者侧兼容补丁声明短入口。
- `Api/CompatibilityPatchRegistry.cs`
- `Api/FeatureDescriptor.cs`
- `Api/ModuleDescriptor.cs`
- `Api/PatchDescriptor.cs`
- `Api/CompatibilityPatchDescriptor.cs`

## 示例

- `Examples/CompatibilityPatchExample.cs`
- `Examples/CompatibilityPatch.md`

## 运行时

`../Runtime/Runtime/DSPCorePlugin.cs` 会从 BepInEx 初始化本功能块并应用 Harmony 补丁。

## 边界

Core 只协调功能块，不应持有存档、图标、建造栏、成就等具体功能行为。兼容补丁注册表只记录声明和报告元数据，不承接具体修复实现。
