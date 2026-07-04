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
    /// 注册一个带稳定身份的配方原型并返回原对象，便于链式作者调用。
    /// Registers a recipe proto with a stable identity and returns the same object for author-side chaining.
    /// </summary>
    /// <param name="proto">配方原型。Recipe proto.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="stableId">稳定 Proto 身份。Stable proto identity.</param>
    /// <param name="phase">数据阶段。Data phase.</param>
    /// <param name="purpose">注册目的说明。Registration purpose.</param>
    /// <returns>同一个配方原型，便于继续链式调用。The same recipe proto for chaining.</returns>
    public static RecipeProto Register(RecipeProto proto, string ownerModGuid, ProtoStableId stableId, CoreDataPhase phase = CoreDataPhase.Data, string? purpose = null)
    {
        if (proto == null)
        {
            throw new ArgumentNullException(nameof(proto));
        }

        DspCore.ProtoRegistration.RegisterRecipe(proto, ownerModGuid, stableId, phase, purpose);
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
