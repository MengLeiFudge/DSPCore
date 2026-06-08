using System.IO;

namespace DSPCore;

/// <summary>
/// DSPCore 托管的恒星级系统基类。
/// Base class for DSPCore-managed star-level systems.
/// </summary>
public abstract class CoreStarSystem
{
    /// <summary>
    /// 初始化恒星系统上下文。
    /// Initializes the star system context.
    /// </summary>
    /// <param name="systemId">系统 ID。System ID.</param>
    /// <param name="star">恒星数据。Star data.</param>
    public void InitializeContext(string systemId, StarData star)
    {
        SystemId = systemId;
        Star = star;
    }

    /// <summary>
    /// 系统稳定 ID。
    /// Stable system ID.
    /// </summary>
    public string SystemId { get; private set; } = string.Empty;

    /// <summary>
    /// 所属恒星。
    /// Owning star.
    /// </summary>
    public StarData? Star { get; private set; }

    /// <summary>
    /// 初始化时调用。
    /// Called during initialization.
    /// </summary>
    public virtual void Init() { }

    /// <summary>
    /// 每个太空扇区 tick 调用。
    /// Called on each space sector tick.
    /// </summary>
    /// <param name="time">游戏 tick。Game tick.</param>
    public virtual void Update(long time) { }

    /// <summary>
    /// 写出系统存档数据。
    /// Writes system save data.
    /// </summary>
    /// <param name="writer">二进制写入器。Binary writer.</param>
    public virtual void Export(BinaryWriter writer) { }

    /// <summary>
    /// 读取系统存档数据。
    /// Reads system save data.
    /// </summary>
    /// <param name="reader">二进制读取器。Binary reader.</param>
    public virtual void Import(BinaryReader reader) { }

    /// <summary>
    /// 当前游戏存档没有系统数据时调用。
    /// Called when the current game save has no system data.
    /// </summary>
    public virtual void IntoOtherSave() { }
}
