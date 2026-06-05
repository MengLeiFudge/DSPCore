using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧成就策略入口。
/// Author-facing achievement policy entry point.
/// </summary>
public static class Achievements
{
    /// <summary>
    /// 是否允许把成就同步到 Steam、RAIL 或 XGP 平台；默认关闭。
    /// Gets or sets whether achievement sync to Steam, RAIL, or XGP platforms is allowed; disabled by default.
    /// </summary>
    public static bool AllowPlatformAchievements
    {
        get => DspCore.Achievements.AllowPlatformAchievements;
        set => DspCore.Achievements.AllowPlatformAchievements = value;
    }

    /// <summary>
    /// 是否允许上传 Milky Way/排行榜数据；默认关闭。
    /// Gets or sets whether Milky Way and leaderboard upload is allowed; disabled by default.
    /// </summary>
    public static bool AllowMilkyWayUpload
    {
        get => DspCore.Achievements.AllowMilkyWayUpload;
        set => DspCore.Achievements.AllowMilkyWayUpload = value;
    }

    /// <summary>
    /// 控制 DSPCore 对成就策略元数据的保留量。
    /// Controls how much achievement-policy metadata DSPCore keeps.
    /// </summary>
    public static AchievementMetadataMode MetadataMode
    {
        get => DspCore.Achievements.MetadataMode;
        set => DspCore.Achievements.MetadataMode = value;
    }

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
