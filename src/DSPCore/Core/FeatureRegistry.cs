using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 管理 DSPCore 功能块声明、依赖和初始化状态。
/// Manages DSPCore feature block declarations, dependencies, and initialization state.
/// </summary>
public sealed class FeatureRegistry
{
    private readonly Dictionary<string, FeatureDescriptor> features = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册一个功能块。
    /// Registers a feature block.
    /// </summary>
    /// <param name="descriptor">功能块描述。Feature block descriptor.</param>
    public void Register(FeatureDescriptor descriptor)
    {
        if (string.IsNullOrWhiteSpace(descriptor.Id))
        {
            throw new ArgumentException("Feature id cannot be empty.", nameof(descriptor));
        }

        features[descriptor.Id] = descriptor;
    }

    /// <summary>
    /// 尝试按 ID 获取功能块。
    /// Tries to get a feature block by id.
    /// </summary>
    /// <param name="id">功能块 ID。Feature block id.</param>
    /// <param name="descriptor">返回的功能块描述。Returned feature block descriptor.</param>
    /// <returns>找到功能块时返回 true。Returns true when the feature exists.</returns>
    public bool TryGet(string id, out FeatureDescriptor descriptor)
    {
        return features.TryGetValue(id, out descriptor!);
    }

    /// <summary>
    /// 获取所有功能块，按优先级和 ID 排序。
    /// Gets all feature blocks ordered by priority and id.
    /// </summary>
    /// <returns>功能块描述快照。Snapshot of feature block descriptors.</returns>
    public IReadOnlyList<FeatureDescriptor> GetAll()
    {
        return features.Values
            .OrderBy(item => item.Priority)
            .ThenBy(item => item.Id, StringComparer.Ordinal)
            .ToArray();
    }
}
