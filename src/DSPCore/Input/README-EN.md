# Input

## Responsibility

This block declares key bindings and their trigger behavior.

## Public API

- `KeyBinds`: author-facing short entry point.
- `KeyBindDescriptor`
- `KeyBindRegistry`
- `CoreKeyAction`

## Example

- `Examples/KeyBindExample.cs`

## Runtime

`Runtime/KeyBindRuntime.cs` polls registered key bindings and invokes callbacks for press, hold, or release.

## Boundaries

Current runtime supports direct callbacks and simple `Ctrl`/`Alt`/`Shift` modifiers. Full player rebinding UI is not implemented yet.
