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
    public static void AddTab(CoreTabDescriptor descriptor)
    {
        DspCore.Tabs.AddTab(descriptor);
    }

    /// <summary>
    /// 注册一个分页。
    /// Registers a tab.
    /// </summary>
    public static void Add(CoreTabDescriptor descriptor)
    {
        AddTab(descriptor);
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
