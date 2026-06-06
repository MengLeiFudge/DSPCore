# Pickers

## Responsibility

This block declares requests to open vanilla item, recipe, or signal picker popups.

## Public API

- `Api/Pickers.cs`: author-facing short entry point.
- `Api/Pickers.cs#Open(request)`: request one picker popup.
- `Api/PickerRequest.cs`
- `Api/PickerRegistry.cs`
- `Api/PickerKind.cs`

## Example

- `Examples/PickerRequest.md`
- `Examples/PickerRequestExample.cs`

## Runtime

`Runtime/PickerRuntime.cs` consumes queued requests, opens the picker, applies return-time filters, and calls `OnReturn`.

## Boundaries

Filters currently validate the returned value. They do not yet hide invalid entries inside the live picker grid.
