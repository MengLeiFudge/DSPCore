namespace DSPCore;

/// <summary>
/// 工厂网络查询能力的短入口。
/// Short entry point for factory network query capabilities.
/// </summary>
public static class Networks
{
    /// <summary>
    /// 注册网络查询适配器。
    /// Registers a network query adapter.
    /// </summary>
    /// <param name="descriptor">网络描述。Network descriptor.</param>
    public static void Register(NetworkDescriptor descriptor)
    {
        DspCore.Networks.Register(descriptor);
    }

    /// <summary>
    /// 尝试获取两个实体的共同网络 ID。
    /// Tries to get the common network ID of two entities.
    /// </summary>
    /// <param name="factory">星球工厂。Planet factory.</param>
    /// <param name="entityA">实体 A。Entity A.</param>
    /// <param name="entityB">实体 B。Entity B.</param>
    /// <param name="networkId">网络 ID。Network ID.</param>
    /// <returns>找到共同网络时返回 true。Returns true when a common network is found.</returns>
    public static bool TryGetCommonNetwork(PlanetFactory factory, int entityA, int entityB, out int networkId)
    {
        return DspCore.Networks.TryGetCommonNetwork(factory, entityA, entityB, out networkId);
    }

    /// <summary>
    /// 判断实体是否连接到指定网络。
    /// Determines whether an entity is connected to a network.
    /// </summary>
    /// <param name="factory">星球工厂。Planet factory.</param>
    /// <param name="entityId">实体 ID。Entity ID.</param>
    /// <param name="networkId">网络 ID。Network ID.</param>
    /// <returns>连接时返回 true。Returns true when connected.</returns>
    public static bool IsConnectedToNetwork(PlanetFactory factory, int entityId, int networkId)
    {
        return DspCore.Networks.IsConnectedToNetwork(factory, entityId, networkId);
    }
}
