using System.IO;

namespace DSPCore;

/// <summary>
/// DSPCore 托管的星球工厂实体组件基类。
/// Base class for DSPCore-managed planet factory entity components.
/// </summary>
public abstract class CoreFactoryComponent
{
    /// <summary>
    /// 初始化组件所属实体上下文。
    /// Initializes the owning entity context.
    /// </summary>
    /// <param name="componentId">组件 ID。Component ID.</param>
    /// <param name="factory">星球工厂。Planet factory.</param>
    /// <param name="entityId">实体 ID。Entity ID.</param>
    public void InitializeContext(string componentId, PlanetFactory factory, int entityId)
    {
        ComponentId = componentId;
        Factory = factory;
        EntityId = entityId;
    }

    /// <summary>
    /// 组件稳定 ID。
    /// Stable component ID.
    /// </summary>
    public string ComponentId { get; private set; } = string.Empty;

    /// <summary>
    /// 所属星球工厂。
    /// Owning planet factory.
    /// </summary>
    public PlanetFactory? Factory { get; private set; }

    /// <summary>
    /// 所属实体 ID。
    /// Owning entity ID.
    /// </summary>
    public int EntityId { get; private set; }

    /// <summary>
    /// 组件挂到实体后调用。
    /// Called after the component is attached to an entity.
    /// </summary>
    /// <param name="prebuildId">来源预建筑 ID。Source prebuild ID.</param>
    public virtual void OnAdded(int prebuildId)
    {
    }

    /// <summary>
    /// 实体移除前调用。
    /// Called before the entity is removed.
    /// </summary>
    public virtual void OnRemoved()
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
    /// 写出组件存档数据。
    /// Writes component save data.
    /// </summary>
    /// <param name="writer">二进制写入器。Binary writer.</param>
    public virtual void Export(BinaryWriter writer)
    {
    }

    /// <summary>
    /// 读取组件存档数据。
    /// Reads component save data.
    /// </summary>
    /// <param name="reader">二进制读取器。Binary reader.</param>
    public virtual void Import(BinaryReader reader)
    {
    }

    /// <summary>
    /// 当前游戏存档没有组件数据时调用。
    /// Called when the current game save has no component data.
    /// </summary>
    public virtual void IntoOtherSave()
    {
    }
}
