# Input

## Responsibility

This block declares key bindings and their trigger behavior.

## Public API

- `Api/KeyBinds.cs`: author-facing short entry point.
- `Api/KeyBindDescriptor.cs`
- `Api/KeyBindRegistry.cs`
- `Api/CoreKeyAction.cs`

## Example

- `Examples/KeyBindRegistration.md`
- `Examples/KeyBindRegistrationExample.cs`

## Runtime

`Runtime/KeyBindRuntime.cs` polls registered key bindings and invokes callbacks for press, hold, or release.

## Boundaries

Current runtime supports direct callbacks and simple `Ctrl`/`Alt`/`Shift` modifiers. Full player rebinding UI is not implemented yet.
