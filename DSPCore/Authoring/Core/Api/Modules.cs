using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧模块声明入口。
/// Author-facing module declaration entry point.
/// </summary>
public static class Modules
{
    /// <summary>
    /// 注册一个模块。
    /// Registers a module.
    /// </summary>
    /// <param name="id">模块 ID。Module id.</param>
    /// <param name="displayName">显示名称。Display name.</param>
    /// <param name="initialize">初始化回调。Initialization callback.</param>
    /// <param name="dependencies">模块依赖 ID。Module dependency ids.</param>
    public static void Register(
        string id,
        string displayName,
        Action initialize,
        IReadOnlyList<string>? dependencies = null)
    {
        Register(new ModuleDescriptor(id, displayName, initialize, dependencies));
    }

    /// <summary>
    /// 注册一个模块描述。
    /// Registers a module descriptor.
    /// </summary>
    public static void Register(ModuleDescriptor descriptor)
    {
        DspCore.Modules.Register(descriptor);
    }

    /// <summary>
    /// 尝试按模块 ID 获取模块描述。
    /// Tries to get a module descriptor by module id.
    /// </summary>
    public static bool TryGet(string id, out ModuleDescriptor descriptor)
    {
        return DspCore.Modules.TryGet(id, out descriptor);
    }

    /// <summary>
    /// 获取所有已注册模块。
    /// Gets all registered modules.
    /// </summary>
    public static IReadOnlyCollection<ModuleDescriptor> GetAll()
    {
        return DspCore.Modules.GetAll();
    }
}
