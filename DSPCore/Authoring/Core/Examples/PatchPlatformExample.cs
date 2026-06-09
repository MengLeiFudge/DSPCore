using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Patches.Register 声明普通条件补丁。
// - Patches.RegisterForPlugin 声明依赖其他插件的补丁。
// - descriptor 路径仍保留给批量注册或高级配置。
//
// Usage:
// - Use Patches.Register for normal conditional patches.
// - Use Patches.RegisterForPlugin for integration patches that require another plugin.
// - Keep concrete Harmony patch code in your mod runtime or owning DSPCore system.
public static class PatchPlatformExample
{
    private const string Owner = "com.example.my-mod";

    public static void Register()
    {
        Patches.Register(
            id: "example.core-patch",
            ownerModGuid: Owner,
            apply: ApplyCorePatch,
            description: "Apply example runtime patch.",
            isEnabled: IsFeatureEnabled,
            getDisabledReason: GetDisabledReason);

        Patches.RegisterForPlugin(
            id: "example.target-plugin-integration",
            ownerModGuid: Owner,
            requiredPluginGuid: "com.example.target-plugin",
            apply: ApplyTargetPluginIntegration,
            description: "Enable integration when the target plugin is loaded.",
            minimumPluginVersion: "1.2.0");
    }

    private static void ApplyCorePatch()
    {
        // Create or use your own Harmony instance here when the integration needs method patches.
    }

    private static void ApplyTargetPluginIntegration()
    {
    }

    private static bool IsFeatureEnabled()
    {
        return true;
    }

    private static string GetDisabledReason()
    {
        return "example feature is disabled";
    }
}
