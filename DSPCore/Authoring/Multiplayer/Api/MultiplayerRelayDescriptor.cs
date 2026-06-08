using System;

namespace DSPCore;

/// <summary>
/// 描述一个主机转发 packet 边界。
/// Describes a host relay packet boundary.
/// </summary>
/// <param name="PacketId">packet 稳定 ID。Stable packet ID.</param>
/// <param name="OwnerModGuid">所属模组 GUID。Owner mod GUID.</param>
/// <param name="HandleOnHost">主机处理回调。Host handler callback.</param>
public sealed record MultiplayerRelayDescriptor(string PacketId, string OwnerModGuid, Action<byte[]> HandleOnHost);
