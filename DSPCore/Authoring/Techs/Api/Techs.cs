namespace DSPCore;

/// <summary>
/// 科技原型注册入口。
/// Author-facing tech proto registration entry point.
/// </summary>
public static class Techs
{
    /// <summary>
    /// 注册一个科技原型。
    /// Registers a tech proto.
    /// </summary>
    public static void Register(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.ProtoRegistration.RegisterTech(proto, ownerModGuid, phase, purpose);
    }
}
