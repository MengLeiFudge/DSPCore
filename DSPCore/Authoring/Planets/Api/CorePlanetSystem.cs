using System.IO;

namespace DSPCore;

/// <summary>
/// DSPCore 托管的星球级系统基类。
/// Base class for DSPCore-managed planet-level systems.
/// </summary>
public abstract class CorePlanetSystem
{
    /// <summary>
    /// 初始化系统所属星球上下文。
    /// Initializes the owning planet context.
    /// </summary>
    /// <param name="systemId">系统 ID。System ID.</param>
    /// <param name="factory">星球工厂。Planet factory.</param>
    public void InitializeContext(string systemId, PlanetFactory factory)
    {
        SystemId = systemId;
        Factory = factory;
    }

    /// <summary>
    /// 系统稳定 ID。
    /// Stable system ID.
    /// </summary>
    public string SystemId { get; private set; } = string.Empty;

    /// <summary>
    /// 所属星球工厂。
    /// Owning planet factory.
    /// </summary>
    public PlanetFactory? Factory { get; private set; }

    /// <summary>
    /// 系统初始化时调用。
    /// Called when the system is initialized.
    /// </summary>
    public virtual void Init()
    {
    }

    /// <summary>
    /// 系统释放时调用。
    /// Called when the system is freed.
    /// </summary>
    public virtual void Free()
    {
    }

    /// <summary>
    /// 本地星球渲染阶段调用。
    /// Called during local planet rendering.
    /// </summary>
    public virtual void DrawUpdate()
    {
    }

    /// <summary>
    /// 电力阶段更新。
    /// Updates during the power phase.
    /// </summary>
    /// <param name="time">游戏 tick。Game tick.</param>
    /// <param name="isActive">工厂是否活跃。Whether the factory is active.</param>
    public virtual void PowerUpdate(long time, bool isActive)
    {
    }

    /// <summary>
    /// 工厂逻辑前置阶段更新。
    /// Updates during the pre factory logic phase.
    /// </summary>
    /// <param name="time">游戏 tick。Game tick.</param>
    /// <param name="isActive">工厂是否活跃。Whether the factory is active.</param>
    public virtual void PreUpdate(long time, bool isActive)
    {
    }

    /// <summary>
    /// 工厂逻辑阶段更新。
    /// Updates during the factory logic phase.
    /// </summary>
    /// <param name="time">游戏 tick。Game tick.</param>
    /// <param name="isActive">工厂是否活跃。Whether the factory is active.</param>
    public virtual void Update(long time, bool isActive)
    {
    }

    /// <summary>
    /// 星球工厂后置阶段更新。
    /// Updates during the planet factory post phase.
    /// </summary>
    /// <param name="time">游戏 tick。Game tick.</param>
    public virtual void PostUpdate(long time)
    {
    }

    /// <summary>
    /// 写出系统存档数据。
    /// Writes system save data.
    /// </summary>
    /// <param name="writer">二进制写入器。Binary writer.</param>
    public virtual void Export(BinaryWriter writer)
    {
    }

    /// <summary>
    /// 读取系统存档数据。
    /// Reads system save data.
    /// </summary>
    /// <param name="reader">二进制读取器。Binary reader.</param>
    public virtual void Import(BinaryReader reader)
    {
    }

    /// <summary>
    /// 当前游戏存档没有系统数据时调用。
    /// Called when the current game save has no system data.
    /// </summary>
    public virtual void IntoOtherSave()
    {
    }
}
