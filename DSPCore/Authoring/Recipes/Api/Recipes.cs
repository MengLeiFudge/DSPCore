namespace DSPCore;

/// <summary>
/// 配方原型注册入口。
/// Author-facing recipe proto registration entry point.
/// </summary>
public static class Recipes
{
    /// <summary>
    /// 注册一个配方原型。
    /// Registers a recipe proto.
    /// </summary>
    public static void Register(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.ProtoRegistration.RegisterRecipe(proto, ownerModGuid, phase, purpose);
    }
}
