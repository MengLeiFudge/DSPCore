using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧自定义配方类型旧入口。
/// Legacy author-facing custom recipe type entry point.
/// </summary>
public static class RecipeTypes
{
    /// <summary>
    /// 注册一个自定义配方类型。
    /// Registers a custom recipe type.
    /// </summary>
    /// <param name="id">配方类型 ID。Recipe type ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="displayName">显示名称。Display name.</param>
    /// <param name="recipeIds">归属此类型的配方 ID。Recipe IDs that belong to this type.</param>
    /// <param name="assemblerItemIds">支持此类型的制作器物品 ID。Assembler item IDs that support this type.</param>
    public static void Register(
        string id,
        string ownerModGuid,
        string displayName,
        IReadOnlyCollection<int>? recipeIds = null,
        IReadOnlyCollection<int>? assemblerItemIds = null)
    {
        GameEnums.RegisterRecipeType(id, ownerModGuid, displayName, recipeIds, assemblerItemIds);
    }

    /// <summary>
    /// 注册一个自定义配方类型描述。
    /// Registers a custom recipe type descriptor.
    /// </summary>
    /// <param name="descriptor">配方类型描述。Recipe type descriptor.</param>
    public static void Register(RecipeTypeDescriptor descriptor)
    {
        GameEnums.RegisterRecipeType(descriptor);
    }

    /// <summary>
    /// 获取所有自定义配方类型。
    /// Gets all custom recipe types.
    /// </summary>
    public static IReadOnlyCollection<RecipeTypeDescriptor> GetAll()
    {
        return GameEnums.GetRecipeTypes();
    }

    /// <summary>
    /// 获取或分配配方类型运行时 ID。
    /// Gets or assigns a runtime id for a recipe type.
    /// </summary>
    public static int GetOrAssignRuntimeId(string id)
    {
        return GameEnums.GetOrAssignRecipeTypeRuntimeId(id);
    }
}
