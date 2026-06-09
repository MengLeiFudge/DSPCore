using System;
using System.Collections.Generic;
using System.IO;

namespace DSPCore;

/// <summary>
/// 作者侧存档入口。
/// Author-facing save entry point.
/// </summary>
public static class Saves
{
    /// <summary>
    /// 创建并用自动 schema 注册一个简单状态对象。
    /// Creates and registers a simple state object with an automatic schema.
    /// </summary>
    /// <typeparam name="TState">状态对象类型。State object type.</typeparam>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <param name="version">schema 版本。Schema version.</param>
    /// <param name="migrate">导入旧版本后的迁移回调。Migration callback after importing an older version.</param>
    /// <param name="intoOtherSave">没有对应数据时的初始化回调。Initialization callback when no matching data exists.</param>
    /// <param name="loadOrder">加载顺序。Load order.</param>
    /// <returns>创建的状态对象，便于内联保存。The created state object for inline storage.</returns>
    public static TState Auto<TState>(
        string modGuid,
        int version = 1,
        Action<int, TState>? migrate = null,
        Action<TState>? intoOtherSave = null,
        CoreLoadOrder loadOrder = CoreLoadOrder.Postload)
        where TState : class, new()
    {
        return Auto(modGuid, new TState(), version, migrate, intoOtherSave, loadOrder);
    }

    /// <summary>
    /// 用自动 schema 注册一个简单状态对象。
    /// Registers a simple state object with an automatic schema.
    /// </summary>
    /// <typeparam name="TState">状态对象类型。State object type.</typeparam>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <param name="state">状态对象实例。State object instance.</param>
    /// <param name="version">schema 版本。Schema version.</param>
    /// <param name="migrate">导入旧版本后的迁移回调。Migration callback after importing an older version.</param>
    /// <param name="intoOtherSave">没有对应数据时的初始化回调。Initialization callback when no matching data exists.</param>
    /// <param name="loadOrder">加载顺序。Load order.</param>
    /// <returns>原状态对象，便于内联保存。The original state object for inline storage.</returns>
    public static TState Auto<TState>(
        string modGuid,
        TState state,
        int version = 1,
        Action<int, TState>? migrate = null,
        Action<TState>? intoOtherSave = null,
        CoreLoadOrder loadOrder = CoreLoadOrder.Postload)
        where TState : class
    {
        DspCore.Saves.Register(modGuid, new AutoSaveHandler<TState>(modGuid, state, version, migrate, intoOtherSave), loadOrder);
        return state;
    }

    /// <summary>
    /// 用委托注册一个简单存档处理器。
    /// Registers a simple save handler from delegates.
    /// </summary>
    /// <param name="modGuid">模组 GUID。Mod GUID.</param>
    /// <param name="export">写出存档数据的回调。Callback that writes save data.</param>
    /// <param name="import">读入存档数据的回调。Callback that reads save data.</param>
    /// <param name="intoOtherSave">没有对应数据时的初始化回调。Initialization callback when no matching data exists.</param>
    /// <param name="loadOrder">加载顺序。Load order.</param>
    public static void Register(
        string modGuid,
        Action<BinaryWriter> export,
        Action<BinaryReader> import,
        Action? intoOtherSave = null,
        CoreLoadOrder loadOrder = CoreLoadOrder.Postload)
    {
        DspCore.Saves.Register(modGuid, new DelegateSaveHandler(export, import, intoOtherSave), loadOrder);
    }

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

    private sealed class DelegateSaveHandler : ICoreSaveHandler
    {
        private readonly Action<BinaryWriter> export;
        private readonly Action<BinaryReader> import;
        private readonly Action? intoOtherSave;

        public DelegateSaveHandler(Action<BinaryWriter> export, Action<BinaryReader> import, Action? intoOtherSave)
        {
            this.export = export ?? throw new ArgumentNullException(nameof(export));
            this.import = import ?? throw new ArgumentNullException(nameof(import));
            this.intoOtherSave = intoOtherSave;
        }

        public void Export(BinaryWriter writer)
        {
            export(writer);
        }

        public void Import(BinaryReader reader)
        {
            import(reader);
        }

        public void IntoOtherSave()
        {
            intoOtherSave?.Invoke();
        }
    }
}
