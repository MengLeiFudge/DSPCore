namespace DSPCore;

/// <summary>
/// 恒星和银河系统能力的短入口。
/// Short entry point for star and galaxy system capabilities.
/// </summary>
public static class GalaxySystems
{
    /// <summary>
    /// 注册一个带无参构造函数的恒星级系统类型。
    /// Registers a star-level system type with a parameterless constructor.
    /// </summary>
    /// <typeparam name="TSystem">系统类型。System type.</typeparam>
    /// <param name="systemId">系统稳定 ID。Stable system ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    public static void RegisterStar<TSystem>(string systemId, string ownerModGuid)
        where TSystem : CoreStarSystem, new()
    {
        RegisterStar(new StarSystemDescriptor(systemId, ownerModGuid, static star => new TSystem()));
    }

    /// <summary>
    /// 注册恒星级系统。
    /// Registers a star-level system.
    /// </summary>
    /// <param name="descriptor">恒星系统描述。Star system descriptor.</param>
    public static void RegisterStar(StarSystemDescriptor descriptor)
    {
        DspCore.StarSystems.Register(descriptor);
    }

    /// <summary>
    /// 注册一个带无参构造函数的银河级系统类型。
    /// Registers a galaxy-level system type with a parameterless constructor.
    /// </summary>
    /// <typeparam name="TSystem">系统类型。System type.</typeparam>
    /// <param name="systemId">系统稳定 ID。Stable system ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    public static void RegisterGalaxy<TSystem>(string systemId, string ownerModGuid)
        where TSystem : CoreGalaxySystem, new()
    {
        RegisterGalaxy(new GalaxySystemDescriptor(systemId, ownerModGuid, static galaxy => new TSystem()));
    }

    /// <summary>
    /// 注册银河级系统。
    /// Registers a galaxy-level system.
    /// </summary>
    /// <param name="descriptor">银河系统描述。Galaxy system descriptor.</param>
    public static void RegisterGalaxy(GalaxySystemDescriptor descriptor)
    {
        DspCore.GalaxySystems.Register(descriptor);
    }
}
