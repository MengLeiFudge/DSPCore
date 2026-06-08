using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧存档入口。
/// Author-facing save entry point.
/// </summary>
public static class Saves
{
    /// <summary>
    /// 注册一个存档处理器。
    /// Registers a save handler.
    /// </summary>
    public static void Register(string modGuid, ICoreSaveHandler handler, CoreLoadOrder loadOrder = CoreLoadOrder.Postload)
    {
        DspCore.Saves.Register(modGuid, handler, loadOrder);
    }

    /// <summary>
    /// 注销一个存档处理器。
    /// Removes a save handler registration.
    /// </summary>
    public static bool Remove(string modGuid)
    {
        return DspCore.Saves.Remove(modGuid);
    }

    /// <summary>
    /// 获取所有存档处理器注册。
    /// Gets all save handler registrations.
    /// </summary>
    public static IReadOnlyCollection<SaveRegistration> GetAll()
    {
        return DspCore.Saves.GetAll();
    }
}
