using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧成就策略入口。
/// Author-facing achievement policy entry point.
/// </summary>
public static class Achievements
{
    /// <summary>
    /// 声明一个模组是否要求阻止竞争性上传。
    /// Declares whether a mod requests competitive uploads to be blocked.
    /// </summary>
    /// <param name="declaration">成就策略声明。Achievement policy declaration.</param>
    public static void Declare(AchievementPolicyDeclaration declaration)
    {
        DspCore.Achievements.Declare(declaration);
    }

    /// <summary>
    /// 声明一个模组是否要求阻止竞争性上传。
    /// Declares whether a mod requests competitive uploads to be blocked.
    /// </summary>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <param name="disableAchievements">兼容参数：是否阻止竞争性上传。Compatibility parameter: whether competitive uploads should be blocked.</param>
    public static void Declare(string modGuid, bool disableAchievements)
    {
        DspCore.Achievements.Declare(modGuid, disableAchievements);
    }

    /// <summary>
    /// 声明一个模组是否要求阻止 Milky Way 和排行榜上传。
    /// Declares whether a mod requests Milky Way and leaderboard uploads to be blocked.
    /// </summary>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <param name="blockUpload">是否阻止上传。Whether uploads should be blocked.</param>
    public static void BlockCompetitiveUpload(string modGuid, bool blockUpload = true)
    {
        DspCore.Achievements.BlockCompetitiveUpload(modGuid, blockUpload);
    }

    /// <summary>
    /// 获取最终是否应阻止 Milky Way 和排行榜上传。
    /// Gets whether Milky Way and leaderboard uploads should be blocked in the final aggregated policy.
    /// </summary>
    /// <returns>任意模组声明阻止上传时返回 true。Returns true when any mod declares upload blocking.</returns>
    public static bool ShouldBlockCompetitiveUpload()
    {
        return DspCore.Achievements.ShouldBlockCompetitiveUpload();
    }

    /// <summary>
    /// 获取兼容旧入口的阻断状态。
    /// Gets the compatibility blocking state for the old entry point.
    /// </summary>
    /// <returns>兼容返回：任意模组声明阻止上传时返回 true。Compatibility return: true when any mod declares upload blocking.</returns>
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
