using System.IO;

namespace DSPCore;

/// <summary>
/// DSPCore 托管的银河级系统基类。
/// Base class for DSPCore-managed galaxy-level systems.
/// </summary>
public abstract class CoreGalaxySystem
{
    /// <summary>
    /// 初始化银河系统上下文。
    /// Initializes the galaxy system context.
    /// </summary>
    /// <param name="systemId">系统 ID。System ID.</param>
    /// <param name="galaxy">银河数据。Galaxy data.</param>
    public void InitializeContext(string systemId, GalaxyData galaxy)
    {
        SystemId = systemId;
        Galaxy = galaxy;
    }

    /// <summary>
    /// 系统稳定 ID。
    /// Stable system ID.
    /// </summary>
    public string SystemId { get; private set; } = string.Empty;

    /// <summary>
    /// 所属银河。
    /// Owning galaxy.
    /// </summary>
    public GalaxyData? Galaxy { get; private set; }

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
