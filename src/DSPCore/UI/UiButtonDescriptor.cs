namespace DSPCore;

/// <summary>
/// 描述一个可由运行时创建的 UI 按钮。
/// Describes a UI button that can be created by a runtime adapter.
/// </summary>
/// <param name="Id">按钮 ID。Button id.</param>
/// <param name="Title">按钮标题。Button title.</param>
/// <param name="Tooltip">按钮提示。Button tooltip.</param>
public sealed record UiButtonDescriptor(string Id, string Title, string Tooltip);
