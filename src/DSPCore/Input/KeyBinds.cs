using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧按键绑定入口。
/// Author-facing key binding entry point.
/// </summary>
public static class KeyBinds
{
    /// <summary>
    /// 注册一个可重绑定按键。
    /// Registers a rebindable key binding.
    /// </summary>
    public static void Register(KeyBindDescriptor descriptor)
    {
        DspCore.KeyBinds.Register(descriptor);
    }

    /// <summary>
    /// 获取所有按键声明。
    /// Gets all key binding declarations.
    /// </summary>
    public static IReadOnlyCollection<KeyBindDescriptor> GetAll()
    {
        return DspCore.KeyBinds.GetAll();
    }
}
