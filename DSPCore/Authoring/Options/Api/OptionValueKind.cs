namespace DSPCore;

/// <summary>
/// 描述配置项在统一设置页中的基础控件类型。
/// Describes the basic control type for an option in the unified settings page.
/// </summary>
public enum OptionValueKind
{
    /// <summary>
    /// 文本输入。
    /// Text input.
    /// </summary>
    String,

    /// <summary>
    /// 布尔开关。
    /// Boolean toggle.
    /// </summary>
    Bool,

    /// <summary>
    /// 整数输入。
    /// Integer input.
    /// </summary>
    Int,

    /// <summary>
    /// 浮点输入。
    /// Floating-point input.
    /// </summary>
    Float,

    /// <summary>
    /// 枚举下拉选择。
    /// Enumeration dropdown.
    /// </summary>
    Enum,

    /// <summary>
    /// 整数范围滑条。
    /// Integer range slider.
    /// </summary>
    IntRange,

    /// <summary>
    /// 浮点范围滑条。
    /// Floating-point range slider.
    /// </summary>
    FloatRange,

    /// <summary>
    /// 按键绑定文本输入和捕获。
    /// Key binding text input and capture.
    /// </summary>
    KeyBinding
}
