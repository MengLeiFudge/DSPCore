namespace DSPCore;

/// <summary>
/// 建造栏作者默认绑定遇到已有绑定时的处理策略。
/// Conflict policy used when an author build bar default binding meets an existing binding.
/// </summary>
public enum BuildBarConflictPolicy
{
    /// <summary>
    /// 保留已有绑定并报告冲突。
    /// Keeps the existing binding and reports a conflict.
    /// </summary>
    KeepExisting = 0,

    /// <summary>
    /// 用新的作者默认绑定替换已有绑定。
    /// Replaces the existing author default binding with the new binding.
    /// </summary>
    ReplaceExisting = 1
}
