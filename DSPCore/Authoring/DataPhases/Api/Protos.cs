using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 原型注册的短别名入口。
/// Short alias entry point for proto registration.
/// </summary>
public static class Protos
{
    /// <summary>
    /// 注册一个 Data 阶段回调。
    /// Registers a Data phase callback.
    /// </summary>
    public static void Data(string ownerModGuid, Action<ProtoPhaseContext> configure, int priority = 0, string? purpose = null)
    {
        ProtoRegistration.Data(ownerModGuid, configure, priority, purpose);
    }

    /// <summary>
    /// 注册一个 DataUpdates 阶段回调。
    /// Registers a DataUpdates phase callback.
    /// </summary>
    public static void DataUpdates(string ownerModGuid, Action<ProtoPhaseContext> configure, int priority = 0, string? purpose = null)
    {
        ProtoRegistration.DataUpdates(ownerModGuid, configure, priority, purpose);
    }

    /// <summary>
    /// 注册一个 DataFinalFixes 阶段回调。
    /// Registers a DataFinalFixes phase callback.
    /// </summary>
    public static void DataFinalFixes(string ownerModGuid, Action<ProtoPhaseContext> configure, int priority = 0, string? purpose = null)
    {
        ProtoRegistration.DataFinalFixes(ownerModGuid, configure, priority, purpose);
    }

    /// <summary>
    /// 注册一个指定数据阶段的回调。
    /// Registers a callback for a specific data phase.
    /// </summary>
    public static void RegisterPhaseAction(string ownerModGuid, CoreDataPhase phase, Action<ProtoPhaseContext> configure, int priority = 0, string? purpose = null)
    {
        ProtoRegistration.RegisterPhaseAction(ownerModGuid, phase, configure, priority, purpose);
    }

    /// <summary>
    /// 从分页槽位、行和格子编号生成物品或配方 GridIndex。
    /// Creates an item or recipe GridIndex from a tab slot, row, and index.
    /// </summary>
    public static int GetGridIndex(TabSlot tab, int row, int index)
    {
        return ProtoRegistration.GetGridIndex(tab, row, index);
    }

    /// <summary>
    /// 从游戏分类编号、行和格子编号生成物品或配方 GridIndex。
    /// Creates an item or recipe GridIndex from a game category value, row, and index.
    /// </summary>
    public static int GetGridIndex(int tab, int row, int index)
    {
        return ProtoRegistration.GetGridIndex(tab, row, index);
    }

    /// <summary>
    /// 注册一个原型对象。
    /// Registers a proto object.
    /// </summary>
    public static void Register(Type protoType, object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, ProtoKind kind = ProtoKind.Unknown, string? purpose = null)
    {
        ProtoRegistration.Register(protoType, proto, ownerModGuid, phase, kind, purpose);
    }

    /// <summary>
    /// 注册一个物品原型。
    /// Registers an item proto.
    /// </summary>
    public static void RegisterItem(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        ProtoRegistration.RegisterItem(proto, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 注册一个配方原型。
    /// Registers a recipe proto.
    /// </summary>
    public static void RegisterRecipe(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        ProtoRegistration.RegisterRecipe(proto, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 注册一个科技原型。
    /// Registers a tech proto.
    /// </summary>
    public static void RegisterTech(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        ProtoRegistration.RegisterTech(proto, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 注册一个指引或教程原型。
    /// Registers a guide or tutorial proto.
    /// </summary>
    public static void RegisterTutorial(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        ProtoRegistration.RegisterTutorial(proto, ownerModGuid, phase, purpose);
    }

    /// <summary>
    /// 旧 PreAdd 兼容入口，后续可能无法等价映射到 DSPCore 新阶段。
    /// Legacy PreAdd bridge; it may not map exactly to DSPCore's new phases later.
    /// </summary>
    [Obsolete("Use Register(..., CoreDataPhase.Data, ...) or a typed RegisterItem/RegisterRecipe/RegisterTech method instead.")]
    public static void RegisterPreload(Type protoType, object proto, string ownerModGuid)
    {
        ProtoRegistration.RegisterPreload(protoType, proto, ownerModGuid);
    }

    /// <summary>
    /// 旧 PostAdd 兼容入口，后续可能无法等价映射到 DSPCore 新阶段。
    /// Legacy PostAdd bridge; it may not map exactly to DSPCore's new phases later.
    /// </summary>
    [Obsolete("Use Register(..., CoreDataPhase.DataFinalFixes, ...) or a typed RegisterItem/RegisterRecipe/RegisterTech method instead.")]
    public static void RegisterPostload(Type protoType, object proto, string ownerModGuid)
    {
        ProtoRegistration.RegisterPostload(protoType, proto, ownerModGuid);
    }

    /// <summary>
    /// 获取指定阶段的原型注册。
    /// Gets proto registrations for a phase.
    /// </summary>
    public static IReadOnlyList<ProtoRegistrationEntry> GetByPhase(CoreDataPhase phase)
    {
        return ProtoRegistration.GetByPhase(phase);
    }

    /// <summary>
    /// 获取所有已注册的原型注册项。
    /// Gets all registered proto registration entries.
    /// </summary>
    public static IReadOnlyList<ProtoRegistrationEntry> GetAll()
    {
        return ProtoRegistration.GetAll();
    }
}
