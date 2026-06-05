using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理资源、图标和本地化声明。
/// Manages resource, icon, and localization declarations.
/// </summary>
public sealed class ResourceRegistry
{
    private readonly Dictionary<string, ResourceDescriptor> resources = new(StringComparer.Ordinal);
    private readonly List<LocalizationEntry> localizations = new();

    /// <summary>
    /// 注册一个资源根。
    /// Registers a resource root.
    /// </summary>
    /// <param name="descriptor">资源描述。Resource descriptor.</param>
    public void RegisterResource(ResourceDescriptor descriptor)
    {
        resources[descriptor.Id] = descriptor;
    }

    /// <summary>
    /// 注册一个本地化文本。
    /// Registers a localized text entry.
    /// </summary>
    /// <param name="entry">本地化文本条目。Localization entry.</param>
    public void RegisterLocalization(LocalizationEntry entry)
    {
        localizations.Add(entry);
    }

    /// <summary>
    /// 获取所有资源根。
    /// Gets all resource roots.
    /// </summary>
    /// <returns>资源描述快照。Snapshot of resource descriptors.</returns>
    public IReadOnlyCollection<ResourceDescriptor> GetResources()
    {
        return resources.Values;
    }

    /// <summary>
    /// 获取所有本地化文本。
    /// Gets all localization entries.
    /// </summary>
    /// <returns>本地化文本快照。Snapshot of localization entries.</returns>
    public IReadOnlyList<LocalizationEntry> GetLocalizations()
    {
        return localizations.ToArray();
    }
}
