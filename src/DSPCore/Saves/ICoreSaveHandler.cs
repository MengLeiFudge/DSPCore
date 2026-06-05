using System.IO;

namespace DSPCore;

/// <summary>
/// DSPCore 新标准存档处理接口。
/// New-standard save handler interface for DSPCore.
/// </summary>
public interface ICoreSaveHandler
{
    /// <summary>
    /// 将模块数据写入二进制流。
    /// Writes module data to a binary stream.
    /// </summary>
    /// <param name="writer">二进制写入器。Binary writer.</param>
    void Export(BinaryWriter writer);

    /// <summary>
    /// 从二进制流读取模块数据。
    /// Reads module data from a binary stream.
    /// </summary>
    /// <param name="reader">二进制读取器。Binary reader.</param>
    void Import(BinaryReader reader);

    /// <summary>
    /// 当当前游戏存档没有对应模块数据时初始化模块状态。
    /// Initializes module state when the current game save has no matching module data.
    /// </summary>
    void IntoOtherSave();
}
