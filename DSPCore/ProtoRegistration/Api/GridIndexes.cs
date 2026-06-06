namespace DSPCore;

/// <summary>
/// 提供 DSP 原生 GridIndex 编码辅助。
/// Provides helpers for encoding DSP native GridIndex values.
/// </summary>
public static class GridIndexes
{
    /// <summary>
    /// 从分页槽位、行和格子编号生成物品或配方 GridIndex。
    /// Creates an item or recipe GridIndex from a tab slot, row, and index.
    /// </summary>
    /// <param name="tab">分页槽位。Tab slot.</param>
    /// <param name="row">行编号。Row number.</param>
    /// <param name="index">格子编号。Cell index.</param>
    /// <returns>DSP 原生 GridIndex。DSP native GridIndex.</returns>
    public static int From(TabSlot tab, int row, int index)
    {
        return From(tab.Value, row, index);
    }

    /// <summary>
    /// 从游戏分类编号、行和格子编号生成物品或配方 GridIndex。
    /// Creates an item or recipe GridIndex from a game category value, row, and index.
    /// </summary>
    /// <param name="tab">游戏分类编号。Game category value.</param>
    /// <param name="row">行编号。Row number.</param>
    /// <param name="index">格子编号。Cell index.</param>
    /// <returns>DSP 原生 GridIndex。DSP native GridIndex.</returns>
    public static int From(int tab, int row, int index)
    {
        return tab * 1000 + row * 100 + index;
    }
}
