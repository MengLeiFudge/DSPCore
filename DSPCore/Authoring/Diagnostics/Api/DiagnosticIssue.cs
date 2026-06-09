namespace DSPCore;

/// <summary>
/// 描述一个作者声明诊断项。
/// Describes an author declaration diagnostic issue.
/// </summary>
/// <param name="Severity">严重级别。Severity.</param>
/// <param name="OwnerModGuid">所属模组 GUID。Owner mod GUID.</param>
/// <param name="Code">稳定诊断代码。Stable diagnostic code.</param>
/// <param name="Message">诊断消息。Diagnostic message.</param>
/// <param name="Subject">相关声明对象。Related declaration subject.</param>
public sealed record DiagnosticIssue(
    DiagnosticSeverity Severity,
    string OwnerModGuid,
    string Code,
    string Message,
    string? Subject = null);
