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
    /// 获取所有成就策略声明。
    /// Gets all achievement policy declarations.
    /// </summary>
    /// <returns>声明快照。Snapshot of declarations.</returns>
    public IReadOnlyCollection<AchievementPolicyDeclaration> GetDeclarations()
    {
        return declarations.Values;
    }
}
