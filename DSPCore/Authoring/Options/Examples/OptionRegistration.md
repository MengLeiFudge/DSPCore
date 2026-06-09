# Option Registration Example

Use this for small mod settings that should live in the DSPCore BepInEx config file.

Prefer `Options.Bool(...)`, `Options.Int(...)`, `Options.Float(...)`, `Options.String(...)`, `Options.Enum(...)`, `Options.IntRange(...)`, and `Options.FloatRange(...)` for normal settings. They register the descriptor and return the current value in one call.

Use the same method name with `OptionUi` when the row needs a display name or page id. This keeps simple settings short while avoiding a fall back to low-level `Register(...)` for ordinary presentation metadata.

Call `Options.OpenWindow()` only from a button, key bind, or custom UI callback after `UIRoot` is initialized. Early plugin startup is too soon for Unity UI creation. The current window is DSPCore-owned and does not inject into the vanilla option page.
