using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧成就策略入口。
/// Author-facing achievement policy entry point.
/// </summary>
public static class Achievements
{
    /// <summary>
    /// 声明一个模组是否要求禁用成就。
    /// Declares whether a mod requires achievements to be disabled.
    /// </summary>
    /// <param name="declaration">成就策略声明。Achievement policy declaration.</param>
    public static void Declare(AchievementPolicyDeclaration declaration)
    {
        DspCore.Achievements.Declare(declaration);
    }

    /// <summary>
    /// 声明一个模组是否要求禁用成就。
    /// Declares whether a mod requires achievements to be disabled.
    /// </summary>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <param name="disableAchievements">是否禁用成就。Whether achievements should be disabled.</param>
    public static void Declare(string modGuid, bool disableAchievements)
    {
        DspCore.Achievements.Declare(modGuid, disableAchievements);
    }

    /// <summary>
    /// 获取最终是否应禁用成就。
    /// Gets whether achievements should be disabled in the final aggregated policy.
    /// </summary>
    /// <returns>任意模组声明禁用时返回 true。Returns true when any mod declares achievement disabling.</returns>
    public static bool ShouldDisableAchievements()
    {
        return DspCore.Achievements.ShouldDisableAchievements();
    }

    /// <summary>
    /// 获取所有成就策略声明。
    /// Gets all achievement policy declarations.
    /// </summary>
    /// <returns>声明快照。Snapshot of declarations.</returns>
    public static IReadOnlyCollection<AchievementPolicyDeclaration> GetDeclarations()
    {
        return DspCore.Achievements.GetDeclarations();
    }
}
