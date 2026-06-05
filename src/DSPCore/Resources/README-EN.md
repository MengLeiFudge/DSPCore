# Resources

## Responsibility

This block registers shared resources and localization entries.

## Public API

- `ResourceDescriptor`
- `ResourceRegistry`
- `LocalizationEntry`

## Runtime

`Runtime/LocalizationRuntime.cs` applies localization entries. `Runtime/ResourceRuntime.cs` currently handles icon sprite loading.

## Boundaries

Resource registration records ownership and paths. Feature-specific consumers decide how those resources are used.
