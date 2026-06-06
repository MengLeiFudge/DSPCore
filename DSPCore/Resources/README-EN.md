# Resources

## Responsibility

This block registers shared resources and localization entries.

## Public API

- `Api/ResourceDescriptor.cs`
- `Api/ResourceRegistry.cs`
- `Api/LocalizationEntry.cs`

## Runtime

`Runtime/LocalizationRuntime.cs` applies localization entries. Icon sprite loading is owned by `../Icons/Runtime/IconRuntime.cs`.

## Boundaries

Resource registration records ownership and paths. Feature-specific consumers decide how those resources are used.
