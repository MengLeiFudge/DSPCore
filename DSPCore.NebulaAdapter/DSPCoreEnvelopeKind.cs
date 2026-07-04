namespace DSPCore.NebulaAdapter;

/// <summary>
/// DSPCore Nebula 信封 packet 的业务类型。
/// Business kind carried by a DSPCore Nebula envelope packet.
/// </summary>
public enum DSPCoreEnvelopeKind
{
    /// <summary>
    /// 普通作者 packet。
    /// Regular author packet.
    /// </summary>
    Packet = 0,

    /// <summary>
    /// 需要主机处理的 packet。
    /// Packet that should be handled by the host.
    /// </summary>
    HostRelay = 1,

    /// <summary>
    /// 星球数据请求。
    /// Planet data request.
    /// </summary>
    PlanetDataRequest = 2,

    /// <summary>
    /// 星球数据响应。
    /// Planet data response.
    /// </summary>
    PlanetDataResponse = 3
}
