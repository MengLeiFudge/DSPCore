using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理图标资源注册、查找和 fallback。
/// Manages icon resource registration, lookup, and fallback.
/// </summary>
public sealed class IconSetRegistry
{
    private readonly Dictionary<string, IconDescriptor> icons = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册一个图标描述。
    /// Registers an icon descriptor.
    /// </summary>
    /// <param name="descriptor">图标描述。Icon descriptor.</param>
    public void Register(IconDescriptor descriptor)
    {
        icons[descriptor.Id] = descriptor;
    }

    /// <summary>
    /// 尝试按图标 ID 获取图标描述。
    /// Tries to get an icon descriptor by icon id.
    /// </summary>
    /// <param name="id">图标 ID。Icon id.</param>
    /// <param name="descriptor">返回的图标描述。Returned icon descriptor.</param>
    /// <returns>找到图标时返回 true。Returns true when the icon exists.</returns>
    public bool TryGet(string id, out IconDescriptor descriptor)
    {
        return icons.TryGetValue(id, out descriptor!);
    }

    /// <summary>
    /// 获取所有图标描述。
    /// Gets all icon descriptors.
    /// </summary>
    /// <returns>图标描述快照。Snapshot of icon descriptors.</returns>
    public IReadOnlyCollection<IconDescriptor> GetAll()
    {
        return icons.Values;
    }
}
