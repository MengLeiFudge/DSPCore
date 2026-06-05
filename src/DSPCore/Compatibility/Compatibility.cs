using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧兼容补丁声明入口。
/// Author-facing compatibility patch declaration entry point.
/// </summary>
public static class Compatibility
{
    /// <summary>
    /// 注册一个兼容补丁。
    /// Registers a compatibility patch.
    /// </summary>
    public static void Register(CompatibilityPatchDescriptor descriptor)
    {
        DspCore.Compatibility.Register(descriptor);
    }

    /// <summary>
    /// 查找适用于指定目标模组的兼容补丁。
    /// Finds compatibility patches for a target mod.
    /// </summary>
    public static IReadOnlyList<CompatibilityPatchDescriptor> FindForMod(string targetModGuid)
    {
        return DspCore.Compatibility.FindForMod(targetModGuid);
    }

    /// <summary>
    /// 获取所有兼容补丁。
    /// Gets all compatibility patches.
    /// </summary>
    public static IReadOnlyList<CompatibilityPatchDescriptor> GetAll()
    {
        return DspCore.Compatibility.GetAll();
    }
}
