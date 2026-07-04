namespace DSPCore;

/// <summary>
/// 描述短配置入口在原版设置窗口 DSPCore 分页中的展示元数据。
/// Describes presentation metadata for short option entries on the DSPCore page inside the vanilla option window.
/// </summary>
/// <param name="PageId">可选设置页面 ID。Optional settings page id.</param>
/// <param name="DisplayName">可选显示名称。Optional display name.</param>
public sealed record OptionUi(string? PageId = null, string? DisplayName = null)
{
    /// <summary>
    /// 空展示元数据。
    /// Empty presentation metadata.
    /// </summary>
    public static OptionUi Empty { get; } = new();

    /// <summary>
    /// 同页内排序值，数值越小越靠前。
    /// Sort order within the page; lower values appear earlier.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// 是否在原版设置窗口 DSPCore 分页显示重置按钮。
    /// Whether the DSPCore page inside the vanilla option window shows a reset button.
    /// </summary>
    public bool CanReset { get; init; }
}
