using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理恒星级系统描述。
/// Manages star-level system descriptors.
/// </summary>
public sealed class StarSystemRegistry
{
    private readonly Dictionary<string, StarSystemDescriptor> descriptors = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册或替换恒星系统描述。
    /// Registers or replaces a star system descriptor.
    /// </summary>
    /// <param name="descriptor">系统描述。System descriptor.</param>
    public void Register(StarSystemDescriptor descriptor)
    {
        descriptors[descriptor.SystemId] = descriptor;
    }

    /// <summary>
    /// 获取所有恒星系统描述快照。
    /// Gets a snapshot of all star system descriptors.
    /// </summary>
    /// <returns>系统描述集合。System descriptor collection.</returns>
    public IReadOnlyCollection<StarSystemDescriptor> GetAll()
    {
        return descriptors.Values;
    }
}
