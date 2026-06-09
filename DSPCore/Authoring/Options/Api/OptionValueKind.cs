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
    Float
}
