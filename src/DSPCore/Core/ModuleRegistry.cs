using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理 DSPCore 模块的声明、依赖和初始化状态。
/// Manages DSPCore module declarations, dependencies, and initialization state.
/// </summary>
public sealed class ModuleRegistry
{
    private readonly Dictionary<string, ModuleDescriptor> modules = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册一个模块描述。
    /// Registers a module descriptor.
    /// </summary>
    /// <param name="descriptor">模块描述。Module descriptor.</param>
    public void Register(ModuleDescriptor descriptor)
    {
        if (string.IsNullOrWhiteSpace(descriptor.Id))
        {
            throw new ArgumentException("Module id cannot be empty.", nameof(descriptor));
        }

        modules[descriptor.Id] = descriptor;
    }

    /// <summary>
    /// 尝试按模块 ID 获取模块描述。
    /// Tries to get a module descriptor by module id.
    /// </summary>
    /// <param name="id">模块 ID。Module id.</param>
    /// <param name="descriptor">返回的模块描述。Returned module descriptor.</param>
    /// <returns>找到模块时返回 true。Returns true when the module exists.</returns>
    public bool TryGet(string id, out ModuleDescriptor descriptor)
    {
        return modules.TryGetValue(id, out descriptor!);
    }

    /// <summary>
    /// 获取所有已注册模块。
    /// Gets all registered modules.
    /// </summary>
    /// <returns>模块描述快照。Snapshot of module descriptors.</returns>
    public IReadOnlyCollection<ModuleDescriptor> GetAll()
    {
        return modules.Values;
    }
}
