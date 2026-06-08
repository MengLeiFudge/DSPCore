namespace DSPCore;

/// <summary>
/// 描述一个可由设置 UI 渲染的配置页面。
/// Describes an option page that can be rendered by a settings UI.
/// </summary>
/// <param name="PageId">页面稳定 ID。Stable page ID.</param>
/// <param name="OwnerModGuid">所属模组 GUID。Owner mod GUID.</param>
/// <param name="Title">页面标题。Page title.</param>
/// <param name="Order">排序值。Sort order.</param>
public sealed record OptionPageDescriptor(string PageId, string OwnerModGuid, string Title, int Order = 0);
