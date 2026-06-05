# Icons

## Responsibility

This block registers icon descriptors and resolves shared icon resources.

## Public API

- `Icons`: author-facing short entry point.
- `IconDescriptor`
- `IconSetRegistry`

## Example

- `Examples/IconSetExample.cs`

## Runtime

`IconRuntime.cs` loads Unity `Resources` sprites or local PNG files, caches sprites, resolves fallbacks, and applies icons to target protos.

## Boundaries

Icon registration does not create item, recipe, or tech protos. Proto features call this block when they need icon binding.
