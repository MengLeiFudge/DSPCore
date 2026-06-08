using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - 模组作者只需要 using DSPCore，即可使用 Achievements 这个短入口。
// - 每个模组只声明自己的 GUID 和是否禁用成就，DSPCore 会汇总所有声明。
// - 只要任意模组声明 DisableAchievements = true，最终策略就是禁用成就。
//
// 建议调用位置：
// - 在你的 BepInEx 插件 Awake 或 CommonAPI/DSPCore 注册阶段调用 Register。
// - 不要在每帧 Update 中重复声明；同一个 ModGuid 的声明会被后一次覆盖。
//
// Usage:
// - Add using DSPCore, then call the short Achievements entry point.
// - Declare only your own mod GUID and achievement-disable flag. DSPCore aggregates all declarations.
// - Call this once during plugin startup or feature registration.
public static class AchievementPolicyExample
{
    public static void Register()
    {
        // ModGuid 必须稳定，建议使用你的 BepInEx GUID。
        // Use a stable mod GUID, normally your BepInEx plugin GUID.
        Achievements.Declare("com.example.balance", disableAchievements: true);
    }
}
