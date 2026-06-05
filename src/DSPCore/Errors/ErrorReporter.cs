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
    /// 从异常创建并记录错误报告。
    /// Creates and records an error report from an exception.
    /// </summary>
    /// <param name="ownerModGuid">报告方模组 GUID。Reporting mod GUID.</param>
    /// <param name="exception">异常对象。Exception object.</param>
    /// <returns>创建的错误报告。Created error report.</returns>
    public ErrorReport ReportException(string ownerModGuid, Exception exception)
    {
        var report = new ErrorReport(ownerModGuid, exception.GetType().FullName ?? exception.GetType().Name, exception.Message, exception.ToString());
        Report(report);
        return report;
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
}
