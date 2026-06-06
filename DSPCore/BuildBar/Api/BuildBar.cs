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

    /// <summary>
    /// 设置玩家自定义建造栏覆盖；itemId 为 0 时表示玩家显式清空槽位。
    /// Sets a player-defined build bar override; itemId 0 explicitly empties the slot.
    /// </summary>
    public static bool SetPlayerOverride(int tab, int row, int index, int itemId)
    {
        return DspCore.BuildBar.SetPlayerOverride(tab, row, index, itemId);
    }

    /// <summary>
    /// 设置玩家自定义建造栏覆盖；itemId 为 0 时表示玩家显式清空槽位。
    /// Sets a player-defined build bar override; itemId 0 explicitly empties the slot.
    /// </summary>
    public static bool SetPlayerOverride(BuildBarSlot slot, int itemId)
    {
        return DspCore.BuildBar.SetPlayerOverride(slot, itemId);
    }

    /// <summary>
    /// 清除一个玩家自定义建造栏覆盖。
    /// Clears a player-defined build bar override.
    /// </summary>
    public static bool ClearPlayerOverride(BuildBarSlot slot)
    {
        return DspCore.BuildBar.ClearPlayerOverride(slot);
    }

    /// <summary>
    /// 获取作者默认绑定叠加玩家覆盖后的有效建造栏绑定。
    /// Gets effective build bar bindings after applying player overrides on top of author defaults.
    /// </summary>
    public static IReadOnlyDictionary<BuildBarSlot, int> GetEffectiveBindings()
    {
        return DspCore.BuildBar.GetEffectiveBindings();
    }
}
