using System;
using DSPCore;

internal static class ErrorDiagnosticsExample
{
    public static void Run()
    {
        try
        {
            DoWork();
        }
        catch (Exception ex)
        {
            // ReportException records ownership, exception type, message, and stack trace.
            Errors.ReportException("com.example.my-mod", ex);

            // BuildDiagnosticText adds recent reports, text-hit plugin candidates, DSPCore
            // declarations, and a Harmony patch-map overview. It does not decide root cause.
            string diagnostics = Errors.BuildDiagnosticText(ex.ToString());

            // CopyDiagnosticText is useful from an in-game "copy report" button.
            Errors.CopyDiagnosticText(ex.ToString());
        }
    }

    private static void DoWork()
    {
        throw new InvalidOperationException("Example failure.");
    }
}
