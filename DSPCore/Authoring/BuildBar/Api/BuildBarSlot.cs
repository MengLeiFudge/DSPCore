namespace DSPCore;

/// <summary>
/// 描述一个建造栏槽位。
/// Describes a build bar slot.
/// </summary>
/// <param name="Tab">建造栏分类，从 1 开始；旧名称 Tab 仅为兼容。Build bar category, starting from 1; the old Tab name is kept for compatibility.</param>
/// <param name="Row">建造栏行号，从 1 开始。Build bar row, starting from 1.</param>
/// <param name="Index">按钮索引。Button index.</param>
public sealed record BuildBarSlot(int Tab, int Row, int Index)
{
    /// <summary>
    /// 建造栏分类，从 1 开始。
    /// Build bar category, starting from 1.
    /// </summary>
    public int Category => Tab;
}
