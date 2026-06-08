using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理蓝图和复制粘贴参数块适配器。
/// Manages blueprint and copy-paste parameter block adapters.
/// </summary>
public sealed class BuildingParameterRegistry
{
    private readonly Dictionary<string, BuildingParameterDescriptor> descriptors = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册或替换参数块适配器。
    /// Registers or replaces a parameter block adapter.
    /// </summary>
    /// <param name="descriptor">参数块描述。Parameter block descriptor.</param>
    public void Register(BuildingParameterDescriptor descriptor)
    {
        if (string.IsNullOrWhiteSpace(descriptor.BlockId))
        {
            throw new ArgumentException("Building parameter block id cannot be empty.", nameof(descriptor));
        }

        descriptors[descriptor.BlockId] = descriptor;
    }

    /// <summary>
    /// 尝试读取参数块适配器。
    /// Tries to get a parameter block adapter.
    /// </summary>
    /// <param name="blockId">参数块 ID。Block ID.</param>
    /// <param name="descriptor">参数块描述。Parameter block descriptor.</param>
    /// <returns>找到时返回 true。Returns true when found.</returns>
    public bool TryGet(string blockId, out BuildingParameterDescriptor descriptor)
    {
        return descriptors.TryGetValue(blockId, out descriptor);
    }

    /// <summary>
    /// 获取所有参数块适配器快照。
    /// Gets a snapshot of all parameter block adapters.
    /// </summary>
    /// <returns>参数块描述集合。Parameter block descriptor collection.</returns>
    public IReadOnlyCollection<BuildingParameterDescriptor> GetAll()
    {
        return descriptors.Values;
    }
}
