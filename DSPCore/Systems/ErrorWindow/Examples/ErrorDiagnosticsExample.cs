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
            // ReportException records ownership, exception type, message, stack trace,
            // and optional context when the failing code already knows the game object.
            PlanetFactory factory = GameMain.localPlanet?.factory;
            if (factory != null)
            {
                Errors.ReportException(
                    "com.example.my-mod",
                    ex,
                    ErrorDiagnosticContext.ForEntity(factory, entityId: 1, note: "Example entity context"));
            }
            else
            {
                Errors.ReportException("com.example.my-mod", ex);
            }

            // BuildDiagnosticText adds game context, recent reports, text-hit plugin candidates,
            // DSPCore declarations, and a Harmony patch-map overview. It does not decide root cause.
            string diagnostics = Errors.BuildDiagnosticText(ex.ToString());

            // When you only need context for one copied snapshot, pass explicit context here.
            if (factory != null)
            {
                string entityDiagnostics = Errors.BuildDiagnosticText(
                    ex.ToString(),
                    ErrorDiagnosticContext.ForEntity(factory, entityId: 1, note: "Example entity context"));
            }

            // CopyDiagnosticText is useful from an in-game "copy report" button.
            Errors.CopyDiagnosticText(ex.ToString());
        }
    }

    private static void DoWork()
    {
        throw new InvalidOperationException("Example failure.");
    }
}
