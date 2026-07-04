using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理可重绑定按键声明。
/// Manages rebindable key binding declarations.
/// </summary>
public sealed class KeyBindRegistry
{
    private readonly Dictionary<string, KeyBindDescriptor> keyBinds = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册一个可重绑定按键。
    /// Registers a rebindable key binding.
    /// </summary>
    /// <param name="descriptor">按键描述。Key binding descriptor.</param>
    public void Register(KeyBindDescriptor descriptor)
    {
        keyBinds[KeyOf(descriptor)] = descriptor;
    }

    /// <summary>
    /// 获取所有按键声明。
    /// Gets all key binding declarations.
    /// </summary>
    /// <returns>按键描述快照。Snapshot of key binding descriptors.</returns>
    public IReadOnlyCollection<KeyBindDescriptor> GetAll()
    {
        return keyBinds.Values;
    }

    private static string KeyOf(KeyBindDescriptor descriptor)
    {
        return descriptor.OwnerModGuid + "\u001f" + descriptor.Id;
    }
}
