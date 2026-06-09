using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 汇总所有模组的成就上传策略声明，并给出最终成就策略。
/// Aggregates achievement upload-policy declarations from all mods and produces the final achievement policy.
/// </summary>
public sealed class AchievementPolicyRegistry
{
    private readonly Dictionary<string, bool> declarations = new(StringComparer.Ordinal);

    /// <summary>
    /// 声明一个模组是否要求阻止竞争性上传。
    /// Declares whether a mod requests competitive uploads to be blocked.
    /// </summary>
    /// <param name="declaration">成就策略声明。Achievement policy declaration.</param>
    public void Declare(AchievementPolicyDeclaration declaration)
    {
        if (string.IsNullOrWhiteSpace(declaration.ModGuid))
        {
            throw new ArgumentException("Mod GUID cannot be empty.", nameof(declaration));
        }

        declarations[declaration.ModGuid] = declaration.DisableAchievements;
    }

    /// <summary>
    /// 声明一个模组是否要求阻止竞争性上传。
    /// Declares whether a mod requests competitive uploads to be blocked.
    /// </summary>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <param name="disableAchievements">兼容参数：是否阻止竞争性上传。Compatibility parameter: whether competitive uploads should be blocked.</param>
    public void Declare(string modGuid, bool disableAchievements)
    {
        Declare(new AchievementPolicyDeclaration(modGuid, disableAchievements));
    }

    /// <summary>
    /// 声明一个模组是否要求阻止 Milky Way 和排行榜上传。
    /// Declares whether a mod requests Milky Way and leaderboard uploads to be blocked.
    /// </summary>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <param name="blockUpload">是否阻止上传。Whether uploads should be blocked.</param>
    public void BlockCompetitiveUpload(string modGuid, bool blockUpload = true)
    {
        Declare(modGuid, blockUpload);
    }

    /// <summary>
    /// 获取最终是否应阻止 Milky Way 和排行榜上传。
    /// Gets whether Milky Way and leaderboard uploads should be blocked in the final aggregated policy.
    /// </summary>
    /// <returns>任意模组声明阻止上传时返回 true。Returns true when any mod declares upload blocking.</returns>
    public bool ShouldBlockCompetitiveUpload()
    {
        return declarations.Values.Any(disableAchievements => disableAchievements);
    }

    /// <summary>
    /// 获取兼容旧入口的阻断状态。
    /// Gets the compatibility blocking state for the old entry point.
    /// </summary>
    /// <returns>兼容返回：任意模组声明阻止上传时返回 true。Compatibility return: true when any mod declares upload blocking.</returns>
    public bool ShouldDisableAchievements()
    {
        return ShouldBlockCompetitiveUpload();
    }

    /// <summary>
    /// 获取是否应屏蔽原版异常检查，让本地成就保持可用。
    /// Gets whether vanilla abnormality checks should be blocked so local achievements stay available.
    /// </summary>
    /// <returns>总是返回 true，让本地成就保持可用。Always returns true so local achievements stay available.</returns>
    public bool ShouldBlockAbnormalityChecks()
    {
        return true;
    }

    /// <summary>
    /// 获取是否应阻止本地成就获取。
    /// Gets whether local achievement access should be blocked.
    /// </summary>
    /// <returns>总是返回 false，保留本地成就获取。Always returns false so local achievements remain available.</returns>
    public bool ShouldBlockAchievementAccess()
    {
        return false;
    }

    /// <summary>
    /// 获取是否应阻止 Milky Way 和排行榜上传。
    /// Gets whether Milky Way and leaderboard uploads should be blocked.
    /// </summary>
    /// <returns>需要阻止上传时返回 true。Returns true when uploads should be blocked.</returns>
    public bool ShouldBlockLeaderboardUpload()
    {
        return ShouldBlockCompetitiveUpload();
    }

    /// <summary>
    /// 获取是否应阻止平台成就和元数据调用。
    /// Gets whether platform achievement and metadata calls should be blocked.
    /// </summary>
    /// <returns>总是返回 false，保留平台成就和元数据调用。Always returns false so platform achievement and metadata calls remain available.</returns>
    public bool ShouldBlockPlatformMetadata()
    {
        return false;
    }

    /// <summary>
    /// 获取所有成就策略声明。
    /// Gets all achievement policy declarations.
    /// </summary>
    /// <returns>声明快照。Snapshot of declarations.</returns>
    public IReadOnlyCollection<AchievementPolicyDeclaration> GetDeclarations()
    {
        return declarations
            .Select(item => new AchievementPolicyDeclaration(item.Key, item.Value))
            .ToArray();
    }
}
