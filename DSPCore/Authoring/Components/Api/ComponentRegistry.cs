using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理作者注册的实体组件描述。
/// Manages author-registered entity component descriptors.
/// </summary>
public sealed class ComponentRegistry
{
    private readonly Dictionary<string, ComponentDescriptor> descriptors = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册或替换一个实体组件描述。
    /// Registers or replaces an entity component descriptor.
    /// </summary>
    /// <param name="descriptor">组件描述。Component descriptor.</param>
    public void Register(ComponentDescriptor descriptor)
    {
        if (string.IsNullOrWhiteSpace(descriptor.ComponentId))
        {
            throw new ArgumentException("Component id cannot be empty.", nameof(descriptor));
        }

        descriptors[descriptor.ComponentId] = descriptor;
    }

    /// <summary>
    /// 尝试读取组件描述。
    /// Tries to get a component descriptor.
    /// </summary>
    /// <param name="componentId">组件 ID。Component ID.</param>
    /// <param name="descriptor">组件描述。Component descriptor.</param>
    /// <returns>找到时返回 true。Returns true when found.</returns>
    public bool TryGet(string componentId, out ComponentDescriptor descriptor)
    {
        return descriptors.TryGetValue(componentId, out descriptor);
    }

    /// <summary>
    /// 获取所有组件描述快照。
    /// Gets a snapshot of all component descriptors.
    /// </summary>
    /// <returns>组件描述集合。Component descriptor collection.</returns>
    public IReadOnlyCollection<ComponentDescriptor> GetAll()
    {
        return descriptors.Values;
    }
}
