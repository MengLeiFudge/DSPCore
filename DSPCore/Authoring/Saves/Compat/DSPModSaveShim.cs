using System.IO;

namespace crecheng.DSPModSave;

/// <summary>
/// 旧 DSPModSave 加载顺序枚举；请迁移到 DSPCore.CoreLoadOrder。
/// Legacy DSPModSave load-order enum; migrate to DSPCore.CoreLoadOrder.
/// </summary>
[System.Obsolete("Use DSPCore.CoreLoadOrder instead.")]
public enum LoadOrder
{
    /// <summary>
    /// 在游戏存档加载前导入。
    /// Imports before the game save finishes loading.
    /// </summary>
    Preload = 0,

    /// <summary>
    /// 在游戏存档加载后导入。
    /// Imports after the game save finishes loading.
    /// </summary>
    Postload = 1
}

/// <summary>
/// 旧 DSPModSave 存档设置特性；请迁移到 DSPCore.Saves.Register 的 loadOrder 参数。
/// Legacy DSPModSave save settings attribute; migrate to the loadOrder parameter of DSPCore.Saves.Register.
/// </summary>
/// <param name="loadOrder">加载顺序。Load order.</param>
[System.Obsolete("Use DSPCore.Saves.Register(modGuid, handler, loadOrder) instead.")]
[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class ModSaveSettingsAttribute(LoadOrder loadOrder) : System.Attribute
{
    /// <summary>
    /// 旧加载顺序。
    /// Legacy load order.
    /// </summary>
    public LoadOrder LoadOrder { get; } = loadOrder;
}

/// <summary>
/// 旧 DSPModSave 存档接口；请迁移到 DSPCore.ICoreSaveHandler。
/// Legacy DSPModSave save interface; migrate to DSPCore.ICoreSaveHandler.
/// </summary>
[System.Obsolete("Use DSPCore.ICoreSaveHandler instead. This interface is kept for compatibility.")]
public interface IModCanSave
{
    /// <summary>
    /// 导出存档数据。
    /// Exports save data.
    /// </summary>
    /// <param name="w">二进制写入器。Binary writer.</param>
    void Export(BinaryWriter w);

    /// <summary>
    /// 导入存档数据。
    /// Imports save data.
    /// </summary>
    /// <param name="r">二进制读取器。Binary reader.</param>
    void Import(BinaryReader r);

    /// <summary>
    /// 当没有对应存档数据时初始化。
    /// Initializes when no matching save data exists.
    /// </summary>
    void IntoOtherSave();
}

/// <summary>
/// 旧 DSPModSave 插件入口常量和手动注册入口；请迁移到 DSPCore.Saves。
/// Legacy DSPModSave plugin constants and manual registration entry point; migrate to DSPCore.Saves.
/// </summary>
[System.Obsolete("Use DSPCore.Saves instead.")]
public static class DSPModSavePlugin
{
    /// <summary>
    /// 旧 DSPModSave GUID。
    /// Legacy DSPModSave GUID.
    /// </summary>
    public const string MODGUID = "crecheng.DSPModSave";

    /// <summary>
    /// 兼容保留的手动注册入口；初版仅保留签名。
    /// Compatibility-only manual registration entry point; first version keeps the signature only.
    /// </summary>
    /// <param name="mod">旧模组实例。Legacy mod instance.</param>
    [System.Obsolete("Use DSPCore.Saves.Register(modGuid, handler) instead.")]
    public static void AddModSaveManually(object mod)
    {
        DSPCore.SaveRuntime.RegisterLegacyHandler(mod);
    }
}
