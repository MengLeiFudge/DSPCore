using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧原版游戏枚举扩展入口。
/// Author-facing entry point for vanilla game enum extensions.
/// </summary>
public static class GameEnums
{
    /// <summary>
    /// DSPCore 预留的自定义配方类型枚举值。
    /// Reserved custom recipe type enum value injected by DSPCore.
    /// </summary>
    public const int CustomRecipeTypeValue = 20;

    /// <summary>
    /// DSPCore 预留的自定义物品类型枚举值。
    /// Reserved custom item type enum value injected by DSPCore.
    /// </summary>
    public const int CustomItemTypeValue = 100;

    /// <summary>
    /// 注册一个自定义配方类型。
    /// Registers a custom recipe type.
    /// </summary>
    /// <param name="id">配方类型 ID。Recipe type ID.</param>
    /// <param name="ownerModGuid">所属模组 GUID。Owner mod GUID.</param>
    /// <param name="displayName">显示名称。Display name.</param>
    /// <param name="recipeIds">归属此类型的配方 ID。Recipe IDs that belong to this type.</param>
    /// <param name="assemblerItemIds">支持此类型的制作器物品 ID。Assembler item IDs that support this type.</param>
    public static void RegisterRecipeType(
        string id,
        string ownerModGuid,
        string displayName,
        IReadOnlyCollection<int>? recipeIds = null,
        IReadOnlyCollection<int>? assemblerItemIds = null)
    {
        RegisterRecipeType(new RecipeTypeDescriptor(id, ownerModGuid, displayName, recipeIds, assemblerItemIds));
    }

    /// <summary>
    /// 注册一个自定义配方类型。
    /// Registers a custom recipe type.
    /// </summary>
    /// <param name="descriptor">配方类型描述。Recipe type descriptor.</param>
    public static void RegisterRecipeType(RecipeTypeDescriptor descriptor)
    {
        DspCore.GameEnums.Register(descriptor);
    }

    /// <summary>
    /// 获取所有自定义配方类型。
    /// Gets all custom recipe types.
    /// </summary>
    /// <returns>配方类型描述集合。Recipe type descriptors.</returns>
    public static IReadOnlyCollection<RecipeTypeDescriptor> GetRecipeTypes()
    {
        return DspCore.GameEnums.GetAll();
    }

    /// <summary>
    /// 获取或分配配方类型运行时 ID。
    /// Gets or assigns a runtime ID for a recipe type.
    /// </summary>
    /// <param name="id">配方类型 ID。Recipe type ID.</param>
    /// <returns>运行时 ID。Runtime ID.</returns>
    public static int GetOrAssignRecipeTypeRuntimeId(string id)
    {
        return DspCore.GameEnums.GetOrAssignRuntimeId(id);
    }

    /// <summary>
    /// 检查指定制作器实体是否可使用指定配方。
    /// Checks whether an assembler entity can use a recipe.
    /// </summary>
    /// <param name="assemblerEntityId">制作器实体 ID。Assembler entity ID.</param>
    /// <param name="recipeId">配方 ID。Recipe ID.</param>
    /// <returns>允许使用时返回 true。Returns true when allowed.</returns>
    public static bool CanAssemblerUseRecipe(int assemblerEntityId, int recipeId)
    {
        return RecipeTypeRuntime.CanAssemblerUseRecipe(assemblerEntityId, recipeId);
    }

    /// <summary>
    /// 把物品原型标记为 DSPCore 自定义物品类型。
    /// Marks an item proto as the DSPCore custom item type.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    public static void SetCustomItemType(ItemProto item)
    {
        if (item != null)
        {
            item.Type = (EItemType)CustomItemTypeValue;
        }
    }

    /// <summary>
    /// 判断物品原型是否使用 DSPCore 自定义物品类型。
    /// Checks whether an item proto uses the DSPCore custom item type.
    /// </summary>
    /// <param name="item">物品原型。Item proto.</param>
    /// <returns>使用自定义物品类型时返回 true。Returns true when the item uses the custom item type.</returns>
    public static bool IsCustomItemType(ItemProto item)
    {
        return item != null && (int)item.Type == CustomItemTypeValue;
    }
}
