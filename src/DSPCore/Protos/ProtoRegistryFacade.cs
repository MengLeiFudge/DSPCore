using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 提供新标准 Proto 注册入口，并作为旧 LDBTool/CommonAPI API 的内部目标。
/// Provides the new-standard Proto registration entry point and acts as the internal target for legacy LDBTool/CommonAPI APIs.
/// </summary>
public sealed class ProtoRegistryFacade
{
    private readonly List<ProtoRegistration> preLoadProtos = new();
    private readonly List<ProtoRegistration> postLoadProtos = new();

    /// <summary>
    /// 注册一个需要在 LDB 加载前加入的 Proto。
    /// Registers a Proto that should be added before LDB loading.
    /// </summary>
    /// <param name="protoType">Proto 类型。Proto type.</param>
    /// <param name="proto">Proto 对象。Proto object.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    public void RegisterPreload(Type protoType, object proto, string ownerModGuid)
    {
        preLoadProtos.Add(new ProtoRegistration(protoType, proto, ownerModGuid, ProtoRegistrationPhase.Preload));
    }

    /// <summary>
    /// 注册一个需要在 LDB 加载后加入的 Proto。
    /// Registers a Proto that should be added after LDB loading.
    /// </summary>
    /// <param name="protoType">Proto 类型。Proto type.</param>
    /// <param name="proto">Proto 对象。Proto object.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    public void RegisterPostload(Type protoType, object proto, string ownerModGuid)
    {
        postLoadProtos.Add(new ProtoRegistration(protoType, proto, ownerModGuid, ProtoRegistrationPhase.Postload));
    }

    /// <summary>
    /// 获取所有已注册的 Proto。
    /// Gets all registered Protos.
    /// </summary>
    /// <returns>Proto 注册快照。Snapshot of Proto registrations.</returns>
    public IReadOnlyList<ProtoRegistration> GetAll()
    {
        var result = new List<ProtoRegistration>(preLoadProtos.Count + postLoadProtos.Count);
        result.AddRange(preLoadProtos);
        result.AddRange(postLoadProtos);
        return result;
    }
}
