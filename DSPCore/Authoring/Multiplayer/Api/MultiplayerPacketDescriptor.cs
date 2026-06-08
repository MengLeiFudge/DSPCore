using System;

namespace DSPCore;

/// <summary>
/// 描述一个可选联机 packet 声明。
/// Describes an optional multiplayer packet declaration.
/// </summary>
/// <param name="PacketId">packet 稳定 ID。Stable packet ID.</param>
/// <param name="OwnerModGuid">所属模组 GUID。Owner mod GUID.</param>
/// <param name="Handler">本地处理回调。Local handler callback.</param>
public sealed record MultiplayerPacketDescriptor(string PacketId, string OwnerModGuid, Action<byte[]> Handler);
