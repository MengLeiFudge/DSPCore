using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 提供新标准 Proto 注册入口，并作为旧 LDBTool/CommonAPI API 的内部目标。
/// Provides the new-standard Proto registration entry point and acts as the internal target for legacy LDBTool/CommonAPI APIs.
/// </summary>
public sealed class ProtoRegistryFacade
{
    private readonly List<ProtoRegistrationEntry> registrations = new();

    /// <summary>
    /// 注册一个原型对象。
    /// Registers a proto object.
    /// </summary>
    /// <param name="protoType">Proto 类型。Proto type.</param>
    /// <param name="proto">Proto 对象。Proto object.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="kind">原型功能类型。Proto feature kind.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    public void Register(Type protoType, object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, ProtoKind kind = ProtoKind.Unknown, string? purpose = null)
    {
        registrations.Add(new ProtoRegistrationEntry(protoType, proto, ownerModGuid, phase, kind, purpose));
    }

    /// <summary>
    /// 注册一个物品原型。
    /// Registers an item proto.
    /// </summary>
    /// <param name="proto">物品 Proto 对象。Item proto object.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    public void RegisterItem(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        Register(proto.GetType(), proto, ownerModGuid, phase, ProtoKind.Item, purpose);
    }

    /// <summary>
    /// 注册一个配方原型。
    /// Registers a recipe proto.
    /// </summary>
    /// <param name="proto">配方 Proto 对象。Recipe proto object.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    public void RegisterRecipe(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        Register(proto.GetType(), proto, ownerModGuid, phase, ProtoKind.Recipe, purpose);
    }

    /// <summary>
    /// 注册一个科技原型。
    /// Registers a tech proto.
    /// </summary>
    /// <param name="proto">科技 Proto 对象。Tech proto object.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    public void RegisterTech(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        Register(proto.GetType(), proto, ownerModGuid, phase, ProtoKind.Tech, purpose);
    }

    /// <summary>
    /// 注册一个指引或教程原型。
    /// Registers a guide or tutorial proto.
    /// </summary>
    /// <param name="proto">指引或教程 Proto 对象。Guide or tutorial proto object.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    public void RegisterTutorial(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        Register(proto.GetType(), proto, ownerModGuid, phase, ProtoKind.Tutorial, purpose);
    }

    /// <summary>
    /// 旧 PreAdd 兼容入口，后续可能无法等价映射到 DSPCore 新阶段。
    /// Legacy PreAdd bridge; it may not map exactly to DSPCore's new phases later.
    /// </summary>
    /// <param name="protoType">Proto 类型。Proto type.</param>
    /// <param name="proto">Proto 对象。Proto object.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    [Obsolete("Use Register(..., CoreDataPhase.Data, ...) or a typed RegisterItem/RegisterRecipe/RegisterTech method instead.")]
    public void RegisterPreload(Type protoType, object proto, string ownerModGuid)
    {
        Register(protoType, proto, ownerModGuid, CoreDataPhase.Data, ProtoKind.Unknown, "Legacy PreAddProto bridge");
    }

    /// <summary>
    /// 旧 PostAdd 兼容入口，后续可能无法等价映射到 DSPCore 新阶段。
    /// Legacy PostAdd bridge; it may not map exactly to DSPCore's new phases later.
    /// </summary>
    /// <param name="protoType">Proto 类型。Proto type.</param>
    /// <param name="proto">Proto 对象。Proto object.</param>
    /// <param name="ownerModGuid">声明方模组 GUID。Declaring mod GUID.</param>
    [Obsolete("Use Register(..., CoreDataPhase.DataFinalFixes, ...) or a typed RegisterItem/RegisterRecipe/RegisterTech method instead.")]
    public void RegisterPostload(Type protoType, object proto, string ownerModGuid)
    {
        Register(protoType, proto, ownerModGuid, CoreDataPhase.DataFinalFixes, ProtoKind.Unknown, "Legacy PostAddProto bridge");
    }

    /// <summary>
    /// 获取指定阶段的原型注册。
    /// Gets proto registrations for a phase.
    /// </summary>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <returns>原型注册快照。Snapshot of proto registrations.</returns>
    public IReadOnlyList<ProtoRegistrationEntry> GetByPhase(CoreDataPhase phase)
    {
        return registrations.FindAll(item => item.Phase == phase).ToArray();
    }

    /// <summary>
    /// 获取所有已注册的原型注册项。
    /// Gets all registered proto registration entries.
    /// </summary>
    /// <returns>原型注册项快照。Snapshot of proto registration entries.</returns>
    public IReadOnlyList<ProtoRegistrationEntry> GetAll()
    {
        return registrations.ToArray();
    }
}
