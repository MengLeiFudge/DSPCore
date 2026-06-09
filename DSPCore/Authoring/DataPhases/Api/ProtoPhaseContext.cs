using System;

namespace DSPCore;

/// <summary>
/// 提供某个原型数据阶段内的注册上下文。
/// Provides the registration context for one proto data phase.
/// </summary>
public sealed class ProtoPhaseContext
{
    internal ProtoPhaseContext(string ownerModGuid, CoreDataPhase phase, ProtoRegistryFacade registry)
    {
        OwnerModGuid = ownerModGuid;
        Phase = phase;
        registryFacade = registry;
        Access = new ProtoAccessView(registry, phase);
    }

    private readonly ProtoRegistryFacade registryFacade;

    /// <summary>
    /// 当前阶段声明所属模组 GUID。
    /// Gets the mod GUID that owns the current phase declaration.
    /// </summary>
    public string OwnerModGuid { get; }

    /// <summary>
    /// 当前执行的数据阶段。
    /// Gets the currently executing data phase.
    /// </summary>
    public CoreDataPhase Phase { get; }

    /// <summary>
    /// 当前阶段可见 Proto 的查询和修改入口。
    /// Gets lookup and mutation access to protos visible in this phase.
    /// </summary>
    public ProtoAccessView Access { get; }

    /// <summary>
    /// 查找当前阶段可见物品原型。
    /// Finds an item proto visible in the current phase.
    /// </summary>
    /// <param name="itemId">物品 ID。Item ID.</param>
    /// <returns>物品原型；找不到时返回 null。Item proto, or null when missing.</returns>
    public ItemProto? FindItem(int itemId)
    {
        return Access.FindItem(itemId);
    }

    /// <summary>
    /// 查找当前阶段可见配方原型。
    /// Finds a recipe proto visible in the current phase.
    /// </summary>
    /// <param name="recipeId">配方 ID。Recipe ID.</param>
    /// <returns>配方原型；找不到时返回 null。Recipe proto, or null when missing.</returns>
    public RecipeProto? FindRecipe(int recipeId)
    {
        return Access.FindRecipe(recipeId);
    }

    /// <summary>
    /// 查找当前阶段可见科技原型。
    /// Finds a tech proto visible in the current phase.
    /// </summary>
    /// <param name="techId">科技 ID。Tech ID.</param>
    /// <returns>科技原型；找不到时返回 null。Tech proto, or null when missing.</returns>
    public TechProto? FindTech(int techId)
    {
        return Access.FindTech(techId);
    }

    /// <summary>
    /// 查找当前阶段可见教程原型。
    /// Finds a tutorial proto visible in the current phase.
    /// </summary>
    /// <param name="tutorialId">教程 ID。Tutorial ID.</param>
    /// <returns>教程原型；找不到时返回 null。Tutorial proto, or null when missing.</returns>
    public TutorialProto? FindTutorial(int tutorialId)
    {
        return Access.FindTutorial(tutorialId);
    }

    /// <summary>
    /// 在当前阶段注册一个原型对象。
    /// Registers a proto object in the current phase.
    /// </summary>
    public void Register(Type protoType, object proto, ProtoKind kind = ProtoKind.Unknown, string? purpose = null)
    {
        registryFacade.Register(protoType, proto, OwnerModGuid, Phase, kind, purpose);
    }

    /// <summary>
    /// 在当前阶段注册一个物品原型。
    /// Registers an item proto in the current phase.
    /// </summary>
    public void RegisterItem(object proto, string? purpose = null)
    {
        registryFacade.RegisterItem(proto, OwnerModGuid, Phase, purpose);
    }

    /// <summary>
    /// 在当前阶段注册一个配方原型。
    /// Registers a recipe proto in the current phase.
    /// </summary>
    public void RegisterRecipe(object proto, string? purpose = null)
    {
        registryFacade.RegisterRecipe(proto, OwnerModGuid, Phase, purpose);
    }

    /// <summary>
    /// 在当前阶段注册一个科技原型。
    /// Registers a tech proto in the current phase.
    /// </summary>
    public void RegisterTech(object proto, string? purpose = null)
    {
        registryFacade.RegisterTech(proto, OwnerModGuid, Phase, purpose);
    }

    /// <summary>
    /// 在当前阶段注册一个指引或教程原型。
    /// Registers a guide or tutorial proto in the current phase.
    /// </summary>
    public void RegisterTutorial(object proto, string? purpose = null)
    {
        registryFacade.RegisterTutorial(proto, OwnerModGuid, Phase, purpose);
    }
}
