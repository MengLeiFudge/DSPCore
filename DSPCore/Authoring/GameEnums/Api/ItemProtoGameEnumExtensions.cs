namespace DSPCore;

/// <summary>
/// 提供 ItemProto 的游戏枚举扩展辅助方法。
/// Provides game-enum extension helpers for ItemProto.
/// </summary>
public static class ItemProtoGameEnumExtensions
{
    /// <summary>
    /// 把当前物品标记为 DSPCore 自定义物品类型。
    /// Marks the current item as the DSPCore custom item type.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <returns>标记成功时返回 true。Returns true when the item was marked.</returns>
    public static bool SetCustomItemType(this ItemProto item)
    {
        if (item == null)
        {
            return false;
        }

        GameEnums.SetCustomItemType(item);
        return true;
    }

    /// <summary>
    /// 判断当前物品是否使用 DSPCore 自定义物品类型。
    /// Checks whether the current item uses the DSPCore custom item type.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <returns>使用自定义物品类型时返回 true。Returns true when the item uses the custom item type.</returns>
    public static bool IsCustomItemType(this ItemProto item)
    {
        return GameEnums.IsCustomItemType(item);
    }
}
