using System;
using System.Linq;

namespace DSPCore;

internal static class DiagnosticRuntime
{
    public static void Initialize()
    {
        var issues = DspCore.Diagnostics.RunBuiltInChecks();
        var reportable = issues
            .Where(item => item.Severity != DiagnosticSeverity.Info)
            .ToArray();
        if (reportable.Length == 0)
        {
            return;
        }

        foreach (var issue in reportable)
        {
            var message = FormatIssue(issue);
            if (issue.Severity == DiagnosticSeverity.Error)
            {
                DspCore.Logger?.LogError(message);
            }
            else
            {
                DspCore.Logger?.LogWarning(message);
            }

            DspCore.Errors.Report(
                issue.OwnerModGuid,
                "DSPCore." + issue.Code,
                message,
                Environment.StackTrace,
                new ErrorDiagnosticContext(Note: issue.Subject));
        }
    }

    private static string FormatIssue(DiagnosticIssue issue)
    {
        var text = issue.Severity + " " + issue.Code + ": " + issue.Message;
        return string.IsNullOrWhiteSpace(issue.Subject) ? text : text + " | " + issue.Subject;
    }
}
