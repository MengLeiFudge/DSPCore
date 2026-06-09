# Option Registration Example

Use this for small mod settings that should live in the DSPCore BepInEx config file.

Prefer `Options.Bool(...)`, `Options.Int(...)`, `Options.Float(...)`, and `Options.String(...)` for normal settings. They register the descriptor and return the current value in one call.

Call `Options.OpenWindow()` only from a button, key bind, or custom UI callback after `UIRoot` is initialized. Early plugin startup is too soon for Unity UI creation.
