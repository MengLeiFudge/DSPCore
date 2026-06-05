using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧建造栏入口。
/// Author-facing build bar entry point.
/// </summary>
public static class BuildBar
{
    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位。
    /// Binds an item id to a quick build bar slot.
    /// </summary>
    public static bool BindQuickBar(int tab, int row, int index, int itemId)
    {
        return DspCore.BuildBar.BindQuickBar(tab, row, index, itemId);
    }

    /// <summary>
    /// 把一个物品 ID 绑定到快捷建造栏槽位。
    /// Binds an item id to a quick build bar slot.
    /// </summary>
    public static bool BindQuickBar(BuildBarSlot slot, int itemId)
    {
        return DspCore.BuildBar.BindQuickBar(slot, itemId);
    }

    /// <summary>
    /// 旧 SetBuildBar 兼容入口，将 category/index/layer 映射到 tab/row/index。
    /// Legacy SetBuildBar bridge that maps category/index/layer to tab/row/index.
    /// </summary>
    [System.Obsolete("Use BindQuickBar(tab, row, index, itemId) instead.")]
    public static bool SetBuildBar(int category, int index, int itemId, int layer = 1)
    {
        return DspCore.BuildBar.SetBuildBar(category, index, itemId, layer);
    }

    /// <summary>
    /// 使用旧 BuildBarTool 层级语义设置一个建造栏按钮绑定。
    /// Sets a build bar button binding using legacy BuildBarTool tier semantics.
    /// </summary>
    [System.Obsolete("Use BindQuickBar(tab, row, index, itemId) instead.")]
    public static bool SetBuildBar(int category, int index, int itemId, BuildBarTier tier)
    {
        return DspCore.BuildBar.SetBuildBar(category, index, itemId, tier);
    }

    /// <summary>
    /// 获取所有建造栏绑定。
    /// Gets all build bar bindings.
    /// </summary>
    public static IReadOnlyDictionary<BuildBarSlot, int> GetAll()
    {
        return DspCore.BuildBar.GetAll();
    }
}
