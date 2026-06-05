namespace DSPCore;

/// <summary>
/// 定义 DSPCore 数据注册阶段。
/// Defines DSPCore data registration phases.
/// </summary>
public enum CoreDataPhase
{
    /// <summary>
    /// 初始数据声明阶段，适合注册新原型和资源。
    /// Initial data declaration phase, suitable for new protos and resources.
    /// </summary>
    Data = 1,

    /// <summary>
    /// 跨模组数据调整阶段，适合修改其他模组声明后的数据。
    /// Cross-mod data update phase, suitable for changing data after other mods declare it.
    /// </summary>
    DataUpdates = 2,

    /// <summary>
    /// 最终修正阶段，适合在派生缓存重建前做收口。
    /// Final fix phase, suitable for final changes before derived caches are rebuilt.
    /// </summary>
    DataFinalFixes = 3
}
