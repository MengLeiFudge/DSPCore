namespace DSPCore;

/// <summary>
/// 描述短配置入口在统一设置窗口中的展示元数据。
/// Describes presentation metadata for short option entries in the unified settings window.
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
}
