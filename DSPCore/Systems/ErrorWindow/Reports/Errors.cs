using System;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// 生成包含错误报告、作者声明和 Harmony 补丁概览的诊断文本。
    /// Builds diagnostic text containing error reports, author declarations, and a Harmony patch overview.
    /// </summary>
    public static string BuildDiagnosticText(string? focalText = null, int maxReports = 20, int maxPatchedMethods = 80)
    {
        return DspCore.Errors.BuildDiagnosticText(focalText, maxReports, maxPatchedMethods);
    }

    /// <summary>
    /// 生成诊断文本并复制到系统剪贴板。
    /// Builds diagnostic text and copies it to the system clipboard.
    /// </summary>
    public static string CopyDiagnosticText(string? focalText = null, int maxReports = 20, int maxPatchedMethods = 80)
    {
        string text = BuildDiagnosticText(focalText, maxReports, maxPatchedMethods);
        GUIUtility.systemCopyBuffer = text;
        return text;
    }
}
