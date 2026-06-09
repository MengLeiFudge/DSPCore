using System;

namespace DSPCore;

/// <summary>
/// 配方原型注册入口。
/// Author-facing recipe proto registration entry point.
/// </summary>
public static class Recipes
{
    /// <summary>
    /// 注册一个配方原型并返回原对象，便于链式作者调用。
    /// Registers a recipe proto and returns the same object for author-side chaining.
    /// </summary>
    public static RecipeProto Register(RecipeProto proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (proto == null)
        {
            throw new ArgumentNullException(nameof(proto));
        }

        Register((object)proto, ownerModGuid, phase, purpose);
        return proto;
    }

    /// <summary>
    /// 注册一个配方原型。
    /// Registers a recipe proto.
    /// </summary>
    public static void Register(object proto, string ownerModGuid, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        DspCore.ProtoRegistration.RegisterRecipe(proto, ownerModGuid, phase, purpose);
    }
}
