using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理 DSPCore 模块化存档处理器。
/// Manages modular save handlers for DSPCore.
/// </summary>
public sealed class SaveRegistry
{
    private readonly Dictionary<string, SaveRegistration> handlers = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册一个存档处理器。
    /// Registers a save handler.
    /// </summary>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <param name="handler">存档处理器。Save handler.</param>
    /// <param name="loadOrder">加载顺序。Load order.</param>
    public void Register(string modGuid, ICoreSaveHandler handler, CoreLoadOrder loadOrder = CoreLoadOrder.Postload)
    {
        if (string.IsNullOrWhiteSpace(modGuid))
        {
            throw new ArgumentException("Mod GUID cannot be empty.", nameof(modGuid));
        }

        handlers[modGuid] = new SaveRegistration(modGuid, handler, loadOrder);
    }

    /// <summary>
    /// 注销一个存档处理器。
    /// Removes a save handler registration.
    /// </summary>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <returns>存在并移除时返回 true。Returns true when an existing registration was removed.</returns>
    public bool Remove(string modGuid)
    {
        return handlers.Remove(modGuid);
    }

    /// <summary>
    /// 获取所有存档处理器注册。
    /// Gets all save handler registrations.
    /// </summary>
    /// <returns>注册快照。Snapshot of registrations.</returns>
    public IReadOnlyCollection<SaveRegistration> GetAll()
    {
        return handlers.Values;
    }
}
