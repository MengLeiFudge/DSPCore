namespace DSPCore;

/// <summary>
/// 提供 UI 节点创建的抽象入口，运行时适配层负责连接 Unity GameObject。
/// Provides an abstract entry point for UI node creation; runtime adapters connect it to Unity GameObjects.
/// </summary>
public sealed class UiNodeFactory
{
    /// <summary>
    /// 创建一个按钮描述。
    /// Creates a button descriptor.
    /// </summary>
    /// <param name="id">按钮 ID。Button id.</param>
    /// <param name="title">按钮标题。Button title.</param>
    /// <param name="tooltip">按钮提示。Button tooltip.</param>
    /// <returns>按钮描述。Button descriptor.</returns>
    public UiButtonDescriptor Button(string id, string title, string tooltip)
    {
        return new UiButtonDescriptor(id, title, tooltip);
    }
}
