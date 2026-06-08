using System;

namespace DSPCore;

/// <summary>
/// 描述对原版数据的安全读取入口。
/// Describes a safe read entry point for vanilla data.
/// </summary>
public sealed class VanillaDataView
{
    /// <summary>
    /// 记录一个原版物品读取请求，真实 LDB 读取由运行时适配层完成。
    /// Records a vanilla item read request; real LDB reads are performed by the runtime adapter.
    /// </summary>
    /// <param name="itemId">物品 ID。Item id.</param>
    /// <param name="consumerModGuid">调用方模组 GUID。Calling mod GUID.</param>
    /// <returns>原版数据读取描述。Vanilla data query descriptor.</returns>
    public VanillaDataQuery GetItem(int itemId, string consumerModGuid)
    {
        return new VanillaDataQuery(ProtoKind.Item, itemId, consumerModGuid);
    }

    /// <summary>
    /// 记录一个原版配方读取请求，真实 LDB 读取由运行时适配层完成。
    /// Records a vanilla recipe read request; real LDB reads are performed by the runtime adapter.
    /// </summary>
    /// <param name="recipeId">配方 ID。Recipe id.</param>
    /// <param name="consumerModGuid">调用方模组 GUID。Calling mod GUID.</param>
    /// <returns>原版数据读取描述。Vanilla data query descriptor.</returns>
    public VanillaDataQuery GetRecipe(int recipeId, string consumerModGuid)
    {
        return new VanillaDataQuery(ProtoKind.Recipe, recipeId, consumerModGuid);
    }

    /// <summary>
    /// 记录一个原版科技读取请求，真实 LDB 读取由运行时适配层完成。
    /// Records a vanilla tech read request; real LDB reads are performed by the runtime adapter.
    /// </summary>
    /// <param name="techId">科技 ID。Tech id.</param>
    /// <param name="consumerModGuid">调用方模组 GUID。Calling mod GUID.</param>
    /// <returns>原版数据读取描述。Vanilla data query descriptor.</returns>
    public VanillaDataQuery GetTech(int techId, string consumerModGuid)
    {
        return new VanillaDataQuery(ProtoKind.Tech, techId, consumerModGuid);
    }
}

/// <summary>
/// 描述一个原版数据读取请求。
/// Describes a vanilla data read request.
/// </summary>
/// <param name="Kind">原型功能类型。Proto feature kind.</param>
/// <param name="Id">原版 ID。Vanilla id.</param>
/// <param name="ConsumerModGuid">调用方模组 GUID。Calling mod GUID.</param>
public sealed record VanillaDataQuery(ProtoKind Kind, int Id, string ConsumerModGuid);
