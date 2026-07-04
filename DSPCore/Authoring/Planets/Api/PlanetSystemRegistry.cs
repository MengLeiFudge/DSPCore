using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理作者注册的星球系统描述。
/// Manages author-registered planet system descriptors.
/// </summary>
public sealed class PlanetSystemRegistry
{
    private readonly Dictionary<string, PlanetSystemDescriptor> descriptors = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册或替换一个星球系统描述。
    /// Registers or replaces a planet system descriptor.
    /// </summary>
    /// <param name="descriptor">系统描述。System descriptor.</param>
    public void Register(PlanetSystemDescriptor descriptor)
    {
        if (string.IsNullOrWhiteSpace(descriptor.SystemId))
        {
            throw new ArgumentException("System id cannot be empty.", nameof(descriptor));
        }

        descriptors[descriptor.SystemId] = descriptor;
    }

    /// <summary>
    /// 尝试读取系统描述。
    /// Tries to get a system descriptor.
    /// </summary>
    /// <param name="systemId">系统 ID。System ID.</param>
    /// <param name="descriptor">系统描述。System descriptor.</param>
    /// <returns>找到时返回 true。Returns true when found.</returns>
    public bool TryGet(string systemId, out PlanetSystemDescriptor descriptor)
    {
        var found = descriptors.TryGetValue(systemId, out var value);
        descriptor = value!;
        return found;
    }

    /// <summary>
    /// 获取所有系统描述快照。
    /// Gets a snapshot of all system descriptors.
    /// </summary>
    /// <returns>系统描述集合。System descriptor collection.</returns>
    public IReadOnlyCollection<PlanetSystemDescriptor> GetAll()
    {
        return descriptors.Values;
    }
}
