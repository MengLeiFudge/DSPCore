using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 描述一行表单或详情属性。
/// Describes one form row or detail property.
/// </summary>
/// <param name="Label">左侧标签。Left-side label.</param>
/// <param name="Value">右侧显示值。Right-side display value.</param>
/// <param name="Tooltip">可选提示文本。Optional tooltip text.</param>
public sealed record UiFormRowDescriptor(string Label, string Value, string? Tooltip = null);

/// <summary>
/// 描述一个列表项。
/// Describes one list item.
/// </summary>
/// <param name="Id">稳定项 ID。Stable item id.</param>
/// <param name="Title">标题。Title.</param>
/// <param name="Summary">摘要。Summary.</param>
/// <param name="Selected">是否选中。Whether the item is selected.</param>
public sealed record UiListItemDescriptor(string Id, string Title, string Summary = "", bool Selected = false);

/// <summary>
/// 描述一个详情区。
/// Describes a detail panel.
/// </summary>
/// <param name="Title">详情标题。Detail title.</param>
/// <param name="Summary">详情摘要。Detail summary.</param>
/// <param name="Properties">属性行。Property rows.</param>
public sealed record UiDetailDescriptor(string Title, string Summary, IReadOnlyList<UiFormRowDescriptor>? Properties = null);
