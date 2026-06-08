# Errors

The Errors block collects structured error reports and enhances the vanilla fatal error window with copy/close actions. It gives mod authors one place to record owned exceptions and gives DSPCore runtime bridges a shared diagnostics trail.

## What This Block Gives You

- You can record exceptions through one entry point instead of maintaining separate error lists in each module.
- Error reports include owner mod GUID, error type, message, and stack trace for later diagnostics UI or log tooling.
- Unity error / exception / assert logs are captured as reports.
- The vanilla fatal window gains Copy and Close buttons so players can copy error text and close the window more easily.

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

## What DSPCore Does After The Call

- `Report(...)` appends the report to the in-memory list.
- `ReportException(...)` creates an `ErrorReport` from exception type, message, and stack trace, then appends it.
- `GetReports()` returns the current report snapshot.
- When Unity receives error / exception / assert logs on the log thread, DSPCore records them under `UnityLog`.
- When `UIFatalErrorTip` shows an error or assertion failed message, DSPCore records a `DSPCore.FatalWindow` report and ensures Copy / Close buttons exist.

## What This Block Does Not Own

- It does not automatically decide which mod caused the root problem. Candidate-mod analysis, Harmony patch maps, and entity inspection are future capabilities.
- It does not upload logs or send external network requests.
- It does not restore your business state; it records errors and improves the player's error-copy flow.

## Examples

Errors does not yet have standalone examples. The author-facing entries are `Errors.Report(...)` and `Errors.ReportException(...)`.
