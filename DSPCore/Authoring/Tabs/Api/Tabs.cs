using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧分页入口。
/// Author-facing tab entry point.
/// </summary>
public static class Tabs
{
    /// <summary>
    /// 注册一个分页。
    /// Registers a tab.
    /// </summary>
    /// <returns>分配给该分页的槽位。Slot assigned to this tab.</returns>
    public static TabSlot AddTab(CoreTabDescriptor descriptor)
    {
        return DspCore.Tabs.AddTab(descriptor);
    }

    /// <summary>
    /// 注册一个分页。
    /// Registers a tab.
    /// </summary>
    /// <returns>分配给该分页的槽位。Slot assigned to this tab.</returns>
    public static TabSlot Add(CoreTabDescriptor descriptor)
    {
        return AddTab(descriptor);
    }

    /// <summary>
    /// 尝试获取已注册分页的槽位。
    /// Tries to get the slot assigned to a registered tab.
    /// </summary>
    public static bool TryGetSlot(string id, out TabSlot slot)
    {
        return DspCore.Tabs.TryGetSlot(id, out slot);
    }

    /// <summary>
    /// 获取所有分页，按排序值和 ID 排序。
    /// Gets all tabs ordered by sort order and id.
    /// </summary>
    public static IReadOnlyList<CoreTabDescriptor> GetAll()
    {
        return DspCore.Tabs.GetAll();
    }
}
