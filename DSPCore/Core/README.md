# 核心入口

## 职责

本功能块负责 `DspCore` 入口、框架级注册表和 BepInEx 启动装配。

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

## 兼容入口

- `Compat/CommonApiShim.cs`：旧命名空间 `CommonAPI` 的模块查询和子模块依赖属性外壳。
- `Compat/IsExternalInit.cs`：net472 下支持 record/init-only 语法所需的编译期 polyfill。

## 运行时

`Runtime/DSPCorePlugin.cs` 会从 BepInEx 初始化本功能块并应用 Harmony 补丁。

## 边界

Core 只协调功能块和启动装配，不应持有存档、图标、建造栏、成就等具体功能行为。旧 API shim 和第三方兼容适配必须放回所属功能块的 `Compat/`。
