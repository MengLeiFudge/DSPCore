using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者声明诊断能力的短入口。
/// Short entry point for author declaration diagnostics.
/// </summary>
public static class Diagnostics
{
    /// <summary>
    /// 记录一个诊断项。
    /// Records a diagnostic issue.
    /// </summary>
    /// <param name="issue">诊断项。Diagnostic issue.</param>
    public static void Report(DiagnosticIssue issue)
    {
        DspCore.Diagnostics.Report(issue);
    }

    /// <summary>
    /// 记录一个信息诊断项。
    /// Records an informational diagnostic issue.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="code">稳定诊断代码。Stable diagnostic code.</param>
    /// <param name="message">诊断消息。Diagnostic message.</param>
    /// <param name="subject">相关声明对象。Related declaration subject.</param>
    public static void Info(string ownerModGuid, string code, string message, string? subject = null)
    {
        DspCore.Diagnostics.Info(ownerModGuid, code, message, subject);
    }

    /// <summary>
    /// 记录一个警告诊断项。
    /// Records a warning diagnostic issue.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="code">稳定诊断代码。Stable diagnostic code.</param>
    /// <param name="message">诊断消息。Diagnostic message.</param>
    /// <param name="subject">相关声明对象。Related declaration subject.</param>
    public static void Warn(string ownerModGuid, string code, string message, string? subject = null)
    {
        DspCore.Diagnostics.Warn(ownerModGuid, code, message, subject);
    }

    /// <summary>
    /// 记录一个错误诊断项。
    /// Records an error diagnostic issue.
    /// </summary>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="code">稳定诊断代码。Stable diagnostic code.</param>
    /// <param name="message">诊断消息。Diagnostic message.</param>
    /// <param name="subject">相关声明对象。Related declaration subject.</param>
    public static void Error(string ownerModGuid, string code, string message, string? subject = null)
    {
        DspCore.Diagnostics.Error(ownerModGuid, code, message, subject);
    }

    /// <summary>
    /// 运行 DSPCore 内置作者声明检查。
    /// Runs DSPCore built-in author declaration checks.
    /// </summary>
    /// <returns>诊断项快照。Diagnostic issue snapshot.</returns>
    public static IReadOnlyList<DiagnosticIssue> RunBuiltInChecks()
    {
        return DspCore.Diagnostics.RunBuiltInChecks();
    }

    /// <summary>
    /// 获取已记录的诊断项。
    /// Gets recorded diagnostic issues.
    /// </summary>
    /// <returns>诊断项快照。Diagnostic issue snapshot.</returns>
    public static IReadOnlyList<DiagnosticIssue> GetIssues()
    {
        return DspCore.Diagnostics.GetIssues();
    }
}
