namespace DSPCore;

/// <summary>
/// 描述一个由 DSPCore 绑定的配置项。
/// Describes a configuration option bound by DSPCore.
/// </summary>
/// <param name="Section">配置分区。Config section.</param>
/// <param name="Key">配置键。Config key.</param>
/// <param name="DefaultValue">默认值。Default value.</param>
/// <param name="Description">配置说明。Config description.</param>
/// <param name="PageId">可选设置页面 ID。Optional settings page ID.</param>
/// <param name="Kind">基础控件类型。Basic control type.</param>
/// <param name="DisplayName">可选显示名称。Optional display name.</param>
/// <param name="Choices">可选候选值。Optional selectable choices.</param>
/// <param name="Minimum">可选最小值。Optional minimum value.</param>
/// <param name="Maximum">可选最大值。Optional maximum value.</param>
/// <param name="Step">可选步进值。Optional step value.</param>
public sealed record OptionDescriptor(
    string Section,
    string Key,
    string DefaultValue,
    string Description,
    string? PageId = null,
    OptionValueKind Kind = OptionValueKind.String,
    string? DisplayName = null,
    string[]? Choices = null,
    float? Minimum = null,
    float? Maximum = null,
    float? Step = null)
{
    /// <summary>
    /// 同页内排序值，数值越小越靠前。
    /// Sort order within the page; lower values appear earlier.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// 是否在统一设置窗口显示重置按钮。
    /// Whether the unified settings window shows a reset button.
    /// </summary>
    public bool CanReset { get; init; }
}
