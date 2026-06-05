# Resources

## Responsibility

This block registers shared resources and localization entries.

## Public API

- `ResourceDescriptor`
- `ResourceRegistry`
- `LocalizationEntry`

## Runtime

`LocalizationRuntime.cs` applies localization entries. Icon sprite loading is owned by `Icons/IconRuntime.cs`.

## Boundaries

Resource registration records ownership and paths. Feature-specific consumers decide how those resources are used.
