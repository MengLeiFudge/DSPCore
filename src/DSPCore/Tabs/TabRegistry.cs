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
    private readonly Dictionary<string, CoreTabDescriptor> tabs = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册一个分页。
    /// Registers a tab.
    /// </summary>
    /// <param name="descriptor">分页描述。Tab descriptor.</param>
    public void AddTab(CoreTabDescriptor descriptor)
    {
        tabs[descriptor.Id] = descriptor;
    }

    /// <summary>
    /// 获取所有分页，按排序值和 ID 排序。
    /// Gets all tabs ordered by sort order and id.
    /// </summary>
    /// <returns>分页描述快照。Snapshot of tab descriptors.</returns>
    public IReadOnlyList<CoreTabDescriptor> GetAll()
    {
        return tabs.Values
            .OrderBy(item => item.Order)
            .ThenBy(item => item.Id, StringComparer.Ordinal)
            .ToArray();
    }
}
