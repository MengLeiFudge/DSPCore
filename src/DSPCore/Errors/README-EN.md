# Errors

## Responsibility

This block collects structured error reports and exposes them to diagnostics UI/runtime code.

## Public API

- `Api/Errors.cs`: author-facing short entry point.
- `Api/ErrorReport.cs`
- `Api/ErrorReporter.cs`

## Runtime

`Runtime/ErrorRuntime.cs` captures Unity logs and enhances the fatal error window with close/copy actions.

## Boundaries

Candidate-mod analysis, Harmony patch maps, and entity inspection belong here when implemented.
