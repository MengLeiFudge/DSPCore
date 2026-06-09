# PatchPlatform

本场景用于声明由 DSPCore 统一调度的条件补丁。

## 适用时机

- 需要在 DSPCore 启动时应用一段补丁初始化逻辑。
- 需要按配置、运行时条件或目标插件是否加载来决定是否启用补丁。
- 需要把禁用原因或应用失败交给 DSPCore 记录，避免每个模组重复写同类日志。

## 关键参数

- `Patches.Register(id, ownerModGuid, apply, description)`：注册普通补丁声明。
- `Patches.RegisterForPlugin(id, ownerModGuid, requiredPluginGuid, apply, ...)`：注册依赖其他插件的补丁声明。
- `isEnabled`：返回 `false` 时 DSPCore 跳过 `apply`。
- `getDisabledReason`：当 `isEnabled` 返回 `false` 时提供日志说明。
- `minimumPluginVersion`：配合 `RegisterForPlugin` 检查目标插件最低版本。

## 运行时前提

DSPCore 会在 `DSPCorePlugin.Awake()` 装配内置运行时补丁后调用已注册的补丁声明。`apply` 只代表补丁初始化回调；如果需要 Harmony patch，请在回调里使用模组自己的 Harmony 实例或已有运行时代码。

目标插件条件由 BepInEx `Chainloader.PluginInfos` 判断。目标插件未加载或版本不足时，DSPCore 会记录禁用原因并跳过 `apply`。

## 常见误用

- 不要把具体功能运行时类放到 Core；Core 只保存作者声明，具体 patch 逻辑应留在所属系统或模组自己的运行时代码。
- 不要用条件补丁替代 `Lifecycle`、`ProtoRegistration` 或 `Saves`；已有专门生命周期时优先用专门能力。
- 不要在 `apply` 里假设游戏存档、星球工厂或 UI surface 已存在。

代码示例见 `PatchPlatformExample.cs`。
