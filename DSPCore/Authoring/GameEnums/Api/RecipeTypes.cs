using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧自定义配方类型入口。
/// Author-facing custom recipe type entry point.
/// </summary>
public static class RecipeTypes
{
    /// <summary>
    /// 注册一个自定义配方类型。
    /// Registers a custom recipe type.
    /// </summary>
    public static void Register(RecipeTypeDescriptor descriptor)
    {
        DspCore.RecipeTypes.Register(descriptor);
    }

    /// <summary>
    /// 获取所有自定义配方类型。
    /// Gets all custom recipe types.
    /// </summary>
    public static IReadOnlyCollection<RecipeTypeDescriptor> GetAll()
    {
        return DspCore.RecipeTypes.GetAll();
    }

    /// <summary>
    /// 获取或分配配方类型运行时 ID。
    /// Gets or assigns a runtime id for a recipe type.
    /// </summary>
    public static int GetOrAssignRuntimeId(string id)
    {
        return DspCore.RecipeTypes.GetOrAssignRuntimeId(id);
    }
}
