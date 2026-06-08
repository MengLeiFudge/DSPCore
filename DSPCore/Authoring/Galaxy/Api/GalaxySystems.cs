namespace DSPCore;

/// <summary>
/// 恒星和银河系统能力的短入口。
/// Short entry point for star and galaxy system capabilities.
/// </summary>
public static class GalaxySystems
{
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
    /// 注册银河级系统。
    /// Registers a galaxy-level system.
    /// </summary>
    /// <param name="descriptor">银河系统描述。Galaxy system descriptor.</param>
    public static void RegisterGalaxy(GalaxySystemDescriptor descriptor)
    {
        DspCore.GalaxySystems.Register(descriptor);
    }
}
