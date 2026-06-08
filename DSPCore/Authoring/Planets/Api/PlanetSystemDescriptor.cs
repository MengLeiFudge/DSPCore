using System;

namespace DSPCore;

/// <summary>
/// 描述一个由 DSPCore 托管的星球系统。
/// Describes a planet system managed by DSPCore.
/// </summary>
public sealed class PlanetSystemDescriptor
{
    /// <summary>
    /// 创建星球系统描述。
    /// Creates a planet system descriptor.
    /// </summary>
    /// <param name="systemId">系统稳定 ID。Stable system ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="factory">系统工厂。System factory.</param>
    public PlanetSystemDescriptor(string systemId, string ownerModGuid, Func<PlanetFactory, CorePlanetSystem> factory)
    {
        SystemId = systemId;
        OwnerModGuid = ownerModGuid;
        Factory = factory;
    }

    /// <summary>
    /// 系统稳定 ID，必须在存档中保持不变。
    /// Stable system ID that must remain unchanged in saves.
    /// </summary>
    public string SystemId { get; }

    /// <summary>
    /// 注册该系统的模组 GUID。
    /// GUID of the mod that registered this system.
    /// </summary>
    public string OwnerModGuid { get; }

    /// <summary>
    /// 创建系统实例的工厂函数。
    /// Factory function that creates a system instance.
    /// </summary>
    public Func<PlanetFactory, CorePlanetSystem> Factory { get; }
}
