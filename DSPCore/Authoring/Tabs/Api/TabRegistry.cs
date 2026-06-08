using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 管理作者声明的分页，并由运行时适配到多个游戏界面。
/// Manages author-declared tabs and lets runtime adapters project them into game UIs.
/// </summary>
public sealed class TabRegistry
{
    private readonly Dictionary<string, TabRegistration> tabs = new(StringComparer.Ordinal);
    private int nextCustomSlot = TabSlot.FirstCustomValue;

    /// <summary>
    /// 注册一个分页。
    /// Registers a tab.
    /// </summary>
    /// <param name="descriptor">分页描述。Tab descriptor.</param>
    /// <returns>分配给该分页的槽位。Slot assigned to this tab.</returns>
    public TabSlot AddTab(CoreTabDescriptor descriptor)
    {
        if (tabs.TryGetValue(descriptor.Id, out var existing))
        {
            tabs[descriptor.Id] = new TabRegistration(descriptor, existing.Slot);
            return existing.Slot;
        }

        var slot = new TabSlot(nextCustomSlot++);
        tabs[descriptor.Id] = new TabRegistration(descriptor, slot);
        return slot;
    }

    /// <summary>
    /// 尝试获取已注册分页的槽位。
    /// Tries to get the slot assigned to a registered tab.
    /// </summary>
    /// <param name="id">分页 ID。Tab id.</param>
    /// <param name="slot">分页槽位。Tab slot.</param>
    /// <returns>找到时为 true。True when the tab exists.</returns>
    public bool TryGetSlot(string id, out TabSlot slot)
    {
        if (tabs.TryGetValue(id, out var registration))
        {
            slot = registration.Slot;
            return true;
        }

        slot = default;
        return false;
    }

    /// <summary>
    /// 获取所有分页，按排序值和 ID 排序。
    /// Gets all tabs ordered by sort order and id.
    /// </summary>
    /// <returns>分页描述快照。Snapshot of tab descriptors.</returns>
    public IReadOnlyList<CoreTabDescriptor> GetAll()
    {
        return tabs.Values
            .Select(item => item.Descriptor)
            .OrderBy(item => item.Order)
            .ThenBy(item => item.Id, StringComparer.Ordinal)
            .ToArray();
    }

    internal IReadOnlyList<TabRegistration> GetRegistrations()
    {
        return tabs.Values
            .OrderBy(item => item.Descriptor.Order)
            .ThenBy(item => item.Descriptor.Id, StringComparer.Ordinal)
            .ToArray();
    }

    internal readonly struct TabRegistration
    {
        public TabRegistration(CoreTabDescriptor descriptor, TabSlot slot)
        {
            Descriptor = descriptor;
            Slot = slot;
        }

        public CoreTabDescriptor Descriptor { get; }

        public TabSlot Slot { get; }
    }
}
