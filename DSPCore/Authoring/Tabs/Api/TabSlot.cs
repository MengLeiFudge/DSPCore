namespace DSPCore;

/// <summary>
/// 表示一个已分配的原型分页槽位。
/// Represents an assigned proto tab slot.
/// </summary>
/// <param name="Value">游戏分类编号。Game category value.</param>
public readonly record struct TabSlot(int Value)
{
    /// <summary>
    /// 第一个 DSPCore 自定义分页槽位。
    /// First DSPCore custom tab slot.
    /// </summary>
    public const int FirstCustomValue = 3;

    /// <summary>
    /// 获取这个槽位的游戏分类编号。
    /// Gets the game category value for this slot.
    /// </summary>
    /// <returns>游戏分类编号。Game category value.</returns>
    public int ToCategory()
    {
        return Value;
    }
}
