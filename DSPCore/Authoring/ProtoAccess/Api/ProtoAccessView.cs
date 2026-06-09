using System.Collections.Generic;
using System.Linq;

namespace DSPCore;

/// <summary>
/// 提供阶段内可见 Proto 的查询和修改入口。
/// Provides lookup and mutation access to protos visible in a data phase.
/// </summary>
public sealed class ProtoAccessView
{
    internal ProtoAccessView(ProtoRegistryFacade registry, CoreDataPhase phase)
    {
        this.registry = registry;
        Phase = phase;
    }

    private readonly ProtoRegistryFacade registry;

    /// <summary>
    /// 当前访问所在的数据阶段。
    /// Gets the data phase that owns this access view.
    /// </summary>
    public CoreDataPhase Phase { get; }

    /// <summary>
    /// 当前阶段是否适合修改跨模组数据。
    /// Gets whether the current phase is suitable for cross-mod mutation.
    /// </summary>
    public bool CanMutate => Phase == CoreDataPhase.DataUpdates || Phase == CoreDataPhase.DataFinalFixes;

    /// <summary>
    /// 查找可见物品原型。
    /// Finds a visible item proto.
    /// </summary>
    /// <param name="itemId">物品 ID。Item ID.</param>
    /// <returns>物品原型；找不到时返回 null。Item proto, or null when missing.</returns>
    public ItemProto? FindItem(int itemId)
    {
        return FindRegistered<ItemProto>(itemId, ProtoKind.Item) ?? LDB.items?.Select(itemId);
    }

    /// <summary>
    /// 尝试查找可见物品原型。
    /// Tries to find a visible item proto.
    /// </summary>
    /// <param name="itemId">物品 ID。Item ID.</param>
    /// <param name="proto">返回的物品原型。Returned item proto.</param>
    /// <returns>是否找到。Whether it was found.</returns>
    public bool TryGetItem(int itemId, out ItemProto proto)
    {
        proto = FindItem(itemId)!;
        return proto != null;
    }

    /// <summary>
    /// 获取当前可见的全部物品原型。
    /// Gets all currently visible item protos.
    /// </summary>
    /// <returns>物品原型集合。Item proto collection.</returns>
    public IReadOnlyList<ItemProto> Items()
    {
        return MergeVisible(Registered<ItemProto>(ProtoKind.Item), LDB.items?.dataArray);
    }

    /// <summary>
    /// 查找可见配方原型。
    /// Finds a visible recipe proto.
    /// </summary>
    /// <param name="recipeId">配方 ID。Recipe ID.</param>
    /// <returns>配方原型；找不到时返回 null。Recipe proto, or null when missing.</returns>
    public RecipeProto? FindRecipe(int recipeId)
    {
        return FindRegistered<RecipeProto>(recipeId, ProtoKind.Recipe) ?? LDB.recipes?.Select(recipeId);
    }

    /// <summary>
    /// 尝试查找可见配方原型。
    /// Tries to find a visible recipe proto.
    /// </summary>
    /// <param name="recipeId">配方 ID。Recipe ID.</param>
    /// <param name="proto">返回的配方原型。Returned recipe proto.</param>
    /// <returns>是否找到。Whether it was found.</returns>
    public bool TryGetRecipe(int recipeId, out RecipeProto proto)
    {
        proto = FindRecipe(recipeId)!;
        return proto != null;
    }

    /// <summary>
    /// 获取当前可见的全部配方原型。
    /// Gets all currently visible recipe protos.
    /// </summary>
    /// <returns>配方原型集合。Recipe proto collection.</returns>
    public IReadOnlyList<RecipeProto> Recipes()
    {
        return MergeVisible(Registered<RecipeProto>(ProtoKind.Recipe), LDB.recipes?.dataArray);
    }

    /// <summary>
    /// 查找可见科技原型。
    /// Finds a visible tech proto.
    /// </summary>
    /// <param name="techId">科技 ID。Tech ID.</param>
    /// <returns>科技原型；找不到时返回 null。Tech proto, or null when missing.</returns>
    public TechProto? FindTech(int techId)
    {
        return FindRegistered<TechProto>(techId, ProtoKind.Tech) ?? LDB.techs?.Select(techId);
    }

    /// <summary>
    /// 尝试查找可见科技原型。
    /// Tries to find a visible tech proto.
    /// </summary>
    /// <param name="techId">科技 ID。Tech ID.</param>
    /// <param name="proto">返回的科技原型。Returned tech proto.</param>
    /// <returns>是否找到。Whether it was found.</returns>
    public bool TryGetTech(int techId, out TechProto proto)
    {
        proto = FindTech(techId)!;
        return proto != null;
    }

    /// <summary>
    /// 获取当前可见的全部科技原型。
    /// Gets all currently visible tech protos.
    /// </summary>
    /// <returns>科技原型集合。Tech proto collection.</returns>
    public IReadOnlyList<TechProto> Techs()
    {
        return MergeVisible(Registered<TechProto>(ProtoKind.Tech), LDB.techs?.dataArray);
    }

    /// <summary>
    /// 查找可见教程原型。
    /// Finds a visible tutorial proto.
    /// </summary>
    /// <param name="tutorialId">教程 ID。Tutorial ID.</param>
    /// <returns>教程原型；找不到时返回 null。Tutorial proto, or null when missing.</returns>
    public TutorialProto? FindTutorial(int tutorialId)
    {
        return FindRegistered<TutorialProto>(tutorialId, ProtoKind.Tutorial) ?? LDB.tutorial?.Select(tutorialId);
    }

    /// <summary>
    /// 尝试查找可见教程原型。
    /// Tries to find a visible tutorial proto.
    /// </summary>
    /// <param name="tutorialId">教程 ID。Tutorial ID.</param>
    /// <param name="proto">返回的教程原型。Returned tutorial proto.</param>
    /// <returns>是否找到。Whether it was found.</returns>
    public bool TryGetTutorial(int tutorialId, out TutorialProto proto)
    {
        proto = FindTutorial(tutorialId)!;
        return proto != null;
    }

    /// <summary>
    /// 获取当前可见的全部教程原型。
    /// Gets all currently visible tutorial protos.
    /// </summary>
    /// <returns>教程原型集合。Tutorial proto collection.</returns>
    public IReadOnlyList<TutorialProto> Tutorials()
    {
        return MergeVisible(Registered<TutorialProto>(ProtoKind.Tutorial), LDB.tutorial?.dataArray);
    }

    private T? FindRegistered<T>(int id, ProtoKind kind)
        where T : Proto
    {
        return Registered<T>(kind)
            .LastOrDefault(proto => proto.ID == id);
    }

    private IEnumerable<T> Registered<T>(ProtoKind kind)
        where T : Proto
    {
        return registry.GetVisibleEntries(Phase)
            .Where(entry => entry.Kind == kind || entry.Proto is T)
            .Select(entry => entry.Proto)
            .OfType<T>();
    }

    private static IReadOnlyList<T> MergeVisible<T>(IEnumerable<T> registered, IEnumerable<T>? ldb)
        where T : Proto
    {
        var byId = new Dictionary<int, T>();
        if (ldb != null)
        {
            foreach (var proto in ldb)
            {
                if (proto != null)
                {
                    byId[proto.ID] = proto;
                }
            }
        }

        foreach (var proto in registered)
        {
            byId[proto.ID] = proto;
        }

        return byId.Values.OrderBy(proto => proto.ID).ToArray();
    }
}
