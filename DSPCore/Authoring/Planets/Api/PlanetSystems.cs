namespace DSPCore;

/// <summary>
/// 星球系统能力的短入口。
/// Short entry point for planet system capabilities.
/// </summary>
public static class PlanetSystems
{
    /// <summary>
    /// 注册星球系统描述。
    /// Registers a planet system descriptor.
    /// </summary>
    /// <param name="descriptor">系统描述。System descriptor.</param>
    public static void Register(PlanetSystemDescriptor descriptor)
    {
        DspCore.PlanetSystems.Register(descriptor);
    }

    /// <summary>
    /// 尝试获取指定星球工厂上的系统实例。
    /// Tries to get a system instance on a planet factory.
    /// </summary>
    /// <typeparam name="T">系统类型。System type.</typeparam>
    /// <param name="factory">星球工厂。Planet factory.</param>
    /// <param name="systemId">系统 ID。System ID.</param>
    /// <param name="system">系统实例。System instance.</param>
    /// <returns>存在并类型匹配时返回 true。Returns true when present and type matched.</returns>
    public static bool TryGet<T>(PlanetFactory factory, string systemId, out T system)
        where T : CorePlanetSystem
    {
        return PlanetLifecycleRuntime.TryGet(factory, systemId, out system);
    }
}
