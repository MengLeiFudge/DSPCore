# Errors

The Errors block collects structured error reports and enhances the vanilla fatal error window with copy/close actions. It gives mod authors one place to record owned exceptions and gives DSPCore runtime bridges a shared diagnostics trail.

## What This Block Gives You

- You can record exceptions through one entry point instead of maintaining separate error lists in each module.
- Error reports include owner mod GUID, error type, message, and stack trace for later diagnostics UI or log tooling.
- Unity error / exception / assert logs are captured as reports.
- `Errors.BuildDiagnosticText(...)` creates copyable diagnostics containing recent errors, text-hit candidate plugins, DSPCore author declarations, and a Harmony patch-map overview.
- The vanilla fatal window gains Copy and Close buttons. Copy writes the enhanced diagnostic text to the clipboard, and Close closes the window.

## Capability: Report Mod Exceptions

```csharp
try
{
    DoWork();
}
catch (Exception ex)
{
    Errors.ReportException("com.example.my-mod", ex);
}
```

You can also construct an `ErrorReport` directly and call `Errors.Report(report)`.

## Capability: Build Or Copy Diagnostic Text

```csharp
string text = Errors.BuildDiagnosticText("current error text");
Errors.CopyDiagnosticText("current error text");
```

The diagnostic text includes the current error text, recent error reports, text-hit candidate plugins, registered feature/module/patch declarations, and a Harmony patched-method owner overview. Candidate plugins are based only on GUID / name text hits and are not a root-cause verdict.

## What DSPCore Does After The Call

- `Report(...)` appends the report to the in-memory list.
- `ReportException(...)` creates an `ErrorReport` from exception type, message, and stack trace, then appends it.
- `GetReports()` returns the current report snapshot.
- `BuildDiagnosticText(...)` returns diagnostic text. `CopyDiagnosticText(...)` writes the same text to the system clipboard and returns it.
- When Unity receives error / exception / assert logs on the log thread, DSPCore records them under `UnityLog`.
- When `UIFatalErrorTip` shows an error or assertion failed message, DSPCore records a `DSPCore.FatalWindow` report and ensures Copy / Close buttons exist. Copy uses `BuildDiagnosticText(...)` to generate enhanced text.

## What This Block Does Not Own

- It does not automatically decide which mod caused the root problem. Candidate plugins only mean the error text matched a plugin GUID or name.
- It does not deeply inspect entity, planet, or factory objects yet.
- It does not upload logs or send external network requests.
- It does not restore your business state; it records errors and improves the player's error-copy flow.

## Examples

See `Examples/ErrorDiagnosticsExample.cs`.
