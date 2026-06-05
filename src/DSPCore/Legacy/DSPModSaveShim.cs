using System.IO;

namespace crecheng.DSPModSave;

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
/// 旧 DSPModSave 插件入口常量和手动注册入口；请迁移到 DSPCore.DspCore.Saves。
/// Legacy DSPModSave plugin constants and manual registration entry point; migrate to DSPCore.DspCore.Saves.
/// </summary>
[System.Obsolete("Use DSPCore.DspCore.Saves instead.")]
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
    [System.Obsolete("Use DSPCore.DspCore.Saves.Register(modGuid, handler) instead.")]
    public static void AddModSaveManually(object mod)
    {
    }
}
