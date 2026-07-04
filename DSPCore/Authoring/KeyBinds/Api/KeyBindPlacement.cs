namespace DSPCore;

/// <summary>
/// 旧按键显示位置枚举；当前按键显示由原版按键页负责。
/// Legacy key display placement enum; key display now belongs to the vanilla key-binding page.
/// </summary>
[System.Obsolete("Key bindings are displayed by the vanilla key-binding page.")]
public enum KeyBindPlacement
{
    /// <summary>
    /// 旧值：只显示在模组自己的设置页面。
    /// Legacy value: show only on the owning mod's settings page.
    /// </summary>
    ModSettings,

    /// <summary>
    /// 旧值：只显示在统一按键绑定页面。
    /// Legacy value: show only on the unified key bindings page.
    /// </summary>
    KeyBindings,

    /// <summary>
    /// 旧值：同时显示在模组设置页面和统一按键绑定页面。
    /// Legacy value: show on both the owning mod's settings page and the unified key bindings page.
    /// </summary>
    Both
}
