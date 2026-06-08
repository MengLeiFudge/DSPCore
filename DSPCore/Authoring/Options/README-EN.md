# Options

The Options module lets mods declare simple options that DSPCore binds to the BepInEx `ConfigFile`. The public surface includes string options, boolean/integer read helpers, option page descriptors, and settings version descriptors.

## What This Module Provides

- Mods can declare options from authoring code without holding the DSPCore plugin instance.
- DSPCore binds options registered before startup, and also binds options registered after startup.
- A settings UI can be generated from `OptionPageDescriptor` and `OptionDescriptor` values with `PageId`.
- Multiplayer or save compatibility checks can read `OptionVersionDescriptor` values.

## Boundaries

- The underlying BepInEx values are still strings. Boolean and integer read helpers exist, while enums are parsed by authors.
- This module does not create concrete settings pages directly.
- Config keys should stay stable so player configuration remains valid.

## Examples

- `Examples/OptionRegistration.md`
- `Examples/OptionRegistrationExample.cs`
