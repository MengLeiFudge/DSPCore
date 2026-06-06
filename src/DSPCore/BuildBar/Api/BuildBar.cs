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
    /// 获取所有建造栏绑定。
    /// Gets all build bar bindings.
    /// </summary>
    public static IReadOnlyDictionary<BuildBarSlot, int> GetAll()
    {
        return DspCore.BuildBar.GetAll();
    }
}
