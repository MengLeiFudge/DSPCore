using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 管理自定义配方类型声明。
/// Manages custom recipe type declarations.
/// </summary>
public sealed class RecipeTypeRegistry
{
    private readonly Dictionary<string, RecipeTypeDescriptor> recipeTypes = new(StringComparer.Ordinal);
    private readonly Dictionary<string, int> runtimeIds = new(StringComparer.Ordinal);

    /// <summary>
    /// 注册一个自定义配方类型。
    /// Registers a custom recipe type.
    /// </summary>
    /// <param name="descriptor">配方类型描述。Recipe type descriptor.</param>
    public void Register(RecipeTypeDescriptor descriptor)
    {
        recipeTypes[descriptor.Id] = descriptor;
    }

    /// <summary>
    /// 获取所有自定义配方类型。
    /// Gets all custom recipe types.
    /// </summary>
    /// <returns>配方类型描述快照。Snapshot of recipe type descriptors.</returns>
    public IReadOnlyCollection<RecipeTypeDescriptor> GetAll()
    {
        return recipeTypes.Values;
    }

    /// <summary>
    /// 获取或分配配方类型运行时 ID。
    /// Gets or assigns a runtime id for a recipe type.
    /// </summary>
    /// <param name="id">配方类型 ID。Recipe type id.</param>
    /// <returns>运行时 ID。Runtime id.</returns>
    public int GetOrAssignRuntimeId(string id)
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
