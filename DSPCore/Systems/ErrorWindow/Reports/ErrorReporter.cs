using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 收集并格式化错误报告，后续运行时可接入 DSP 错误窗口。
/// Collects and formats error reports; runtime adapters can later attach it to the DSP fatal error window.
/// </summary>
public sealed class ErrorReporter
{
    private readonly List<ErrorReport> reports = new();

    /// <summary>
    /// 记录一个错误报告。
    /// Records an error report.
    /// </summary>
    /// <param name="report">错误报告。Error report.</param>
    public void Report(ErrorReport report)
    {
        reports.Add(report);
    }

    /// <summary>
    /// 创建并记录一个错误报告。
    /// Creates and records an error report.
    /// </summary>
    /// <param name="ownerModGuid">报告方模组 GUID。Reporting mod GUID.</param>
    /// <param name="errorType">错误类型。Error type.</param>
    /// <param name="message">错误消息。Error message.</param>
    /// <param name="stackTrace">堆栈信息。Stack trace.</param>
    /// <param name="context">可选诊断上下文。Optional diagnostic context.</param>
    /// <returns>创建的错误报告。Created error report.</returns>
    public ErrorReport Report(string ownerModGuid, string errorType, string message, string stackTrace, ErrorDiagnosticContext? context = null)
    {
        var report = new ErrorReport(ownerModGuid, errorType, message, stackTrace, context);
        Report(report);
        return report;
    }

    /// <summary>
    /// 从异常创建并记录错误报告。
    /// Creates and records an error report from an exception.
    /// </summary>
    /// <param name="ownerModGuid">报告方模组 GUID。Reporting mod GUID.</param>
    /// <param name="exception">异常对象。Exception object.</param>
    /// <returns>创建的错误报告。Created error report.</returns>
    public ErrorReport ReportException(string ownerModGuid, Exception exception)
    {
        return ReportException(ownerModGuid, exception, null);
    }

    /// <summary>
    /// 从异常和诊断上下文创建并记录错误报告。
    /// Creates and records an error report from an exception and diagnostic context.
    /// </summary>
    /// <param name="ownerModGuid">报告方模组 GUID。Reporting mod GUID.</param>
    /// <param name="exception">异常对象。Exception object.</param>
    /// <param name="context">可选诊断上下文。Optional diagnostic context.</param>
    /// <returns>创建的错误报告。Created error report.</returns>
    public ErrorReport ReportException(string ownerModGuid, Exception exception, ErrorDiagnosticContext? context)
    {
        return Report(ownerModGuid, exception.GetType().FullName ?? exception.GetType().Name, exception.Message, exception.ToString(), context);
    }

    /// <summary>
    /// 获取所有错误报告。
    /// Gets all error reports.
    /// </summary>
    /// <returns>错误报告快照。Snapshot of error reports.</returns>
    public IReadOnlyList<ErrorReport> GetReports()
    {
        return reports.ToArray();
    }

    /// <summary>
    /// 生成包含错误报告、作者声明和 Harmony 补丁概览的诊断文本。
    /// Builds diagnostic text containing error reports, author declarations, and a Harmony patch overview.
    /// </summary>
    /// <param name="focalText">可选当前错误文本。Optional current error text.</param>
    /// <param name="maxReports">最多包含的最近报告数量。Maximum recent reports to include.</param>
    /// <param name="maxPatchedMethods">最多包含的 Harmony patched method 数量。Maximum Harmony patched methods to include.</param>
    /// <returns>可复制的诊断文本。Copyable diagnostic text.</returns>
    public string BuildDiagnosticText(string? focalText = null, int maxReports = 20, int maxPatchedMethods = 80)
    {
        return ErrorDiagnosticText.Build(focalText ?? string.Empty, maxReports, maxPatchedMethods);
    }

    /// <summary>
    /// 生成带游戏对象上下文的诊断文本。
    /// Builds diagnostic text with game-object context.
    /// </summary>
    /// <param name="focalText">可选当前错误文本。Optional current error text.</param>
    /// <param name="context">可选游戏对象上下文。Optional game-object context.</param>
    /// <param name="maxReports">最多包含的最近报告数量。Maximum recent reports to include.</param>
    /// <param name="maxPatchedMethods">最多包含的 Harmony patched method 数量。Maximum Harmony patched methods to include.</param>
    /// <returns>可复制的诊断文本。Copyable diagnostic text.</returns>
    public string BuildDiagnosticText(string? focalText, ErrorDiagnosticContext? context, int maxReports = 20, int maxPatchedMethods = 80)
    {
        return ErrorDiagnosticText.Build(focalText ?? string.Empty, maxReports, maxPatchedMethods, context);
    }
}
