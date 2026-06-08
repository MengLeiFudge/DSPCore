namespace DSPCore;

/// <summary>
/// 物品原型注册入口。
/// Author-facing item proto registration entry point.
/// </summary>
public static class Items
{
    /// <summary>
    /// 注册一个物品原型。
    /// Registers an item proto.
    /// </summary>
    public static void Register(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.ProtoRegistration.RegisterItem(proto, ownerModGuid, phase, purpose);
    }
}
