using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理工厂网络查询适配器。
/// Manages factory network query adapters.
/// </summary>
public sealed class NetworkRegistry
{
    private readonly Dictionary<string, NetworkDescriptor> descriptors = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册或替换一个网络查询适配器。
    /// Registers or replaces a network query adapter.
    /// </summary>
    /// <param name="descriptor">网络描述。Network descriptor.</param>
    public void Register(NetworkDescriptor descriptor)
    {
        descriptors[descriptor.NetworkId] = descriptor;
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
    public bool TryGetCommonNetwork(PlanetFactory factory, int entityA, int entityB, out int networkId)
    {
        foreach (var descriptor in descriptors.Values)
        {
            try
            {
                var result = descriptor.TryGetCommonNetwork(factory, entityA, entityB);
                if (result.HasValue)
                {
                    networkId = result.Value;
                    return true;
                }
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            }
        }

        networkId = 0;
        return false;
    }

    /// <summary>
    /// 判断实体是否连接到指定网络。
    /// Determines whether an entity is connected to a network.
    /// </summary>
    /// <param name="factory">星球工厂。Planet factory.</param>
    /// <param name="entityId">实体 ID。Entity ID.</param>
    /// <param name="networkId">网络 ID。Network ID.</param>
    /// <returns>连接时返回 true。Returns true when connected.</returns>
    public bool IsConnectedToNetwork(PlanetFactory factory, int entityId, int networkId)
    {
        foreach (var descriptor in descriptors.Values)
        {
            if (descriptor.IsConnectedToNetwork == null)
            {
                continue;
            }

            try
            {
                if (descriptor.IsConnectedToNetwork(factory, entityId, networkId))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                DspCore.Errors.ReportException(descriptor.OwnerModGuid, ex);
            }
        }

        return false;
    }
}
