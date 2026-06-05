namespace DSPCore;

/// <summary>
/// 定义成就策略元数据保留量。
/// Defines how much achievement-policy metadata is retained.
/// </summary>
public enum AchievementMetadataMode
{
    /// <summary>
    /// 只保留最终聚合结果。
    /// Keeps only the final aggregated result.
    /// </summary>
    AggregateOnly = 0,

    /// <summary>
    /// 保留每个模组的声明；这是默认值。
    /// Keeps each mod declaration; this is the default.
    /// </summary>
    DeclarationsOnly = 1,

    /// <summary>
    /// 保留声明和来源版本等诊断信息。
    /// Keeps declarations and source-version diagnostic information.
    /// </summary>
    Diagnostics = 2
}
