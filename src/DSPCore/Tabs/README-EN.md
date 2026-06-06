# Tabs

## Responsibility

This block declares custom UI tabs for item, recipe, and replicator surfaces.

## Public API

- `Api/Tabs.cs`: author-facing short entry point.
- `Api/CoreTabDescriptor.cs`
- `Api/TabRegistry.cs`

## Example

- `Examples/TabRegistration.md`
- `Examples/TabRegistrationExample.cs`

## Runtime

`Runtime/TabRuntime.cs` clones existing type buttons and routes custom tab clicks through the vanilla GridIndex category flow.

## Boundaries

Signal picker, beacon, blueprint, and other surfaces need a richer tab-content model before they can be supported correctly.
