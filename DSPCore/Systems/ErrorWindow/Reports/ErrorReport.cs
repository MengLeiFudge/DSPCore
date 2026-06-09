namespace DSPCore;

/// <summary>
/// 描述一个 DSPCore 错误报告。
/// Describes a DSPCore error report.
/// </summary>
/// <param name="OwnerModGuid">报告方模组 GUID。Reporting mod GUID.</param>
/// <param name="ErrorType">错误类型。Error type.</param>
/// <param name="Message">错误消息。Error message.</param>
/// <param name="StackTrace">堆栈信息。Stack trace.</param>
/// <param name="Context">可选诊断上下文。Optional diagnostic context.</param>
public sealed record ErrorReport(
    string OwnerModGuid,
    string ErrorType,
    string Message,
    string StackTrace,
    ErrorDiagnosticContext? Context = null)
{
    /// <summary>
    /// 创建不带诊断上下文的错误报告。
    /// Creates an error report without diagnostic context.
    /// </summary>
    /// <param name="ownerModGuid">报告方模组 GUID。Reporting mod GUID.</param>
    /// <param name="errorType">错误类型。Error type.</param>
    /// <param name="message">错误消息。Error message.</param>
    /// <param name="stackTrace">堆栈信息。Stack trace.</param>
    public ErrorReport(string ownerModGuid, string errorType, string message, string stackTrace)
        : this(ownerModGuid, errorType, message, stackTrace, null)
    {
    }
}
