namespace DSPCore;

/// <summary>
/// 描述联机 packet 的发送目标。
/// Describes the target for a multiplayer packet send.
/// </summary>
public enum MultiplayerSendTarget
{
    /// <summary>
    /// 交给适配器默认广播或默认发送策略。
    /// Uses the adapter's default broadcast or send behavior.
    /// </summary>
    All,

    /// <summary>
    /// 发送给主机。
    /// Sends to the host.
    /// </summary>
    Host,

    /// <summary>
    /// 发送给本地星球。
    /// Sends to the local planet.
    /// </summary>
    LocalPlanet,

    /// <summary>
    /// 发送给本地恒星。
    /// Sends to the local star.
    /// </summary>
    LocalStar,

    /// <summary>
    /// 发送给指定星球，目标 ID 使用星球 ID。
    /// Sends to a specific planet; target id is the planet id.
    /// </summary>
    Planet,

    /// <summary>
    /// 发送给指定恒星，目标 ID 使用恒星 ID。
    /// Sends to a specific star; target id is the star id.
    /// </summary>
    Star
}
