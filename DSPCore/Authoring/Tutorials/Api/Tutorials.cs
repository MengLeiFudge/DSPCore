using System;

namespace DSPCore;

/// <summary>
/// 指引或教程原型注册入口。
/// Author-facing guide or tutorial proto registration entry point.
/// </summary>
public static class Tutorials
{
    /// <summary>
    /// 注册一个指引或教程原型并返回原对象。
    /// Registers a guide or tutorial proto and returns the same object.
    /// </summary>
    /// <param name="proto">指引或教程原型。Guide or tutorial proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个指引或教程原型，便于继续链式调用。The same guide or tutorial proto for chaining.</returns>
    public static TutorialProto Register(TutorialProto proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (proto == null)
        {
            throw new ArgumentNullException(nameof(proto));
        }

        DspCore.ProtoRegistration.RegisterTutorial(proto, ownerModGuid, phase, purpose);
        return proto;
    }

    /// <summary>
    /// 注册一个带稳定身份的指引或教程原型并返回原对象。
    /// Registers a guide or tutorial proto with a stable identity and returns the same object.
    /// </summary>
    /// <param name="proto">指引或教程原型。Guide or tutorial proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="stableId">稳定 Proto 身份。Stable proto identity.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个指引或教程原型，便于继续链式调用。The same guide or tutorial proto for chaining.</returns>
    public static TutorialProto Register(TutorialProto proto, string ownerModGuid, ProtoStableId stableId, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (proto == null)
        {
            throw new ArgumentNullException(nameof(proto));
        }

        DspCore.ProtoRegistration.RegisterTutorial(proto, ownerModGuid, stableId, phase, purpose);
        return proto;
    }

    /// <summary>
    /// 注册一个指引或教程原型。
    /// Registers a guide or tutorial proto.
    /// </summary>
    public static void Register(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.ProtoRegistration.RegisterTutorial(proto, ownerModGuid, phase, purpose);
    }
}
