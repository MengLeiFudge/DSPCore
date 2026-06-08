using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理银河级系统描述。
/// Manages galaxy-level system descriptors.
/// </summary>
public sealed class GalaxySystemRegistry
{
    private readonly Dictionary<string, GalaxySystemDescriptor> descriptors = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册或替换银河系统描述。
    /// Registers or replaces a galaxy system descriptor.
    /// </summary>
    /// <param name="descriptor">系统描述。System descriptor.</param>
    public void Register(GalaxySystemDescriptor descriptor)
    {
        descriptors[descriptor.SystemId] = descriptor;
    }

    /// <summary>
    /// 获取所有银河系统描述快照。
    /// Gets a snapshot of all galaxy system descriptors.
    /// </summary>
    /// <returns>系统描述集合。System descriptor collection.</returns>
    public IReadOnlyCollection<GalaxySystemDescriptor> GetAll()
    {
        return descriptors.Values;
    }
}
