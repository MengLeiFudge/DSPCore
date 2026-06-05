namespace DSPCore;

/// <summary>
/// 描述一个建造栏槽位。
/// Describes a build bar slot.
/// </summary>
/// <param name="Category">建造分类。Build category.</param>
/// <param name="Index">按钮索引。Button index.</param>
/// <param name="Layer">建造栏层级，从 1 开始。Build bar layer, starting from 1.</param>
public sealed record BuildBarSlot(int Category, int Index, int Layer = 1);
