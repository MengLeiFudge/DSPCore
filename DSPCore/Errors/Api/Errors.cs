using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧错误报告入口。
/// Author-facing error reporting entry point.
/// </summary>
public static class Errors
{
    /// <summary>
    /// 记录一个错误报告。
    /// Records an error report.
    /// </summary>
    public static void Report(ErrorReport report)
    {
        DspCore.Errors.Report(report);
    }

    /// <summary>
    /// 从异常创建并记录错误报告。
    /// Creates and records an error report from an exception.
    /// </summary>
    public static ErrorReport ReportException(string ownerModGuid, Exception exception)
    {
        return DspCore.Errors.ReportException(ownerModGuid, exception);
    }

    /// <summary>
    /// 获取所有错误报告。
    /// Gets all error reports.
    /// </summary>
    public static IReadOnlyList<ErrorReport> GetReports()
    {
        return DspCore.Errors.GetReports();
    }
}
