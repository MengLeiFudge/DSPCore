# Author Declaration Diagnostics

The Diagnostics module lets DSPCore check author-registered declarations during startup and write suspicious declarations to the log and copyable diagnostic text. The public surface includes `Diagnostics.Info(...)`, `Diagnostics.Warn(...)`, `Diagnostics.Error(...)`, `Diagnostics.RunBuiltInChecks()`, and `Diagnostics.GetIssues()`.

## What This Module Provides

- Mod authors do not need to iterate every DSPCore registry to find common declaration mistakes.
- DSPCore runs built-in checks during startup. Warning and error issues are written to the BepInEx log and appear in the Diagnostics section of `Errors.BuildDiagnosticText(...)`.
- Authors can use short entries to add their own business checks to the same diagnostic text.
- Diagnostics only report issues. They do not block loading or change Proto, Tab, Option, Resource, or UI runtime behavior.

## Capability: Report Author Declaration Issues

```csharp
Diagnostics.Warn(
    ownerModGuid: "com.example.my-mod",
    code: "example.recipe.unreachable",
    message: "Recipe is registered but no machine can craft it.",
    subject: "recipe=9554");
```

Keep `code` stable so authors and players can search logs. Use `subject` for the concrete object, such as `item=9554`, `tab=com.example.machines`, or `recipe=9554`.

## Capability: Built-In Checks

Startup runs built-in checks once automatically. Tests or debugging code can also read them manually:

```csharp
IReadOnlyList<DiagnosticIssue> issues = Diagnostics.RunBuiltInChecks();
```

Current built-in checks cover:

- Missing or duplicate IDs on registered protos.
- Duplicate `GridIndex` values on registered items and recipes.
- Custom `GridIndex` values that point to no registered DSPCore tab.
- Tab declarations that have no icon or reference an unknown icon id.
- Options that reference an unknown settings page.
- Localization keys that are missing a basic Chinese or English entry.

## Boundaries

- Diagnostics are author hints, not hard validation. DSPCore does not stop the game from loading because of these diagnostics.
- Built-in checks only inspect declarations currently registered with DSPCore. If a mod registers declarations later, call `Diagnostics.RunBuiltInChecks()` after that registration or report issues manually.
- `GridIndex` checks cover items and recipes registered through DSPCore. Objects that modify vanilla LDB directly without entering DSPCore registries are out of scope.
- Localization checks only require common Chinese/English base entries. They do not judge text quality or full language coverage.
- Runtime error reporting remains owned by ErrorWindow / Errors. Diagnostics owns author declaration quality.

## Examples

- `Examples/DeclarationDiagnostics.md`
- `Examples/DeclarationDiagnosticsExample.cs`
