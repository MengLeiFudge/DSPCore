namespace DSPCore;

/// <summary>
/// 描述一个模组的成就上传策略声明。
/// Describes one mod's achievement upload-policy declaration.
/// </summary>
/// <param name="ModGuid">模组 GUID。Mod GUID.</param>
/// <param name="DisableAchievements">兼容字段：是否阻止竞争性上传。Compatibility field: whether competitive uploads should be blocked.</param>
public sealed record AchievementPolicyDeclaration(
    string ModGuid,
    bool DisableAchievements);
