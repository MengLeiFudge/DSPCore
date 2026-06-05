# Pickers

## Responsibility

This block declares requests to open vanilla item, recipe, or signal picker popups.

## Public API

- `Pickers`: author-facing short entry point.
- `PickerRequest`
- `PickerRegistry`
- `PickerKind`

## Example

- `Examples/PickerExample.cs`

## Runtime

`Runtime/PickerRuntime.cs` consumes queued requests, opens the picker, applies return-time filters, and calls `OnReturn`.

## Boundaries

Filters currently validate the returned value. They do not yet hide invalid entries inside the live picker grid.
