namespace DSPCore;

/// <summary>
/// 可选联机适配器实现的发送桥。
/// Send bridge implemented by an optional multiplayer adapter.
/// </summary>
public interface IMultiplayerTransport
{
    /// <summary>
    /// 发送普通 DSPCore packet。
    /// Sends a regular DSPCore packet.
    /// </summary>
    /// <param name="packetId">packet 稳定 ID。Stable packet id.</param>
    /// <param name="payload">负载。Payload.</param>
    /// <param name="target">发送目标。Send target.</param>
    /// <param name="targetId">目标 ID。Target id.</param>
    bool SendPacket(string packetId, byte[] payload, MultiplayerSendTarget target, int targetId);

    /// <summary>
    /// 发送需要主机处理的 DSPCore packet。
    /// Sends a DSPCore packet that should be handled by the host.
    /// </summary>
    /// <param name="packetId">packet 稳定 ID。Stable packet id.</param>
    /// <param name="payload">负载。Payload.</param>
    bool SendHostRelay(string packetId, byte[] payload);

    /// <summary>
    /// 请求主机导出某个星球的数据。
    /// Requests planet data export from the host.
    /// </summary>
    /// <param name="requestId">请求稳定 ID。Stable request id.</param>
    /// <param name="planetId">星球 ID。Planet id.</param>
    bool RequestPlanetData(string requestId, int planetId);
}
