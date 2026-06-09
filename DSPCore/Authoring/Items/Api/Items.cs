using System;

namespace DSPCore;

/// <summary>
/// 物品原型注册入口。
/// Author-facing item proto registration entry point.
/// </summary>
public static class Items
{
    /// <summary>
    /// 注册一个物品原型并返回原对象，便于链式作者调用。
    /// Registers an item proto and returns the same object for author-side chaining.
    /// </summary>
    public static ItemProto Register(ItemProto proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (proto == null)
        {
            throw new ArgumentNullException(nameof(proto));
        }

        Register((object)proto, ownerModGuid, phase, purpose);
        return proto;
    }

    /// <summary>
    /// 注册一个物品原型。
    /// Registers an item proto.
    /// </summary>
    public static void Register(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.ProtoRegistration.RegisterItem(proto, ownerModGuid, phase, purpose);
    }
}
