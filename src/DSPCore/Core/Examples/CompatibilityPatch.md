# CompatibilityPatch

本场景用于登记跨模组、跨版本或游戏版本相关的兼容补丁声明。该声明机制属于 Core 的补丁支撑能力，不是独立功能块。

## 适用时机

- 你的功能需要对某个目标模组或版本范围应用兼容逻辑。
- 需要在错误报告、迁移文档或运行时诊断中说明补丁归属。

## 关键参数

- `Id`：稳定补丁 ID，建议包含功能名和目标。
- `TargetModGuid`：补丁目标模组 GUID。
- `TargetVersionRange`：目标版本范围说明。
- `Reason`：补丁存在原因。
- `Apply`：实际应用补丁的回调。

## 运行时前提

兼容声明应在插件启动阶段注册。具体补丁逻辑仍应委托给所属功能块，不要把所有修复集中到 Core 或一个通用 fixer。

## 常见误用

- 不要用模糊 ID，例如 `fix1` 或 `compat`。
- 不要把无关功能的补丁塞进 Compatibility 注册表的 `Apply` 内部；`Apply` 应转发到所属功能块。

代码示例见 `CompatibilityPatchExample.cs`。
