using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理框架级补丁声明，后续由运行时适配层转换为 Harmony 补丁。
/// Manages framework-level patch declarations that runtime adapters can later apply through Harmony.
/// </summary>
public sealed class PatchRegistry
{
    private readonly List<PatchDescriptor> patches = new();

    /// <summary>
    /// 注册一个补丁描述。
    /// Registers a patch descriptor.
    /// </summary>
    /// <param name="descriptor">补丁描述。Patch descriptor.</param>
    public void Register(PatchDescriptor descriptor)
    {
        if (string.IsNullOrWhiteSpace(descriptor.Id))
        {
            throw new ArgumentException("Patch id cannot be empty.", nameof(descriptor));
        }

        patches.Add(descriptor);
    }

    /// <summary>
    /// 获取所有补丁描述。
    /// Gets all patch descriptors.
    /// </summary>
    /// <returns>补丁描述快照。Snapshot of patch descriptors.</returns>
    public IReadOnlyList<PatchDescriptor> GetAll()
    {
        return patches.ToArray();
    }
}
