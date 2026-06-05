using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧功能块声明入口。
/// Author-facing feature block declaration entry point.
/// </summary>
public static class Features
{
    /// <summary>
    /// 注册一个功能块。
    /// Registers a feature block.
    /// </summary>
    public static void Register(FeatureDescriptor descriptor)
    {
        DspCore.Features.Register(descriptor);
    }

    /// <summary>
    /// 尝试按 ID 获取功能块。
    /// Tries to get a feature block by id.
    /// </summary>
    public static bool TryGet(string id, out FeatureDescriptor descriptor)
    {
        return DspCore.Features.TryGet(id, out descriptor);
    }

    /// <summary>
    /// 获取所有功能块，按优先级和 ID 排序。
    /// Gets all feature blocks ordered by priority and id.
    /// </summary>
    public static IReadOnlyList<FeatureDescriptor> GetAll()
    {
        return DspCore.Features.GetAll();
    }
}
