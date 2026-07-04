namespace DSPCore;

/// <summary>
/// 控制可重绑定按键在统一设置窗口中的显示位置。
/// Controls where rebindable keys are shown in the unified settings window.
/// </summary>
public enum KeyBindPlacement
{
    /// <summary>
    /// 只显示在模组自己的设置页面。
    /// Show only on the owning mod's settings page.
    /// </summary>
    ModSettings,

    /// <summary>
    /// 只显示在统一按键绑定页面。
    /// Show only on the unified key bindings page.
    /// </summary>
    KeyBindings,

    /// <summary>
    /// 同时显示在模组设置页面和统一按键绑定页面。
    /// Show on both the owning mod's settings page and the unified key bindings page.
    /// </summary>
    Both
}
