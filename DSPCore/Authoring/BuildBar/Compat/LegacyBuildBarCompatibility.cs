namespace DSPCore;

/// <summary>
/// 提供旧 BuildBarTool 层级语义到 DSPCore 建造栏槽位模型的兼容桥接。
/// Provides compatibility bridges from legacy BuildBarTool tier semantics to DSPCore build bar slots.
/// </summary>
public static class LegacyBuildBarCompatibility
{
    /// <summary>
    /// 使用旧 SetBuildBar 语义设置一个建造栏按钮绑定。
    /// Sets a build bar button binding using legacy SetBuildBar semantics.
    /// </summary>
    /// <param name="category">旧建造分类；映射为 category。Legacy build category; mapped to category.</param>
    /// <param name="index">旧按钮索引；映射为 index。Legacy button index; mapped to index.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="layer">旧建造栏层级；映射为 row。Legacy build bar layer; mapped to row.</param>
    /// <returns>绑定写入当前作者默认绑定时返回 true。Returns true when the binding becomes the current author default.</returns>
    [System.Obsolete("Use DSPCore.BuildBar.BindQuickBar(category, row, index, itemId) instead.")]
    public static bool SetBuildBar(int category, int index, int itemId, int layer = 1)
    {
        return BuildBar.BindQuickBar(category, layer, index, itemId);
    }

    /// <summary>
    /// 使用旧 BuildBarTool 层级语义设置一个建造栏按钮绑定。
    /// Sets a build bar button binding using legacy BuildBarTool tier semantics.
    /// </summary>
    /// <param name="category">建造分类，从 1 开始。Build category, starting from 1.</param>
    /// <param name="index">按钮索引，从 1 开始。Button index, starting from 1.</param>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="tier">旧建造栏层级。Legacy build bar tier.</param>
    /// <returns>绑定写入当前作者默认绑定时返回 true。Returns true when the binding becomes the current author default.</returns>
    [System.Obsolete("Use DSPCore.BuildBar.BindQuickBar(category, row, index, itemId) instead.")]
    public static bool SetBuildBar(int category, int index, int itemId, BuildBarTier tier)
    {
        return SetBuildBar(category, index, itemId, (int)tier);
    }
}
