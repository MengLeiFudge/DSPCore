using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理游戏枚举扩展声明。
/// Manages game enum extension declarations.
/// </summary>
public class GameEnumRegistry
{
    private readonly Dictionary<string, RecipeTypeDescriptor> recipeTypes = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ItemTypeDescriptor> itemTypes = new(StringComparer.Ordinal);
    private readonly Dictionary<string, int> recipeTypeRuntimeIds = new(StringComparer.Ordinal);
    private readonly Dictionary<string, int> itemTypeRuntimeIds = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册一个自定义配方类型。
    /// Registers a custom recipe type.
    /// </summary>
    /// <param name="descriptor">配方类型描述。Recipe type descriptor.</param>
    public void Register(RecipeTypeDescriptor descriptor)
    {
        if (descriptor == null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        recipeTypes[descriptor.Id] = descriptor;
    }

    /// <summary>
    /// 注册一个自定义物品类型。
    /// Registers a custom item type.
    /// </summary>
    /// <param name="descriptor">物品类型描述。Item type descriptor.</param>
    public void Register(ItemTypeDescriptor descriptor)
    {
        if (descriptor == null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        itemTypes[descriptor.Id] = descriptor;
    }

    /// <summary>
    /// 获取所有自定义配方类型。
    /// Gets all custom recipe types.
    /// </summary>
    /// <returns>配方类型描述快照。Snapshot of recipe type descriptors.</returns>
    public IReadOnlyCollection<RecipeTypeDescriptor> GetAll()
    {
        return GetRecipeTypes();
    }

    /// <summary>
    /// 获取所有自定义配方类型。
    /// Gets all custom recipe types.
    /// </summary>
    /// <returns>配方类型描述快照。Snapshot of recipe type descriptors.</returns>
    public IReadOnlyCollection<RecipeTypeDescriptor> GetRecipeTypes()
    {
        return recipeTypes.Values;
    }

    /// <summary>
    /// 获取所有自定义物品类型。
    /// Gets all custom item types.
    /// </summary>
    /// <returns>物品类型描述快照。Snapshot of item type descriptors.</returns>
    public IReadOnlyCollection<ItemTypeDescriptor> GetItemTypes()
    {
        return itemTypes.Values;
    }

    /// <summary>
    /// 获取或分配配方类型运行时 ID。
    /// Gets or assigns a runtime id for a recipe type.
    /// </summary>
    /// <param name="id">配方类型 ID。Recipe type id.</param>
    /// <returns>运行时 ID。Runtime id.</returns>
    public int GetOrAssignRuntimeId(string id)
    {
        return GetOrAssignRecipeTypeRuntimeId(id);
    }

    /// <summary>
    /// 获取或分配配方类型运行时 ID。
    /// Gets or assigns a runtime id for a recipe type.
    /// </summary>
    /// <param name="id">配方类型 ID。Recipe type id.</param>
    /// <returns>运行时 ID。Runtime id.</returns>
    public int GetOrAssignRecipeTypeRuntimeId(string id)
    {
        return GetOrAssignRuntimeId(recipeTypeRuntimeIds, id);
    }

    /// <summary>
    /// 获取或分配物品类型运行时 ID。
    /// Gets or assigns a runtime id for an item type.
    /// </summary>
    /// <param name="id">物品类型 ID。Item type id.</param>
    /// <returns>运行时 ID。Runtime id.</returns>
    public int GetOrAssignItemTypeRuntimeId(string id)
    {
        return GetOrAssignRuntimeId(itemTypeRuntimeIds, id);
    }

    private static int GetOrAssignRuntimeId(IDictionary<string, int> runtimeIds, string id)
    {
        if (runtimeIds.TryGetValue(id, out var runtimeId))
        {
            return runtimeId;
        }

        runtimeId = runtimeIds.Count + 1;
        runtimeIds[id] = runtimeId;
        return runtimeId;
    }
}
