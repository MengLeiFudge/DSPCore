namespace DSPCore;

/// <summary>
/// 描述一个 DSPCore 错误报告。
/// Describes a DSPCore error report.
/// </summary>
/// <param name="OwnerModGuid">报告方模组 GUID。Reporting mod GUID.</param>
/// <param name="ErrorType">错误类型。Error type.</param>
/// <param name="Message">错误消息。Error message.</param>
/// <param name="StackTrace">堆栈信息。Stack trace.</param>
public sealed record ErrorReport(string OwnerModGuid, string ErrorType, string Message, string StackTrace);
