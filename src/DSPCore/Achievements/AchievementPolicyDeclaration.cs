namespace DSPCore;

/// <summary>
/// 描述一个模组的成就策略声明。
/// Describes one mod's achievement policy declaration.
/// </summary>
/// <param name="ModGuid">模组 GUID。Mod GUID.</param>
/// <param name="DisableAchievements">是否禁用成就。Whether achievements should be disabled.</param>
/// <param name="Reason">声明原因。Declaration reason.</param>
/// <param name="SourceVersion">声明方版本。Declaring source version.</param>
public sealed record AchievementPolicyDeclaration(
    string ModGuid,
    bool DisableAchievements,
    string Reason,
    string? SourceVersion = null);
