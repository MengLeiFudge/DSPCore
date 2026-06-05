using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 管理按游戏版本、目标模组和目标版本启用的兼容补丁。
/// Manages compatibility patches enabled by game version, target mod, and target version.
/// </summary>
public sealed class CompatibilityPatchRegistry
{
    private readonly List<CompatibilityPatchDescriptor> patches = new();

    /// <summary>
    /// 注册一个兼容补丁。
    /// Registers a compatibility patch.
    /// </summary>
    /// <param name="descriptor">兼容补丁描述。Compatibility patch descriptor.</param>
    public void Register(CompatibilityPatchDescriptor descriptor)
    {
        patches.Add(descriptor);
    }

    /// <summary>
    /// 查找适用于指定目标模组的兼容补丁。
    /// Finds compatibility patches for a target mod.
    /// </summary>
    /// <param name="targetModGuid">目标模组 GUID。Target mod GUID.</param>
    /// <returns>兼容补丁列表。List of compatibility patches.</returns>
    public IReadOnlyList<CompatibilityPatchDescriptor> FindForMod(string targetModGuid)
    {
        return patches.Where(item => string.Equals(item.TargetModGuid, targetModGuid, StringComparison.Ordinal)).ToArray();
    }

    /// <summary>
    /// 获取所有兼容补丁。
    /// Gets all compatibility patches.
    /// </summary>
    /// <returns>兼容补丁快照。Snapshot of compatibility patches.</returns>
    public IReadOnlyList<CompatibilityPatchDescriptor> GetAll()
    {
        return patches.ToArray();
    }
}
