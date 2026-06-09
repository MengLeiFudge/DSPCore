namespace DSPCore;

/// <summary>
/// 表示作者声明诊断项的严重级别。
/// Represents the severity of an author declaration diagnostic issue.
/// </summary>
public enum DiagnosticSeverity
{
    /// <summary>
    /// 信息项，不表示错误。
    /// Informational item that does not indicate an error.
    /// </summary>
    Info = 0,

    /// <summary>
    /// 警告项，表示可能导致功能不可见或行为异常。
    /// Warning item that may make a feature invisible or behave unexpectedly.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// 错误项，表示声明冲突或缺少必要数据。
    /// Error item that indicates a declaration conflict or missing required data.
    /// </summary>
    Error = 2
}
