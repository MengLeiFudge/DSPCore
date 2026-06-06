using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧 Proto 注册入口。
/// Author-facing Proto registration entry point.
/// </summary>
public static class Protos
{
    /// <summary>
    /// 注册一个原型对象。
    /// Registers a proto object.
    /// </summary>
    public static void Register(Type protoType, object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, ProtoKind kind = ProtoKind.Unknown, string? purpose = null)
    {
        DspCore.Protos.Register(protoType, proto, ownerModGuid, phase, kind, purpose);
    }

    /// <summary>
    /// 注册一个物品原型。
    /// Registers an item proto.
    /// </summary>
    public static void RegisterItem(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.Protos.RegisterItem(proto, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 注册一个配方原型。
    /// Registers a recipe proto.
    /// </summary>
    public static void RegisterRecipe(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.Protos.RegisterRecipe(proto, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 注册一个科技原型。
    /// Registers a tech proto.
    /// </summary>
    public static void RegisterTech(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.Protos.RegisterTech(proto, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 注册一个指引或教程原型。
    /// Registers a guide or tutorial proto.
    /// </summary>
    public static void RegisterTutorial(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.Protos.RegisterTutorial(proto, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 旧 PreAdd 兼容入口，后续可能无法等价映射到 DSPCore 新阶段。
    /// Legacy PreAdd bridge; it may not map exactly to DSPCore's new phases later.
    /// </summary>
    [Obsolete("Use Register(..., CoreDataPhase.Data, ...) or a typed RegisterItem/RegisterRecipe/RegisterTech method instead.")]
    public static void RegisterPreload(Type protoType, object proto, string ownerModGuid)
    {
        DspCore.Protos.RegisterPreload(protoType, proto, ownerModGuid);
    }

    /// <summary>
    /// 旧 PostAdd 兼容入口，后续可能无法等价映射到 DSPCore 新阶段。
    /// Legacy PostAdd bridge; it may not map exactly to DSPCore's new phases later.
    /// </summary>
    [Obsolete("Use Register(..., CoreDataPhase.DataFinalFixes, ...) or a typed RegisterItem/RegisterRecipe/RegisterTech method instead.")]
    public static void RegisterPostload(Type protoType, object proto, string ownerModGuid)
    {
        DspCore.Protos.RegisterPostload(protoType, proto, ownerModGuid);
    }

    /// <summary>
    /// 获取指定阶段的原型注册。
    /// Gets proto registrations for a phase.
    /// </summary>
    public static IReadOnlyList<ProtoRegistration> GetByPhase(CoreDataPhase phase)
    {
        return DspCore.Protos.GetByPhase(phase);
    }

    /// <summary>
    /// 获取所有已注册的 Proto。
    /// Gets all registered Protos.
    /// </summary>
    public static IReadOnlyList<ProtoRegistration> GetAll()
    {
        return DspCore.Protos.GetAll();
    }
}
