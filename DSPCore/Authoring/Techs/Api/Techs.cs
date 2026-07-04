using System;

namespace DSPCore;

/// <summary>
/// 科技原型注册入口。
/// Author-facing tech proto registration entry point.
/// </summary>
public static class Techs
{
    /// <summary>
    /// 注册一个科技原型并返回原对象。
    /// Registers a tech proto and returns the same object.
    /// </summary>
    /// <param name="proto">科技原型。Tech proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto Register(TechProto proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (proto == null)
        {
            throw new ArgumentNullException(nameof(proto));
        }

        DspCore.ProtoRegistration.RegisterTech(proto, ownerModGuid, phase, purpose);
        return proto;
    }

    /// <summary>
    /// 注册一个带稳定身份的科技原型并返回原对象。
    /// Registers a tech proto with a stable identity and returns the same object.
    /// </summary>
    /// <param name="proto">科技原型。Tech proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="stableId">稳定 Proto 身份。Stable proto identity.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个科技原型，便于继续链式调用。The same tech proto for chaining.</returns>
    public static TechProto Register(TechProto proto, string ownerModGuid, ProtoStableId stableId, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (proto == null)
        {
            throw new ArgumentNullException(nameof(proto));
        }

        DspCore.ProtoRegistration.RegisterTech(proto, ownerModGuid, stableId, phase, purpose);
        return proto;
    }

    /// <summary>
    /// 注册一个科技原型。
    /// Registers a tech proto.
    /// </summary>
    public static void Register(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.ProtoRegistration.RegisterTech(proto, ownerModGuid, phase, purpose);
    }
}
