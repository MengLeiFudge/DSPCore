using System;
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
    /// <param name="id">功能块 ID。Feature block id.</param>
    /// <param name="displayName">显示名称。Display name.</param>
    /// <param name="initialize">初始化回调。Initialization callback.</param>
    /// <param name="priority">初始化优先级，数值越小越早。Initialization priority; lower values run earlier.</param>
    /// <param name="dependencies">依赖的功能块 ID。Dependent feature block ids.</param>
    public static void Register(
        string id,
        string displayName,
        Action initialize,
        int priority = 0,
        IReadOnlyList<string>? dependencies = null)
    {
        Register(new FeatureDescriptor(id, displayName, priority, initialize, dependencies));
    }

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
