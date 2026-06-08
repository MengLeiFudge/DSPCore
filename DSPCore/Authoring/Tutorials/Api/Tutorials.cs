namespace DSPCore;

/// <summary>
/// 指引或教程原型注册入口。
/// Author-facing guide or tutorial proto registration entry point.
/// </summary>
public static class Tutorials
{
    /// <summary>
    /// 注册一个指引或教程原型。
    /// Registers a guide or tutorial proto.
    /// </summary>
    public static void Register(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.ProtoRegistration.RegisterTutorial(proto, ownerModGuid, phase, purpose);
    }
}
