using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 汇总所有模组的成就禁用声明，并给出最终成就策略。
/// Aggregates achievement-disable declarations from all mods and produces the final achievement policy.
/// </summary>
public sealed class AchievementPolicyRegistry
{
    private readonly Dictionary<string, AchievementPolicyDeclaration> declarations = new(StringComparer.Ordinal);

    /// <summary>
    /// 是否允许把成就同步到 Steam、RAIL 或 XGP 平台；默认关闭。
    /// Gets or sets whether achievement sync to Steam, RAIL, or XGP platforms is allowed; disabled by default.
    /// </summary>
    public bool AllowPlatformAchievements { get; set; }

    /// <summary>
    /// 是否允许上传 Milky Way/排行榜数据；默认关闭。
    /// Gets or sets whether Milky Way and leaderboard upload is allowed; disabled by default.
    /// </summary>
    public bool AllowMilkyWayUpload { get; set; }

    /// <summary>
    /// 控制 DSPCore 对成就策略元数据的保留量。
    /// Controls how much achievement-policy metadata DSPCore keeps.
    /// </summary>
    public AchievementMetadataMode MetadataMode { get; set; } = AchievementMetadataMode.DeclarationsOnly;

    /// <summary>
    /// 声明一个模组是否要求禁用成就。
    /// Declares whether a mod requires achievements to be disabled.
    /// </summary>
    /// <param name="declaration">成就策略声明。Achievement policy declaration.</param>
    public void Declare(AchievementPolicyDeclaration declaration)
    {
        if (string.IsNullOrWhiteSpace(declaration.ModGuid))
        {
            throw new ArgumentException("Mod GUID cannot be empty.", nameof(declaration));
        }

        declarations[declaration.ModGuid] = declaration;
    }

    /// <summary>
    /// 获取最终是否应禁用成就。
    /// Gets whether achievements should be disabled in the final aggregated policy.
    /// </summary>
    /// <returns>任意模组声明禁用时返回 true。Returns true when any mod declares achievement disabling.</returns>
    public bool ShouldDisableAchievements()
    {
        return declarations.Values.Any(item => item.DisableAchievements);
    }

    /// <summary>
    /// 获取是否应屏蔽原版异常检查，让本地成就保持可用。
    /// Gets whether vanilla abnormality checks should be blocked so local achievements stay available.
    /// </summary>
    /// <returns>没有模组声明禁用成就时返回 true。Returns true when no mod declares achievement disabling.</returns>
    public bool ShouldBlockAbnormalityChecks()
    {
        return !ShouldDisableAchievements();
    }

    /// <summary>
    /// 获取是否应阻止平台成就同步。
    /// Gets whether platform achievement synchronization should be blocked.
    /// </summary>
    /// <returns>需要阻止平台成就同步时返回 true。Returns true when platform sync should be blocked.</returns>
    public bool ShouldBlockPlatformAchievements()
    {
        return ShouldDisableAchievements() || !AllowPlatformAchievements;
    }

    /// <summary>
    /// 获取是否应阻止 Milky Way 和排行榜上传。
    /// Gets whether Milky Way and leaderboard uploads should be blocked.
    /// </summary>
    /// <returns>需要阻止上传时返回 true。Returns true when uploads should be blocked.</returns>
    public bool ShouldBlockMilkyWayUpload()
    {
        return ShouldDisableAchievements() || !AllowMilkyWayUpload;
    }

    /// <summary>
    /// 获取所有成就策略声明。
    /// Gets all achievement policy declarations.
    /// </summary>
    /// <returns>声明快照。Snapshot of declarations.</returns>
    public IReadOnlyCollection<AchievementPolicyDeclaration> GetDeclarations()
    {
        return declarations.Values;
    }
}
