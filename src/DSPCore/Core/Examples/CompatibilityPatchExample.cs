using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Compatibility 是 Core 下的补丁声明入口，用于记录跨模组、跨版本或游戏版本相关的兼容补丁。
// - 兼容补丁本身仍应归属于具体功能，例如 Tutorial、BuildBar、Save 或 UI。
// - 不要把无关修复都塞进一个通用 fixer 名称里；声明 ID 应能看出目标和原因。
//
// Usage:
// - Register a descriptor once during startup.
// - Keep Apply small and delegate feature-specific work to the owning feature.
public static class CompatibilityPatchExample
{
    public static void Register()
    {
        // Id 应稳定且全局唯一，建议包含功能名和目标。
        // Use a stable id that names the feature and target.
        Compatibility.Register(new CompatibilityPatchDescriptor(
            Id: "tutorial-fractionation-compat",
            TargetModGuid: "com.menglei.dsp.fe",
            TargetVersionRange: ">=2.3.0",
            Reason: "Tutorial flow needs a shared compatibility patch",
            Apply: ApplyTutorialPatch));
    }

    private static void ApplyTutorialPatch()
    {
        // 在这里调用 Tutorial 功能块或你的兼容修复逻辑。
        // Call the Tutorial feature block or your compatibility fix here.
    }
}
