# Icons

## Responsibility

This block registers icon descriptors and resolves shared icon resources.

## Public API

- `Api/Icons.cs`: author-facing short entry point.
- `Api/IconDescriptor.cs`
- `Api/IconSetRegistry.cs`

## Example

- `Examples/IconSetRegistration.md`
- `Examples/IconSetRegistrationExample.cs`

## Runtime

`Runtime/IconRuntime.cs` loads Unity `Resources` sprites or local PNG files, caches sprites, resolves fallbacks, and applies icons to target protos.

## Boundaries

Icon registration does not create item, recipe, or tech protos. Proto features call this block when they need icon binding.
