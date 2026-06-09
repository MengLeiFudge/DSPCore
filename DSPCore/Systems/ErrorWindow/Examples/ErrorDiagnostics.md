# Error Diagnostics Example

Use this when your mod catches an exception and wants to include DSPCore's shared diagnostic context in a bug report UI or log file.

`Errors.ReportException(...)` records the owned exception. `Errors.BuildDiagnosticText(...)` returns a copyable snapshot. `Errors.CopyDiagnosticText(...)` writes the same snapshot to the clipboard and returns it.
