using System;

namespace DSPCore;

/// <summary>
/// 描述一个工厂网络查询适配器。
/// Describes a factory network query adapter.
/// </summary>
/// <param name="NetworkId">网络适配器 ID。Network adapter ID.</param>
/// <param name="OwnerModGuid">所属模组 GUID。Owner mod GUID.</param>
/// <param name="TryGetCommonNetwork">查询两个实体共同网络。Queries the common network of two entities.</param>
/// <param name="IsConnectedToNetwork">查询实体是否连接到指定网络。Queries whether an entity is connected to a network.</param>
public sealed record NetworkDescriptor(
    string NetworkId,
    string OwnerModGuid,
    Func<PlanetFactory, int, int, int?> TryGetCommonNetwork,
    Func<PlanetFactory, int, int, bool>? IsConnectedToNetwork = null);
